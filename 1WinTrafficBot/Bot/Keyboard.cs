using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace _1WinTrafficBot.Bot
{
    public static class Keyboard
    {
        // Главная клавиатура меню
        public static ReplyKeyboardMarkup MainMenuKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "Заинтересован", "О нас" },
                new KeyboardButton[] { "Услуги / Цены", "Кейсы" },
                new KeyboardButton[] { "Сотрудничество", "Связаться с нами" },
                new KeyboardButton[] { "Сменить язык" }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }

        // Клавиатура для кнопки "Заинтересован"
        public static ReplyKeyboardMarkup InterestedKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "Отправить контакт", "Назад" }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }

        // Клавиатура для выбора языка
        public static ReplyKeyboardMarkup LanguageKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "Русский", "Українська" },
                new KeyboardButton[] { "English", "عربي" },
                new KeyboardButton[] { "Назад" }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
    }
}
