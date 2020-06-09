using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TeleGramBot_Scheduler.Data;
using TeleGramBot_Scheduler.Sessions;

namespace TeleGramBot_Scheduler.UpdateProcessors
{
    public class TextMessageUpdateProcessor : IUpdateProcessor
    {
        private readonly IRepository<DataMessage> _messageRepository;
        private readonly IRepository<SessionStatusForChatId> _sessionStatusForChatIdRepo;

        public bool IsApplicable(Update update)
            => update.Type == UpdateType.Message && update.Message.Text != null && update.Message.Text !="show" 
            && !DateTime.TryParse(update.Message.Text, out DateTime result) && !int.TryParse(update.Message.Text, out int result2);

        public TextMessageUpdateProcessor()
        {
            _messageRepository = new MessageRepository();
            _sessionStatusForChatIdRepo = new SessionStatusForChatIdRepository();
        }

        public void Apply(Update update, TelegramBotClient botClient, SessionProcessor sessionProcessor)
        {
            var message = update.Message;

            if (sessionProcessor.IsSessionOpen == false)
            {
                var errorMessage = botClient
                    .SendTextMessageAsync(message.Chat.Id, $"Сперва вызовите меню через команду show.\n")
                    .Result;
                return;
            }
            var allStatuses = _sessionStatusForChatIdRepo.GetAll();
            var currentStatusState = allStatuses.Where(a => a.ChatId == update.Message.Chat.Id).FirstOrDefault();
            
            if (currentStatusState.SessionProcessor == (int)SessionProcessor.NameOfSession.SessionProcessorForUpdateMessage
                && currentStatusState.SessionStatus != (int)SessionProcessorForUpdateMessage.SessionStatus.UpdateIdIsAply)
            {
                var errorMessage = botClient
                    .SendTextMessageAsync(message.Chat.Id, $"Вы не закончили оформлять заметку.\n")
                    .Result;
                return;
            }

            if (currentStatusState.SessionProcessor == (int)SessionProcessor.NameOfSession.SessionProcessorForNewMessage
                && currentStatusState.SessionStatus != (int)SessionProcessorForNewMessage.SessionStatus.OpenSession)
            {
                var errorMessage = botClient
                    .SendTextMessageAsync(message.Chat.Id, $"Вы не закончили оформлять заметку.\n")
                    .Result;
                return;
            }

            var dataMessage = new DataMessage { MessageText = message.Text, IsActive = true, ChatId = (int)message.Chat.Id };
            

            if (currentStatusState.SessionProcessor == (int)SessionProcessor.NameOfSession.SessionProcessorForUpdateMessage
                && currentStatusState.SessionStatus == (int)SessionProcessorForUpdateMessage.SessionStatus.UpdateIdIsAply)
            {
                var messageToUpdate = _messageRepository.Get(currentStatusState.MessageId);
                messageToUpdate.MessageText = dataMessage.MessageText;
                _messageRepository.Update(messageToUpdate);
             //   _messageRepository.SaveChanges();
                currentStatusState.SessionStatus = (int)SessionProcessorForUpdateMessage.SessionStatus.UpdateMessageIsAply;
                _sessionStatusForChatIdRepo.Update(currentStatusState);
           //     _sessionStatusForChatIdRepo.SaveChanges();
            }
            else if (currentStatusState.SessionProcessor == (int)SessionProcessor.NameOfSession.SessionProcessorForNewMessage
                && currentStatusState.SessionStatus == (int)SessionProcessorForNewMessage.SessionStatus.OpenSession)
            {
                _messageRepository.Add(dataMessage);
          //      _messageRepository.SaveChanges();
                currentStatusState.SessionStatus = (int)SessionProcessorForNewMessage.SessionStatus.MessageIsApply;
                _sessionStatusForChatIdRepo.Update(currentStatusState);
          //      _sessionStatusForChatIdRepo.SaveChanges();
            }

            var sentMessage = botClient
                .SendTextMessageAsync(message.Chat.Id, $"Ваша заметка с текстом:\n{message.Text} сохранена. Заполните дату и время для напоминания в формате 'DD.MM.YY HH:MM:SS':\n")
                .Result;
        }
    }
}
