using System;

namespace Township_API.Models
{
    public class BackupSettings
    {
        public bool Enabled { get; set; } = false;
        public string Frequency { get; set; } = "Daily"; // Daily | Weekly | Monthly
        public string Time { get; set; } = "00:00";       // 24hr "HH:mm"
        public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Sunday; // used if Weekly
        public int DayOfMonth { get; set; } = 1;                    // used if Monthly
        public string FolderPath { get; set; } = "";
        public int RetentionDays { get; set; } = 1;

        // DB connection - optional; falls back to appsettings default if empty
        public string? Server { get; set; }
        public string? Database { get; set; }
        public string? SqlUser { get; set; }
        public string? SqlPassword { get; set; }

        // Email - optional; skipped entirely if EmailEnabled = false
        public bool EmailEnabled { get; set; } = false;
        public string? FromEmail { get; set; }
        public string? AppPassword { get; set; }
        public string? ToEmail { get; set; }

        public DateTime? LastRunTime { get; set; }
        public string? LastRunStatus { get; set; }   // Success | Failed
        public DateTime? LastRunDateOnly { get; set; }
    }

    public class BackupHistoryEntry
    {
        public DateTime RunTime { get; set; }
        public bool Success { get; set; }
        public string Database { get; set; } = "";
        public string? FileName { get; set; }
        public double? SizeMB { get; set; }
        public string? ErrorMessage { get; set; }
        public string TriggerType { get; set; } = "Scheduled"; // Scheduled | Manual
    }

    public class BackupData
    {
        public BackupSettings Settings { get; set; } = new();
        public System.Collections.Generic.List<BackupHistoryEntry> History { get; set; } = new();
    }
}