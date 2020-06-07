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
    public class TextMessageUpdateProcessor : IUpdateProcessor
    {
        private readonly IRepository<DataMessage> _messageRepository;
        public bool IsApplicable(Update update)
            => update.Type == UpdateType.Message && update.Message.Text != null && update.Message.Text !="show" 
            && !DateTime.TryParse(update.Message.Text, out DateTime result) && !int.TryParse(update.Message.Text, out int result2);

        public TextMessageUpdateProcessor()
        {
            _messageRepository = new MessageRepository();
        }

        public void Apply(Update update, TelegramBotClient botClient, SessionProcessor sessionProcessor)
        {
            var message = update.Message;
            if (sessionProcessor.Session_Status != SessionProcessor.SessionStatus.OpenSession && sessionProcessor.Session_Status != SessionProcessor.SessionStatus.UpdateIsSelected)
            {
                var errorMessage = botClient
                    .SendTextMessageAsync(message.Chat.Id, $"Вы не закончили оформлять заметку.\n")
                    .Result;
                return;
            }

            var dataMessage = new DataMessage { MessageText = message.Text, IsActive = true, ChatId = (int)message.Chat.Id };
            _messageRepository.Add(dataMessage);
            _messageRepository.SaveChanges();
            if (sessionProcessor.Session_Status == SessionProcessor.SessionStatus.UpdateIsSelected)
            {
                sessionProcessor.Session_Status = SessionProcessor.SessionStatus.UpdateMessageIsAply;
            }
            else
            {
                sessionProcessor.Session_Status = SessionProcessor.SessionStatus.MessageIsApply;
            }
            var sentMessage = botClient
                .SendTextMessageAsync(message.Chat.Id, $"Ваша заметка с текстом:\n{message.Text} сохранена. Заполните дату и время для напоминания в формате 'DD.MM.YY HH:MM:SS':\n")
                .Result;
        }
    }
}
