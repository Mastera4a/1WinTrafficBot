using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;

namespace _1WinTrafficBot.Bot
{
    public class UpdateHandler
    {
        private readonly ITelegramBotClient _botClient; // Сюда мы помещаем объект бота
        private readonly string _managerUsername = "@ofm_1win"; // Ник менеджера для уведомлений

        // Конструктор класса
        public UpdateHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        // Основной метод, который вызывается при любом обновлении (сообщении)
        public async Task HandleUpdateAsync(Update update)
        {
            // Проверяем, что обновление — это сообщение от пользователя
            if (update.Type != UpdateType.Message) return;
            var message = update.Message;

            // Проверяем, что сообщение текстовое
            if (message.Type != MessageType.Text) return;

            string text = message.Text; // Сохраняем текст пользователя в переменную

            // Обработка команд/кнопок
            switch (text)
            {
                case "О нас":
                    await _botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "1WIN TRAFFIC — агентство, которое даёт OnlyFans-моделям реальный доход...",
                        replyMarkup: Keyboard.MainMenuKeyboard() // Показываем главное меню
                    );
                    break;

                case "Услуги / Цены":
                    await _botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Мы закрываем весь трафик под ключ...\nСтоимость ведения — 1100$/мес",
                        replyMarkup: Keyboard.MainMenuKeyboard()
                    );
                    break;

                case "Кейсы":
                    await _botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Наши кейсы — рост подписчиков в 4–8 раз, увеличение дохода в 2–5 раз...",
                        replyMarkup: Keyboard.MainMenuKeyboard()
                    );
                    break;

                case "Сотрудничество":
                    await _botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Работать с нами — значит расти. Преимущества: быстрый запуск, прозрачная работа, поддержка 24/7...",
                        replyMarkup: Keyboard.MainMenuKeyboard()
                    );
                    break;

                case "Связаться с нами":
                    await _botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Напиши нам — Telegram менеджера: @ofm_1win",
                        replyMarkup: Keyboard.MainMenuKeyboard()
                    );
                    break;

                case "Заинтересован":
                    // Отправляем пользователю кнопку для уведомления менеджера
                    await _botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Нажмите 'Отправить контакт', чтобы уведомить менеджера.",
                        replyMarkup: Keyboard.InterestedKeyboard()
                    );
                    break;

                case "Отправить контакт":
                    // Сообщение менеджеру
                    string managerMessage = $"Пользователь заинтересован!\n" +
                                            $"Имя: {message.From.FirstName}\n" +
                                            $"Username: @{message.From.Username}\n" +
                                            $"ID: {message.From.Id}";
                    await _botClient.SendMessage(
                        chatId: _managerUsername,
                        text: managerMessage
                    );

                    // Подтверждение пользователю
                    await _botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Менеджер получил ваш сигнал. Спасибо!",
                        replyMarkup: Keyboard.MainMenuKeyboard()
                    );
                    break;

                case "Сменить язык":
                    await _botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Выберите язык:",
                        replyMarkup: Keyboard.LanguageKeyboard()
                    );
                    break;

                case "Русский":
                case "Українська":
                case "English":
                case "عربي":
                    // Здесь можно добавить переключение языков (на будущее)
                    await _botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: $"Вы выбрали язык: {text}",
                        replyMarkup: Keyboard.MainMenuKeyboard()
                    );
                    break;

                case "Назад":
                    await _botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Возврат в главное меню",
                        replyMarkup: Keyboard.MainMenuKeyboard()
                    );
                    break;

                default:
                    // Если пользователь пишет что-то неожиданное
                    await _botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Пожалуйста, выберите пункт меню.",
                        replyMarkup: Keyboard.MainMenuKeyboard()
                    );
                    break;
            }
        }
    }
}
