using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using _1WinTrafficBot.Models;

namespace _1WinTrafficBot.Services
{
    public class TextService
    {
        private readonly Dictionary<string, SectionTexts> _texts = new();

        public TextService()
        {
            LoadAllLanguages();
        }

        private void LoadAllLanguages()
        {
            LoadLanguage("ru");
            LoadLanguage("ua");
            LoadLanguage("en");
            LoadLanguage("ar");
        }

        private void LoadLanguage(string lang)
        {
            string path = Path.Combine("Data", $"texts.{lang}.json");

            if (!File.Exists(path))
            {
                Console.WriteLine($"[WARNING] Text file not found: {path}");
                return;
            }

            string json = File.ReadAllText(path);

            SectionTexts? data = JsonSerializer.Deserialize<SectionTexts>(json);

            if (data != null)
            {
                _texts[lang] = data;
                Console.WriteLine($"[INFO] Loaded language: {lang}");
            }
        }

        public SectionTexts GetTexts(string lang)
        {
            if (_texts.ContainsKey(lang))
                return _texts[lang];

            // fallback — если язык не найден, используем русский
            return _texts["ru"];
        }

        public void ReloadAll()
        {
            _texts.Clear();
            LoadAllLanguages();
        }
    }
}
