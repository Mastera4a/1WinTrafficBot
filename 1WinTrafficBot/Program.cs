using _1WinTrafficBot.Bot;
using _1WinTrafficBot.Services;

namespace _1WinTrafficBot
{

    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== 1WIN TRAFFIC BOT ===");

            string token = "8255765312:AAHYM-VXe1Jyfc8Dlkkjavox33YKA4Gt604";

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("❌ Ошибка: токен пустой!");
                return;
            }

            // 🔹 Загружаем тексты
            var textService = new TextService();

            // 🔹 Запускаем бота
            var botService = new BotService(token, textService);
            botService.Start();

            Console.WriteLine("Бот работает. Нажмите Enter для выхода…");
            Console.ReadLine();
        }
    }
}
