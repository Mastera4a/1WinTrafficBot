using System;
using System.Collections.Generic;
using System.Text;

namespace _1WinTrafficBot.Models
{
    public class AdminEditState
    {
        // ru / ua / en / ar
        public string LanguageCode { get; set; } = "ru";

        // about / services / cases / cooperation / contact / interested
        public string SectionKey { get; set; } = string.Empty;
    }
}
