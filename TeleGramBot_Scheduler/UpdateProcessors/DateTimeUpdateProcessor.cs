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
    public class DateTimeUpdateProcessor : IUpdateProcessor
    {
        private readonly IRepository<DataMessage> _messageRepository;
        public bool IsApplicable(Update update)
            => update.Type == UpdateType.Message && DateTime.TryParse(update.Message.Text, out DateTime result) && update.Message.Text != null;

        public DateTimeUpdateProcessor()
        {
            _messageRepository = new MessageRepository();
        }

        public void Apply(Update update, TelegramBotClient botClient, SessionProcessor sessionProcessor)
        {
            var message = update.Message;

            if (sessionProcessor.Session_Status != SessionProcessor.SessionStatus.MessageIsApply && sessionProcessor.Session_Status != SessionProcessor.SessionStatus.UpdateMessageIsAply)
            {
                var errorMessage = botClient
                  .SendTextMessageAsync(message.Chat.Id, $"Текст заметки не сохранился, попробуйте ещё раз.\n")
                  .Result;
                return;
            }

            DateTime newDate = DateTime.Parse(message.Text);
            var messageToUpdate = _messageRepository.GetLast();
            messageToUpdate.TimeToRemind = newDate;
            _messageRepository.Update(messageToUpdate);
            _messageRepository.SaveChanges();

            if (sessionProcessor.Session_Status == SessionProcessor.SessionStatus.UpdateMessageIsAply)
            {
                sessionProcessor.Session_Status = SessionProcessor.SessionStatus.UpdateDeteTimeIsAply;
                var sentMessage = botClient
                .SendTextMessageAsync(message.Chat.Id, $"Дата и время сохранены. Укажите ещё раз Id заметки, которую нужно заменить на новую:\n")
                .Result;
            }
            else
            {
                sessionProcessor.Session_Status = SessionProcessor.SessionStatus.TimeToRemindIsApply;
                var sentMessage = botClient
                .SendTextMessageAsync(message.Chat.Id, $"Дата и время сохранены.\n")
                .Result;
            }         
        }
    }
}