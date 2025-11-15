using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace _1WinTrafficBot
{
     // Model for storing localized content
    public class LocalizedContent
    {
        public string LanguageCode { get; set; } = "ru";
        public Dictionary<string, string> Sections { get; set; } = new();
    }

    internal class Program
    {
        private static TelegramBotClient bot;

        static async Task Main(string[] args)
        {
            string token = "8255765312:AAHYM-VXe1Jyfc8Dlkkjavox33YKA4Gt604";

            bot = new TelegramBotClient(token);

            // Проверяем бота
            var me = await bot.GetMe();
            Console.WriteLine($"Bot started: @{me.Username}");

            // Слушаем апдейты вручную
            while (true)
            {
                var updates = await bot.GetUpdates(offset: _offset, timeout: 20);
                foreach (var upd in updates)
                {
                    _offset = upd.Id + 1;
                    await HandleUpdate(upd);
                }
            }
        }

        private static int _offset = 0;

        private static async Task HandleUpdate(Update update)
        {
            if (update.Type != UpdateType.Message)
                return;

            var msg = update.Message;
            if (msg.Text == null) return;

            Console.WriteLine($"User: {msg.Chat.Id} → {msg.Text}");

            if (msg.Text == "/start")
            {
                await bot.SendMessage(
                    chatId: msg.Chat.Id,
                    text: "Привет! Бот работает!"
                );
            }
            else
            {
                await bot.SendMessage(
                    chatId: msg.Chat.Id,
                    text: $"Ты написал: {msg.Text}"
                );
            }
        }
    }
}
