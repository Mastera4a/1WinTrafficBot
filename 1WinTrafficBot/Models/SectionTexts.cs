using System;
using System.Collections.Generic;
using System.Text;

namespace _1WinTrafficBot.Models
{
    public class SectionTexts
    {
        public string About { get; set; } = string.Empty;          // Раздел "О нас"
        public string Services { get; set; } = string.Empty;       // Раздел "Услуги / Цены"
        public string Cases { get; set; } = string.Empty;          // Раздел "Кейсы"
        public string Cooperation { get; set; } = string.Empty;    // Раздел "Сотрудничество"
        public string Contact { get; set; } = string.Empty;        // Раздел "Связаться с нами"

        public string Interested { get; set; }
    }
}
