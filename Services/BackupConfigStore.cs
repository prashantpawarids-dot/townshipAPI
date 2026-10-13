using Microsoft.Data.SqlClient;
using Township_API.Models;

namespace Township_API.Services
{
    public interface IBackupConfigStore
    {
        BackupSettings GetSettings();
        void SaveSettings(BackupSettings settings);
        List<BackupHistoryEntry> GetHistory(int take = 30);
        void AddHistory(BackupHistoryEntry entry);
    }

    public class BackupConfigStore : IBackupConfigStore
    {
        private readonly string _connString;

        public BackupConfigStore(IConfiguration config)
        {
            // ⚠️ "DefaultConnection" ko apne appsettings.json mein jo key hai usse match karo
            _connString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
        }

        public BackupSettings GetSettings()
        {
            using var conn = new SqlConnection(_connString);
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM BackupSettings WHERE Id = 1", conn);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return new BackupSettings();

            return new BackupSettings
            {
                Enabled = reader.GetBoolean(reader.GetOrdinal("Enabled")),
                Frequency = reader["Frequency"]?.ToString() ?? "Daily",
                Time = reader["Time"]?.ToString() ?? "00:00",
                DayOfWeek = (DayOfWeek)Convert.ToInt32(reader["DayOfWeek"]),
                DayOfMonth = Convert.ToInt32(reader["DayOfMonth"]),
                FolderPath = reader["FolderPath"] as string ?? "",
                RetentionDays = Convert.ToInt32(reader["RetentionDays"]),
                Server = reader["Server"] as string,
                Database = reader["Database"] as string,
                SqlUser = reader["SqlUser"] as string,
                SqlPassword = reader["SqlPassword"] as string,
                EmailEnabled = reader.GetBoolean(reader.GetOrdinal("EmailEnabled")),
                FromEmail = reader["FromEmail"] as string,
                AppPassword = reader["AppPassword"] as string,
                ToEmail = reader["ToEmail"] as string,
                LastRunTime = reader["LastRunTime"] as DateTime?,
                LastRunStatus = reader["LastRunStatus"] as string,
                LastRunDateOnly = reader["LastRunDateOnly"] as DateTime?
            };
        }

        public void SaveSettings(BackupSettings s)
        {
            using var conn = new SqlConnection(_connString);
            conn.Open();
            var cmd = new SqlCommand(@"
                UPDATE BackupSettings SET
                    Enabled = @Enabled, Frequency = @Frequency, [Time] = @Time,
                    DayOfWeek = @DayOfWeek, DayOfMonth = @DayOfMonth, FolderPath = @FolderPath,
                    RetentionDays = @RetentionDays, Server = @Server, [Database] = @Database,
                    SqlUser = @SqlUser, SqlPassword = @SqlPassword, EmailEnabled = @EmailEnabled,
                    FromEmail = @FromEmail, AppPassword = @AppPassword, ToEmail = @ToEmail,
                    LastRunTime = @LastRunTime, LastRunStatus = @LastRunStatus, LastRunDateOnly = @LastRunDateOnly
                WHERE Id = 1", conn);

            cmd.Parameters.AddWithValue("@Enabled", s.Enabled);
            cmd.Parameters.AddWithValue("@Frequency", s.Frequency);
            cmd.Parameters.AddWithValue("@Time", s.Time);
            cmd.Parameters.AddWithValue("@DayOfWeek", (int)s.DayOfWeek);
            cmd.Parameters.AddWithValue("@DayOfMonth", s.DayOfMonth);
            cmd.Parameters.AddWithValue("@FolderPath", (object?)s.FolderPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RetentionDays", s.RetentionDays);
            cmd.Parameters.AddWithValue("@Server", (object?)s.Server ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Database", (object?)s.Database ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SqlUser", (object?)s.SqlUser ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SqlPassword", (object?)s.SqlPassword ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmailEnabled", s.EmailEnabled);
            cmd.Parameters.AddWithValue("@FromEmail", (object?)s.FromEmail ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AppPassword", (object?)s.AppPassword ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ToEmail", (object?)s.ToEmail ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LastRunTime", (object?)s.LastRunTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LastRunStatus", (object?)s.LastRunStatus ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LastRunDateOnly", (object?)s.LastRunDateOnly ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public List<BackupHistoryEntry> GetHistory(int take = 30)
        {
            var list = new List<BackupHistoryEntry>();
            using var conn = new SqlConnection(_connString);
            conn.Open();
            var cmd = new SqlCommand($"SELECT TOP {take} * FROM BackupHistory ORDER BY RunTime DESC", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new BackupHistoryEntry
                {
                    RunTime = Convert.ToDateTime(reader["RunTime"]),
                    Success = reader.GetBoolean(reader.GetOrdinal("Success")),
                    Database = reader["Database"] as string ?? "",
                    FileName = reader["FileName"] as string,
                    SizeMB = reader["SizeMB"] as double?,
                    ErrorMessage = reader["ErrorMessage"] as string,
                    TriggerType = reader["TriggerType"] as string ?? "Scheduled"
                });
            }
            return list;
        }

        public void AddHistory(BackupHistoryEntry entry)
        {
            using var conn = new SqlConnection(_connString);
            conn.Open();
            var cmd = new SqlCommand(@"
                INSERT INTO BackupHistory (RunTime, Success, [Database], FileName, SizeMB, ErrorMessage, TriggerType)
                VALUES (@RunTime, @Success, @Database, @FileName, @SizeMB, @ErrorMessage, @TriggerType)", conn);

            cmd.Parameters.AddWithValue("@RunTime", entry.RunTime);
            cmd.Parameters.AddWithValue("@Success", entry.Success);
            cmd.Parameters.AddWithValue("@Database", (object?)entry.Database ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FileName", (object?)entry.FileName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SizeMB", (object?)entry.SizeMB ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ErrorMessage", (object?)entry.ErrorMessage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TriggerType", entry.TriggerType);

            cmd.ExecuteNonQuery();
        }
    }
}