using _1WinTrafficBot.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace _1WinTrafficBot.Bot
{
    public class BotService
    {
        private readonly ITelegramBotClient _bot;
        private readonly UpdateHandler _updateHandler;

        public BotService(string token, TextService textService)
        {
            _bot = new TelegramBotClient(token);
            _updateHandler = new UpdateHandler(_bot, textService);
        }

        public void Start()
        {
            Console.WriteLine("[BOT] Запускаем бота…");

            var cancellation = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // получать все типы обновлений
            };

            _bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellation.Token
            );

            Console.WriteLine("[BOT] Бот успешно запущен и слушает сообщения!");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken token)
        {
            await _updateHandler.HandleAsync(update);
        }

        private Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
        {
            Console.WriteLine($"[BOT ERROR] {ex.Message}");
            return Task.CompletedTask;
        }
    }
}
