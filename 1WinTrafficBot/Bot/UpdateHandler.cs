using _1WinTrafficBot.Bot;
using _1WinTrafficBot.Services;
using _1WinTrafficBot.Bot;
using _1WinTrafficBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace _1WinTrafficBot.Bot
{
    public class UpdateHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly TextService _textService;

        // Храним язык для каждого user_id
        private readonly Dictionary<long, string> _userLang = new();

        // Telegram менеджера (получает заявки)
        private readonly long _managerId = 123456789; // ← ПОМЕНЯЙ!

        public UpdateHandler(ITelegramBotClient bot, TextService textService)
        {
            _bot = bot;
            _textService = textService;
        }

        // Основной метод — вызывается каждый раз при обновлении
        public async Task HandleAsync(Update update)
        {
            if (update.Type == UpdateType.Message && update.Message!.Text != null)
            {
                await HandleMessage(update.Message);
            }
        }

        // Обработка текстовых сообщений
        private async Task HandleMessage(Message msg)
        {
            long userId = msg.Chat.Id;
            string text = msg.Text!;

            // Если язык ещё не выбран – ставим RU по умолчанию
            if (!_userLang.ContainsKey(userId))
                _userLang[userId] = "ru";

            string lang = _userLang[userId];

            // ОБРАБОТКА КНОПКИ СМЕНЫ ЯЗЫКА
            if (text == Translate("language", lang))
            {
                await _bot.SendMessage(
                    chatId: userId,
                    text: "Выберите язык:",
                    replyMarkup: Keyboard.LanguageMenu()
                );
                return;
            }

            // Пользователь выбрал язык — RU / UA / EN / AR
            if (IsLanguageCode(text))
            {
                lang = ConvertLanguage(text);
                _userLang[userId] = lang;

                await _bot.SendMessage(
                    chatId: userId,
                    text: GetStartMessage(lang),
                    replyMarkup: Keyboard.MainMenu(lang)
                );
                return;
            }

            // ОБРАБОТКА РАЗДЕЛОВ
            switch (text)
            {
                case var _ when text == Translate("about", lang):
                    await SendSection(userId, lang, "About");
                    break;

                case var _ when text == Translate("services", lang):
                    await SendSection(userId, lang, "Services");
                    break;

                case var _ when text == Translate("cases", lang):
                    await SendSection(userId, lang, "Cases");
                    break;

                case var _ when text == Translate("cooperation", lang):
                    await SendSection(userId, lang, "Cooperation");
                    break;

                case var _ when text == Translate("contact", lang):
                    await SendSection(userId, lang, "Contact");
                    break;

                case var _ when text == Translate("interested", lang):
                    await HandleInterest(msg);
                    break;

                case var _ when text == Translate("back", lang):
                    await _bot.SendMessage(
                        chatId: userId,
                        text: GetStartMessage(lang),
                        replyMarkup: Keyboard.MainMenu(lang)
                    );
                    break;

                default:
                    await _bot.SendMessage(
                        chatId: userId,
                        text: GetStartMessage(lang),
                        replyMarkup: Keyboard.MainMenu(lang)
                    );
                    break;
            }
        }

        // Отправить текст раздела
        private async Task SendSection(long chatId, string lang, string key)
        {
            var texts = _textService.GetTexts(lang);
            string message = key switch
            {
                "About" => texts.About,
                "Services" => texts.Services,
                "Cases" => texts.Cases,
                "Cooperation" => texts.Cooperation,
                "Contact" => texts.Contact,
                _ => "Unknown section"
            };

            await _bot.SendMessage(
                chatId: chatId,
                text: message,
                replyMarkup: Keyboard.SectionMenu(lang)
            );
        }

        // Обработка "Заинтересован"
        private async Task HandleInterest(Message msg)
        {
            long userId = msg.Chat.Id;
            string lang = _userLang[userId];

            // 1. Сообщение пользователю
            var texts = _textService.GetTexts(lang);

            await _bot.SendMessage(
                chatId: userId,
                text: texts.Interested,
                replyMarkup: Keyboard.MainMenu(lang)
            );

            // 2. Уведомление менеджеру
            string notify =
                $"🔥 НОВАЯ ЗАЯВКА\n" +
                $"Имя: {msg.Chat.FirstName}\n" +
                $"Username: @{msg.Chat.Username}\n" +
                $"ID: {msg.Chat.Id}\n" +
                $"Язык интерфейса: {lang}";

            await _bot.SendMessage(
                chatId: _managerId,
                text: notify
            );
        }

        // Проверка RU/UA/EN/AR
        private bool IsLanguageCode(string txt)
        {
            return txt == "RU" || txt == "UA" || txt == "EN" || txt == "AR";
        }

        // Конвертация RU → ru
        private string ConvertLanguage(string code)
        {
            return code switch
            {
                "RU" => "ru",
                "UA" => "ua",
                "EN" => "en",
                "AR" => "ar",
                _ => "ru"
            };
        }

        // Текст приветствия
        private string GetStartMessage(string lang)
        {
            return lang switch
            {
                "ru" => "Добро пожаловать! Выберите нужный раздел:",
                "ua" => "Вітаємо! Оберіть розділ:",
                "en" => "Welcome! Choose a section:",
                "ar" => "مرحبًا! اختر القسم:",
                _ => "Welcome!"
            };
        }

        // То же Translate что и в Keyboard.cs
        private string Translate(string key, string lang)
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
