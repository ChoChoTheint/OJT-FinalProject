namespace finalproject.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("event_log")]
    public class EventLog
    {
        public int log_type { get; set; } // '0 = info, 1 = Warning, 2 = Error, 3 = Insert, 4 = Update, 5 = Delete',
        public DateTime log_datetime { get; set; }
        public string? log_message { get; set; }
        public string? error_message { get; set; }
        public required string form_name { get; set; }
        public required string source { get; set; }
    }

    public enum LogType
    {
        Info = 0,
        Warning = 1,
        Error = 2,
        Insert = 3,
        Update = 4,
        Delete = 5
    }
}
