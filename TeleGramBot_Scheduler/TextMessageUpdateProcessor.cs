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
    public class TextMessageUpdateProcessor
    {
        private readonly IRepository<DataMessage> _messageRepository;
        public bool IsApplicable(Update update)
            => update.Type == UpdateType.Message && update.Message.Text != null;

        public TextMessageUpdateProcessor()
        {
            _messageRepository = new MessageRepository();
        }

        public void Apply(Update update, TelegramBotClient botClient)
        {
            var message = update.Message;
            var dataMessage = new DataMessage {MessageText = message.Text, IsActive = true};
            _messageRepository.Add(dataMessage);
            var sentMessage = botClient
                .SendTextMessageAsync(message.Chat.Id, $"Your task: {message.MessageId} with text:\n{message.Text} is saved. Type DateTime to remind:\n")
                .Result;
        }
    }
}
