using System;
using System.Collections.Generic;
using System.Text;

namespace _1WinTrafficBot.Models
{
    public class RequestInfo
    {
        public long UserId { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string Language { get; set; } = "ru";
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
