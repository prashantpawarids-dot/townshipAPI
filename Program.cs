using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Township_API.Data;
using Township_API.Services;
using EventFlow.Configuration.Serialization;
using Township_API.Hubs; // ← ADD THIS

partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.SetIsOriginAllowed(_ => true)  // ← replace AllowAnyOrigin
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();             // ← add this
            });
        });

        builder.Services.AddDbContext<AppDBContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        //builder.Services.AddControllers()
        //.AddJsonOptions(options =>
        //{
        //    options.JsonSerializerOptions.IncludeFields = true;
        //});


        builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.IncludeFields = true;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.NumberHandling =
        System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
});

        builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new { message = "Validation failed", errors });
            };
        });


        builder.Services.AddResponseCompression(opts => opts.EnableForHttps = true);

        builder.Services.AddSignalR(options =>
        {
            options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB for base64 photos
        });


        //builder.Services.AddSingleton<SemacControllerService>();
        //builder.Services.AddHostedService(sp => sp.GetRequiredService<SemacControllerService>());

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<SemacListenerService>();
        //builder.Services.AddHostedService(sp => sp.GetRequiredService<SemacListenerService>());
        builder.Services.AddSingleton<SemacListenerService>();

        builder.Services.AddSingleton<Township_API.Services.ICameraPingService, Township_API.Services.CameraPingService>();
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<Township_API.Services.ICameraAuthService, Township_API.Services.CameraAuthService>();
        builder.Services.AddSingleton<Township_API.Services.FfmpegLiveViewHandler>();
        builder.Services.AddSingleton<IBackupConfigStore, BackupConfigStore>();
        builder.Services.AddScoped<IBackupService, BackupService>();
        builder.Services.AddHostedService<BackupWorker>();

        var app = builder.Build();

        app.UseCors("AllowAll");
        app.UseResponseCompression();

        app.UseWebSockets();

        app.Map("/api/ws/live-view", async context =>
        {
            if (!context.WebSockets.IsWebSocketRequest) { context.Response.StatusCode = 400; return; }
            var ip = context.Request.Query["ip"].ToString();
            if (string.IsNullOrWhiteSpace(ip)) { context.Response.StatusCode = 400; return; }

            var handler = context.RequestServices.GetRequiredService<Township_API.Services.FfmpegLiveViewHandler>();
            using var socket = await context.WebSockets.AcceptWebSocketAsync();
            await handler.HandleAsync(socket, ip, context.RequestAborted);
        });

        if (1 == 1 || app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<CardPreviewHub>("/hubs/cardpreview"); // ← ADD THIS
        app.MapHub<TransactionHub>("/hubs/transactions");

        app.Run();
    }
}