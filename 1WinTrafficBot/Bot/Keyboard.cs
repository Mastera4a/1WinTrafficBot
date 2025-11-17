using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace _1WinTrafficBot.Bot
{
    public static class Keyboard
    {
        // Главное меню — 7 кнопок
        public static ReplyKeyboardMarkup MainMenu(string lang)
        {
            return new ReplyKeyboardMarkup(new[]
            {
            new KeyboardButton[] { Translate("interested", lang) },
            new KeyboardButton[] { Translate("about", lang) },
            new KeyboardButton[] { Translate("services", lang) },
            new KeyboardButton[] { Translate("cases", lang) },
            new KeyboardButton[] { Translate("cooperation", lang) },
            new KeyboardButton[] { Translate("contact", lang) },
            new KeyboardButton[] { Translate("language", lang) }
        })
            {
                ResizeKeyboard = true
            };
        }

        // В любом разделе — кнопка "Заинтересован"
        public static ReplyKeyboardMarkup SectionMenu(string lang)
        {
            return new ReplyKeyboardMarkup(new[]
            {
            new KeyboardButton[] { Translate("interested", lang) },
            new KeyboardButton[] { Translate("back", lang) }
        })
            {
                ResizeKeyboard = true
            };
        }

        // Меню выбора языка
        public static ReplyKeyboardMarkup LanguageMenu()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                    new KeyboardButton[] { "🇷🇺 RU", "🇺🇦 UA" },
                    new KeyboardButton[] { "🇬🇧 EN", "🇦🇪 AR" }
            })
            {
                ResizeKeyboard = true
            };
        }
        //public static ReplyKeyboardMarkup LanguageMenu()
        //{
        //    return new ReplyKeyboardMarkup(new[]
        //    {
        //    new KeyboardButton[] { "RU", "UA" },
        //    new KeyboardButton[] { "EN", "AR" }
        //})
        //    {
        //        ResizeKeyboard = true
        //    };
        //}

        // Простая локализация кнопок
        private static string Translate(string key, string lang)
        {
            return key switch
            {
                "interested" => lang switch
                {
                    "ru" => "Заинтересован",
                    "ua" => "Зацікавлений",
                    "en" => "Interested",
                    "ar" => "مهتم",
                    _ => "Interested"
                },

                "about" => lang switch
                {
                    "ru" => "О нас",
                    "ua" => "Про нас",
                    "en" => "About us",
                    "ar" => "معلومات عنا",
                    _ => "About us"
                },

                "services" => lang switch
                {
                    "ru" => "Услуги / Цены",
                    "ua" => "Послуги / Ціни",
                    "en" => "Services / Prices",
                    "ar" => "الخدمات / الأسعار",
                    _ => "Services"
                },

                "cases" => lang switch
                {
                    "ru" => "Кейсы",
                    "ua" => "Кейси",
                    "en" => "Cases",
                    "ar" => "الأمثلة",
                    _ => "Cases"
                },

                "cooperation" => lang switch
                {
                    "ru" => "Сотрудничество",
                    "ua" => "Співпраця",
                    "en" => "Cooperation",
                    "ar" => "التعاون",
                    _ => "Cooperation"
                },

                "contact" => lang switch
                {
                    "ru" => "Связаться с нами",
                    "ua" => "Звʼязатися з нами",
                    "en" => "Contact us",
                    "ar" => "اتصل بنا",
                    _ => "Contact us"
                },

                "language" => lang switch
                {
                    "ru" => "Сменить язык",
                    "ua" => "Змінити мову",
                    "en" => "Change language",
                    "ar" => "تغيير اللغة",
                    _ => "Language"
                },

                "back" => lang switch
                {
                    "ru" => "⬅ Назад",
                    "ua" => "⬅ Назад",
                    "en" => "⬅ Back",
                    "ar" => "⬅ رجوع",
                    _ => "⬅ Back"
                },

                _ => key
            };
        }
    }
}
