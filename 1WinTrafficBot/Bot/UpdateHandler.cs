using _1WinTrafficBot.Bot;
using _1WinTrafficBot.Bot;
using _1WinTrafficBot.Models;
using _1WinTrafficBot.Services;
using _1WinTrafficBot.Services;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace _1WinTrafficBot.Bot
{
    public class UpdateHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly TextService _textService;
        private readonly Dictionary<long, AdminState> _adminState = new();
        private readonly Dictionary<long, AdminEditState> _adminEdits = new();



        private readonly HashSet<long> _admins = new()
        {
            559541816, // ← твой ID
            7296468013  // ← ID менеджера (если нужно)
        };

        // Храним язык для каждого user_id
        private readonly Dictionary<long, string> _userLang = new();

        // Telegram менеджера (получает заявки)
        private readonly long _managerId = 7296468013; // ← ПОМЕНЯЙ!

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

            // Обработка INLINE-кнопок (админ-панель)
            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallback(update.CallbackQuery!);
            }
        }

        private async Task HandleCallback(CallbackQuery query)
        {
            long uid = query.From.Id;

            // Проверяем прав администратора
            if (!_admins.Contains(uid))
            {
                await _bot.AnswerCallbackQuery(query.Id);
                return;
            }

            string data = query.Data ?? string.Empty;

            switch (data)
            {
                // === ГЛАВНОЕ МЕНЮ АДМИНА ===
                case "adm_edit":
                    await ShowLanguageMenu(uid);
                    break;

                case "adm_requests":
                    await ShowRequests(uid);
                    break;

                case "adm_reload":
                    _textService.ReloadAll();
                    await _bot.SendMessage(uid, "Тексты перезагружены ✔");
                    break;


                // === ВЫБОР ЯЗЫКА ===
                case "adm_lang_ru":
                    await ShowSectionMenu(uid, "ru");
                    break;

                case "adm_lang_ua":
                    await ShowSectionMenu(uid, "ua");
                    break;

                case "adm_lang_en":
                    await ShowSectionMenu(uid, "en");
                    break;

                case "adm_lang_ar":
                    await ShowSectionMenu(uid, "ar");
                    break;


                // === ВЫБОР РАЗДЕЛА ===
                case "adm_sec_about":
                    await StartEditSection(uid, "about");
                    break;

                case "adm_sec_services":
                    await StartEditSection(uid, "services");
                    break;

                case "adm_sec_cases":
                    await StartEditSection(uid, "cases");
                    break;

                case "adm_sec_cooperation":
                    await StartEditSection(uid, "cooperation");
                    break;

                case "adm_sec_contact":
                    await StartEditSection(uid, "contact");
                    break;

                case "adm_sec_interested":
                    await StartEditSection(uid, "interested");
                    break;

                default:
                    await _bot.SendMessage(uid, $"Неизвестная команда: {data}");
                    break;
            }

            // Обязательный ответ Telegram, чтобы убрать "часики"
            await _bot.AnswerCallbackQuery(query.Id);
        }

        //private async Task ShowSectionMenu(long chatId, string langCode)
        //{
        //    // сохраняем выбранный язык
        //    _adminState[chatId] = new AdminState
        //    {
        //        Step = "select_section",
        //        SelectedLanguage = langCode.Replace("adm_lang_", "")
        //    };

        //    var kb = new InlineKeyboardMarkup(new[]
        //    {
        //        new [] { InlineKeyboardButton.WithCallbackData("О нас", "adm_sec_about") },
        //        new [] { InlineKeyboardButton.WithCallbackData("Услуги / Цены", "adm_sec_services") },
        //        new [] { InlineKeyboardButton.WithCallbackData("Кейсы", "adm_sec_cases") },
        //        new [] { InlineKeyboardButton.WithCallbackData("Сотрудничество", "adm_sec_partners") },
        //        new [] { InlineKeyboardButton.WithCallbackData("Связаться с нами", "adm_sec_contact") },
        //        new [] { InlineKeyboardButton.WithCallbackData("Текст кнопки 'Заинтересован'", "adm_sec_interested") }
        //    });

        //    await _bot.SendMessage(
        //        chatId,
        //        $"Выберите раздел ({_adminState[chatId].SelectedLanguage}):",
        //        replyMarkup: kb
        //    );
        //}


        private async Task ShowRequests(long chatId)
        {
            string path = Path.Combine("Data", "requests.json");

            if (!File.Exists(path))
            {
                await _bot.SendMessage(chatId, "Заявок пока нет.");
                return;
            }

            string json = File.ReadAllText(path);

            await _bot.SendMessage(
                chatId,
                $"Заявки:\n\n{json.Substring(0, Math.Min(json.Length, 4000))}"
            );
        }


        private async Task ShowAdminMenu(long chatId)
        {
            var kb = new InlineKeyboardMarkup(new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData("📝 Редактировать тексты", "adm_edit") },
                new [] { InlineKeyboardButton.WithCallbackData("📂 Просмотр заявок", "adm_requests") },
                new [] { InlineKeyboardButton.WithCallbackData("🔄 Перезагрузить тексты", "adm_reload") }
            });

            await _bot.SendMessage(
                chatId,
                "Меню администратора:",
                replyMarkup: kb
            );
        }

        private async Task ShowLanguageMenu(long chatId)
        {
            var kb = new InlineKeyboardMarkup(new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData("🇷🇺 Русский", "adm_lang_ru") },
                new [] { InlineKeyboardButton.WithCallbackData("🇺🇦 Українська", "adm_lang_ua") },
                new [] { InlineKeyboardButton.WithCallbackData("🇬🇧 English", "adm_lang_en") },
                new [] { InlineKeyboardButton.WithCallbackData("🇦🇪 العربية", "adm_lang_ar") }
            });

            await _bot.SendMessage(chatId, "Выберите язык:", replyMarkup: kb);
        }

        private async Task ShowSectionMenu(long chatId, string langCode)
        {
            // Запоминаем, какой язык сейчас редактирует админ
            _adminEdits[chatId] = new AdminEditState
            {
                LanguageCode = langCode
            };

            var kb = new InlineKeyboardMarkup(new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData("О нас", "adm_sec_about") },
                new [] { InlineKeyboardButton.WithCallbackData("Услуги / Цены", "adm_sec_services") },
                new [] { InlineKeyboardButton.WithCallbackData("Кейсы", "adm_sec_cases") },
                new [] { InlineKeyboardButton.WithCallbackData("Сотрудничество", "adm_sec_cooperation") },
                new [] { InlineKeyboardButton.WithCallbackData("Связаться с нами", "adm_sec_contact") },
                new [] { InlineKeyboardButton.WithCallbackData("Текст \"Заинтересован\"", "adm_sec_interested") }
            });

            await _bot.SendMessage(
                chatId,
                $"Язык: {langCode.ToUpper()}\nВыберите раздел, который хотите изменить:",
                replyMarkup: kb
            );
        }

        private async Task StartEditSection(long chatId, string sectionKey)
        {
            if (!_adminEdits.TryGetValue(chatId, out var state))
            {
                // На всякий случай, если по какой-то причине состояния нет
                state = new AdminEditState { LanguageCode = "ru" };
                _adminEdits[chatId] = state;
            }

            state.SectionKey = sectionKey;

            var texts = _textService.GetTexts(state.LanguageCode);

            string current = sectionKey switch
            {
                "about" => texts.About,
                "services" => texts.Services,
                "cases" => texts.Cases,
                "cooperation" => texts.Cooperation,
                "contact" => texts.Contact,
                "interested" => texts.Interested,
                _ => ""
            };

            await _bot.SendMessage(
                chatId,
                $"Сейчас в разделе *{sectionKey}* (язык: {state.LanguageCode.ToUpper()}):\n\n" +
                $"{current}\n\n" +
                "👉 Отправьте мне НОВЫЙ текст одним сообщением.",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
            );
        }

        private async Task SaveSectionText(long chatId, string langCode, string sectionKey, string newText)
        {
            string path = Path.Combine("Data", $"texts.{langCode}.json");

            if (!File.Exists(path))
            {
                await _bot.SendMessage(chatId, $"Файл {path} не найден.");
                return;
            }

            string json = File.ReadAllText(path);
            SectionTexts? data = JsonSerializer.Deserialize<SectionTexts>(json);

            if (data == null)
            {
                await _bot.SendMessage(chatId, "Не удалось прочитать JSON.");
                return;
            }

            switch (sectionKey)
            {
                case "about":
                    data.About = newText;
                    break;
                case "services":
                    data.Services = newText;
                    break;
                case "cases":
                    data.Cases = newText;
                    break;
                case "cooperation":
                    data.Cooperation = newText;
                    break;
                case "contact":
                    data.Contact = newText;
                    break;
                case "interested":
                    data.Interested = newText;
                    break;
            }

            string newJson = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions { WriteIndented = true }
            );

            File.WriteAllText(path, newJson);

            // Перезагрузить тексты в работающем боте
            _textService.ReloadAll();

            await _bot.SendMessage(
                chatId,
                $"Текст раздела *{sectionKey}* для языка {langCode.ToUpper()} успешно обновлён ✅",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
            );
        }

        // Обработка текстовых сообщений
        private async Task HandleMessage(Message msg)
        {
            long userId = msg.Chat.Id;
            string text = msg.Text!;

            // Если это админ и он сейчас редактирует раздел — воспринимаем сообщение как новый текст
            if (_admins.Contains(userId) && _adminEdits.TryGetValue(userId, out var editState) && !string.IsNullOrEmpty(editState.SectionKey))
            {
                await SaveSectionText(userId, editState.LanguageCode, editState.SectionKey, text);
                _adminEdits.Remove(userId); // Сбросить состояние
                return;
            }

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

            // Команда /admin (вход в админ-панель)
            if (text == "/admin")
            {
                // Проверяем — админ ли этот пользователь
                if (!_admins.Contains(msg.Chat.Id))
                {
                    await _bot.SendMessage(msg.Chat.Id, "У вас нет доступа.");
                    return;
                }

                // Показываем меню администратора
                await ShowAdminMenu(msg.Chat.Id);
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

        private void SaveRequest(RequestInfo req)
        {
            string path = Path.Combine("Data", "requests.json");

            List<RequestInfo> list;

            // Если файла нет → создаём новый список
            if (!File.Exists(path))
            {
                list = new List<RequestInfo>();
            }
            else
            {
                string json = File.ReadAllText(path);

                try
                {
                    list = JsonSerializer.Deserialize<List<RequestInfo>>(json) ??
                           new List<RequestInfo>();
                }
                catch
                {
                    list = new List<RequestInfo>();
                }
            }

            // Добавляем новую заявку в список
            list.Add(req);

            // Сохраняем обратно в файл
            File.WriteAllText(
                path,
                JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true })
            );
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

            var texts = _textService.GetTexts(lang);

            // 1) Сообщение пользователю
            await _bot.SendMessage(
                chatId: userId,
                text: texts.Interested,
                replyMarkup: Keyboard.MainMenu(lang)
            );

            // 2) Фиксация интереса (в будущем можно записывать в JSON/БД)
            Console.WriteLine($"[INTEREST] User {msg.Chat.Id} ({msg.Chat.Username}) is interested.");

            var req = new RequestInfo
            {
                UserId = msg.Chat.Id,
                Username = msg.Chat.Username,
                FirstName = msg.Chat.FirstName,
                Language = lang,
                Date = DateTime.UtcNow
            };

            SaveRequest(req);

            // 3) Уведомление менеджеру
            string managerNotify =
                $"🔥 *НОВАЯ ЗАЯВКА*\n\n" +
                $"👤 Имя: _{msg.Chat.FirstName}_\n" +
                $"🔗 Username: @{msg.Chat.Username}\n" +
                $"🆔 Telegram ID: `{msg.Chat.Id}`\n" +
                $"🌐 Язык интерфейса: *{lang}*\n\n" +
                $"Пользователь нажал кнопку \"Заинтересован\".";

            await _bot.SendMessage(
                chatId: _managerId,
                text: managerNotify,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
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
