using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TeleGramBot_Scheduler.Data;
using TeleGramBot_Scheduler.Sessions;
using TeleGramBot_Scheduler.UpdateProcessors;

namespace TeleGramBot_Scheduler
{
    public class BotProcessMenager
    {
        private readonly TelegramBotClient _botClient;
        private readonly IRepository<DataMessage> _messageRepository;
        private List<IUpdateProcessor> updateProcessors = new List<IUpdateProcessor>
        {
            new TextMessageUpdateProcessor(),
            new CallbackQueryProcessor(),
            new DateTimeUpdateProcessor(),
            new IdProcessor()
        };
        private SessionProcessor sessionProcessor;

        
        public BotProcessMenager(TelegramBotClient botClient)
        {
            _botClient = botClient;
            _messageRepository = new MessageRepository();
            sessionProcessor = new SessionProcessor();
        }

        public void Start()
        {
            while (true)
            {
                var updates = _botClient.GetUpdatesAsync(BotSettings.MessageOffset).Result;
                foreach (var update in updates)
                {
                   // ShowMessageIfItsDateTimeToRemindIsNow(update);
                    if (update.Type == UpdateType.Message && update.Message.Text == "show")
                    {
                        sessionProcessor.IsSessionOpen = true;
                        ShowMenu(update);
                    }

                    var applicableUpdateProcessors = updateProcessors.Where(up => up.IsApplicable(update));

                    foreach (var updateProcessor in applicableUpdateProcessors)
                    {
                        updateProcessor.Apply(update, _botClient, sessionProcessor);
                    }

                    /*if (!sessionProcessor.IsSessionOpen)
                    {
                        ShowMenu(update);
                        ChangeOffset(updates);
                        sessionProcessor.IsSessionOpen = true;
                        continue;
                    }
                    После закрытия сессии новый update не приходит и получаем исключение NullRefEx
                     */
                    ChangeOffset(updates);
                }
            }
        }

        private void ShowMessageIfItsDateTimeToRemindIsNow(Update update)
        {
            var allmessages = new List<DataMessage>();
            allmessages = _messageRepository.GetAll().ToList();

            var actualmessages = allmessages.Where(l => l.TimeToRemind == DateTime.Now && l.IsActive == true);
            if (actualmessages != null)
            {
                var listMessageText = $"Сегодня Вы просили напомнить:\n";

                foreach (var actualmessage in actualmessages)
                {
                    listMessageText += $"\n{actualmessage.TimeToRemind.Date.ToString("dd/MM/yyyy H:mm")}, Id {actualmessage.Id}: {actualmessage.MessageText}\n";
                }

                var sentMessage = _botClient
                                .SendTextMessageAsync(update.Message.Chat.Id, $"{listMessageText}\n")
                                .Result;
            }
        }

        private void ShowMenu(Update update)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[] 
            {
                new[]{
                    InlineKeyboardButton.WithCallbackData("Показать все напоминания"),
                    InlineKeyboardButton.WithCallbackData("Добавить новое")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Изменить"),
                    InlineKeyboardButton.WithCallbackData("Удалить"),
                    InlineKeyboardButton.WithCallbackData("Выполнено")
                }
            });
            var sentMessage = _botClient
                .SendTextMessageAsync(update.Message.Chat.Id, $"Выберите пункт меню\n", replyMarkup: inlineKeyboard)
                .Result;
        }

        private static void ChangeOffset(Update[] updates)
        {
            var lastUpdate = updates.LastOrDefault();
            if (lastUpdate != null)
                BotSettings.MessageOffset = lastUpdate.Id + 1;
        }
    }
}
