using Township_API.Models;

namespace Township_API.Services
{
    public class BackupWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BackupWorker> _logger;

        public BackupWorker(IServiceScopeFactory scopeFactory, ILogger<BackupWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var store = scope.ServiceProvider.GetRequiredService<IBackupConfigStore>();
                    var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();

                    var settings = store.GetSettings();

                    if (settings.Enabled && ShouldRunNow(settings))
                    {
                        _logger.LogInformation("Scheduled backup triggered.");
                        var entry = await backupService.RunBackupAsync(settings, "Scheduled");

                        settings.LastRunTime = entry.RunTime;
                        settings.LastRunStatus = entry.Success ? "Success" : "Failed";
                        settings.LastRunDateOnly = DateTime.Today;

                        store.SaveSettings(settings);
                        store.AddHistory(entry);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Backup worker loop error");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private bool ShouldRunNow(BackupSettings settings)
        {
            if (settings.LastRunDateOnly.HasValue && settings.LastRunDateOnly.Value.Date == DateTime.Today)
                return false; // already ran today

            if (!TimeSpan.TryParse(settings.Time, out var scheduledTime))
                return false;

            var now = DateTime.Now.TimeOfDay;
            if (now.Hours != scheduledTime.Hours || now.Minutes != scheduledTime.Minutes)
                return false;

            return settings.Frequency switch
            {
                "Daily" => true,
                "Weekly" => DateTime.Today.DayOfWeek == settings.DayOfWeek,
                "Monthly" => DateTime.Today.Day == Math.Min(settings.DayOfMonth, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)),
                _ => false
            };
        }
    }
}