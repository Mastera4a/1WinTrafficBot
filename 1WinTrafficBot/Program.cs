using _1WinTrafficBot.Bot;
using _1WinTrafficBot.Services;
using Microsoft.AspNetCore.Builder;
using Telegram.Bot;

namespace _1WinTrafficBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            app.Urls.Add($"http://0.0.0.0:{port}");

            app.MapGet("/", () => "Bot is running!");

            app.RunAsync();


            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== 1WIN TRAFFIC BOT ===");

            string token = Environment.GetEnvironmentVariable("BOT_TOKEN");
            //string token = "8555782174:AAH5NPj2UDuhH0oE02nC6ie4c_BEjFTx-2U";

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("❌ Ошибка: токен пустой!");
                return;
            }

            var textService = new TextService();
            var botService = new BotService(token, textService);
            botService.Start();

            Console.WriteLine("Бот работает");

            await Task.Delay(-1);
        }
    }
}
