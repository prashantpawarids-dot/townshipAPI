using Microsoft.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using Township_API.Models;

namespace Township_API.Services
{
    public interface IBackupService
    {
        Task<BackupHistoryEntry> RunBackupAsync(BackupSettings settings, string triggerType);
    }

    public class BackupService : IBackupService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<BackupService> _logger;

        public BackupService(IConfiguration config, ILogger<BackupService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<BackupHistoryEntry> RunBackupAsync(BackupSettings settings, string triggerType)
        {
            var entry = new BackupHistoryEntry { RunTime = DateTime.Now, TriggerType = triggerType };

            try
            {
                //string server = settings.Server ?? "";
                //string database = settings.Database ?? "";
                //bool useDefaultAuth = string.IsNullOrWhiteSpace(settings.SqlUser);

                //if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database))
                //{
                //    var defaultConnString = _config.GetConnectionString("DefaultConnection");
                //    if (!string.IsNullOrWhiteSpace(defaultConnString))
                //    {
                //        var defaultBuilder = new SqlConnectionStringBuilder(defaultConnString);
                //        if (string.IsNullOrWhiteSpace(server)) server = defaultBuilder.DataSource;
                //        if (string.IsNullOrWhiteSpace(database)) database = defaultBuilder.InitialCatalog;
                //    }
                //}

                string server = settings.Server ?? "";
                string database = settings.Database ?? "";
                string sqlUser = settings.SqlUser ?? "";
                string sqlPassword = settings.SqlPassword ?? "";

                // Fall back to DefaultConnection for anything not set in backup settings
                var defaultConnString = _config.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrWhiteSpace(defaultConnString))
                {
                    var d = new SqlConnectionStringBuilder(defaultConnString);
                    if (string.IsNullOrWhiteSpace(server)) server = d.DataSource;
                    if (string.IsNullOrWhiteSpace(database)) database = d.InitialCatalog;
                    if (string.IsNullOrWhiteSpace(sqlUser) && !d.IntegratedSecurity)
                    {
                        sqlUser = d.UserID;
                        sqlPassword = d.Password;
                    }
                }

                if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database))
                    throw new InvalidOperationException("Server/Database not configured.");
                if (string.IsNullOrWhiteSpace(settings.FolderPath))
                    throw new InvalidOperationException("Backup folder not selected.");

                Directory.CreateDirectory(settings.FolderPath);

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = $"{database}_{timestamp}.bak";
                var fullPath = Path.Combine(settings.FolderPath, fileName);

                //string connString = useDefaultAuth
                //    ? $"Server={server};Database=master;Integrated Security=True;TrustServerCertificate=True;"
                ////    : $"Server={server};Database=master;User Id={settings.SqlUser};Password={settings.SqlPassword};TrustServerCertificate=True;";

                //using (var conn = new SqlConnection(connString))

                var csb = new SqlConnectionStringBuilder
                {
                    DataSource = server,
                    InitialCatalog = "master",
                    TrustServerCertificate = true,
                    Encrypt = false
                };
                if (string.IsNullOrWhiteSpace(sqlUser))
                {
                    csb.IntegratedSecurity = true;
                }
                else
                {
                    csb.UserID = sqlUser;
                    csb.Password = sqlPassword;
                }

                using (var conn = new SqlConnection(csb.ConnectionString))
                {
                    await conn.OpenAsync();
                    //var sql = $"BACKUP DATABASE [{database}] TO DISK = N'{fullPath}' WITH INIT";
                    var sql = $"BACKUP DATABASE [{database}] TO DISK = N'{fullPath.Replace("'", "''")}' WITH INIT";
                    using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 0 }; // no timeout — large DBs take minutes
                    await cmd.ExecuteNonQueryAsync();
                }

                if (!File.Exists(fullPath))
                    throw new Exception("Backup file was not created.");

                entry.Success = true;
                entry.Database = database;
                entry.FileName = fileName;
                entry.SizeMB = Math.Round(new FileInfo(fullPath).Length / 1024.0 / 1024.0, 2);

                CleanupOldBackups(settings.FolderPath, database, settings.RetentionDays);

                if (settings.EmailEnabled)
                    await SendEmailAsync(settings, entry);

                return entry;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Backup failed");
                entry.Success = false;
                entry.ErrorMessage = ex.Message;

                if (settings.EmailEnabled)
                {
                    try { await SendEmailAsync(settings, entry); } catch { }
                }
                return entry;
            }
        }

        private void CleanupOldBackups(string folder, string database, int retentionDays)
        {
            var cutoff = DateTime.Now.AddDays(-retentionDays);
            foreach (var f in Directory.GetFiles(folder, $"{database}_*.bak"))
            {
                if (File.GetLastWriteTime(f) < cutoff)
                {
                    try { File.Delete(f); } catch { }
                }
            }
        }

        private async Task SendEmailAsync(BackupSettings settings, BackupHistoryEntry entry)
        {
            if (string.IsNullOrWhiteSpace(settings.FromEmail) || string.IsNullOrWhiteSpace(settings.ToEmail) || string.IsNullOrWhiteSpace(settings.AppPassword))
                return;

            var status = entry.Success ? "SUCCESS" : "FAILED";
            var color = entry.Success ? "#22c55e" : "#ef4444";
            var detailsRow = entry.Success
                ? $"<tr><td>File</td><td>{entry.FileName}</td></tr><tr><td>Size</td><td>{entry.SizeMB} MB</td></tr>"
                : $"<tr><td>Error</td><td style='color:#ef4444;'>{entry.ErrorMessage}</td></tr>";

            var html = $@"
            <html><body style='font-family:Segoe UI,Arial,sans-serif;background:#f4f4f7;padding:30px;'>
            <div style='max-width:500px;margin:auto;background:#fff;border-radius:10px;overflow:hidden;'>
              <div style='background:{color};padding:20px;'><h2 style='color:#fff;margin:0;'>Database Backup {status}</h2></div>
              <div style='padding:20px;'>
                <table style='width:100%;font-size:14px;'>
                  <tr><td>Database</td><td>{entry.Database}</td></tr>
                  {detailsRow}
                  <tr><td>Time</td><td>{entry.RunTime:dd-MMM-yyyy hh:mm tt}</td></tr>
                  <tr><td>Trigger</td><td>{entry.TriggerType}</td></tr>
                </table>
              </div>
            </div></body></html>";

            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(settings.FromEmail, settings.AppPassword),
                EnableSsl = true
            };
            var mail = new MailMessage(settings.FromEmail, settings.ToEmail, $"SQL Backup {status} - {entry.RunTime:yyyyMMdd_HHmmss}", html) { IsBodyHtml = true };
            await client.SendMailAsync(mail);
        }
    }
}