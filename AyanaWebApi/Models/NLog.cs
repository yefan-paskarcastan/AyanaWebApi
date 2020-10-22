using System;

namespace AyanaWebApi.Models
{
    public class NLog
    {
        public int Id { get; set; }
        public string CallSite { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string MachineName { get; set; }
        public string StackTrace { get; set; }
        public string Thread { get; set; }
        public string Username { get; set; }
    }
}
