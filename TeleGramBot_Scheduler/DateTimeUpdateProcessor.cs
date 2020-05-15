using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TeleGramBot_Scheduler.Data;

namespace TeleGramBot_Scheduler
{
    public class DateTimeUpdateProcessor
    {
        private readonly IRepository<DataMessage> _messageRepository;
        public bool IsApplicable(Update update)
            => update.Type == UpdateType.Message && DateTime.TryParse(update.Message.Text, out DateTime result) && update.Message.Text != null;

        public DateTimeUpdateProcessor()
        {
            _messageRepository = new MessageRepository();
        }

        public void Apply(Update update, TelegramBotClient botClient)
        {
            var message = update.Message;
            DateTime newDate = DateTime.Parse(message.Text);
            var messageToUpdate = _messageRepository.GetLast();
            messageToUpdate.TimeToRemind = newDate;
            _messageRepository.Update(messageToUpdate);
            _messageRepository.SaveChanges();

            var sentMessage = botClient
                .SendTextMessageAsync(message.Chat.Id, $"DateTime was added\n")
                .Result;
        }
    }
}