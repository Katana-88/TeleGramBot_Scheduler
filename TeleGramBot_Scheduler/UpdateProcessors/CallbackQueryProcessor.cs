using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TeleGramBot_Scheduler.Data;

namespace TeleGramBot_Scheduler.UpdateProcessors
{
    public class CallbackQueryProcessor : IUpdateProcessor
    {
        private readonly IRepository<DataMessage> _messageRepository;
        public bool IsApplicable(Update update)
            => update.Type == UpdateType.CallbackQuery;

        public CallbackQueryProcessor()
        {
            _messageRepository = new MessageRepository();
        }

        public void Apply(Update update, TelegramBotClient botClient, SessionProcessor sessionProcessor)
        {
            string buttonText = update.CallbackQuery.Data;

            if (buttonText == "Добавить новое")
            {
                sessionProcessor.Session_Status = SessionProcessor.SessionStatus.OpenSession;
                var sentMessage = botClient
                                .SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Введите новый текст сообщения для напоминания:\n")
                                .Result;
            }

            if (buttonText == "Показать все напоминания")
            {
                var allmessages = new List<DataMessage>();
                allmessages = _messageRepository.GetAll().ToList();

                var actualmessages = allmessages.Where(l => l.TimeToRemind > DateTime.Now && l.IsActive == true);
                var listMessageText = $"Активные заметки:\n";

                foreach (var actualmessage in actualmessages)
                {
                    listMessageText += $"\n{actualmessage.TimeToRemind.Date.ToString("dd/MM/yyyy H:mm")}, Id {actualmessage.Id}: {actualmessage.MessageText}\n";
                }
                var sentMessage = botClient
                                .SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"{listMessageText}\n")
                                .Result;
            }

            if (buttonText == "Удалить")
            {
                sessionProcessor.Session_Status = SessionProcessor.SessionStatus.DeleteIsSelected;
                var sentMessage = botClient
                                .SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Введите Id заметки, которую нужно удалить:\n")
                                .Result;
            }

            if (buttonText == "Изменить")
            {
                sessionProcessor.Session_Status = SessionProcessor.SessionStatus.UpdateIsSelected;
                var sentMessage = botClient
                                .SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Введите Id заметки, которую нужно изменить:\n")
                                .Result;
            }
            //добавить отключение напоминания.
        }
    }
}