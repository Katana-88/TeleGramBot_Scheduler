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
    public class IdProcessor : IUpdateProcessor
    {
        private readonly IRepository<DataMessage> _messageRepository;
        //private readonly IRepository<SessionStatusForChatId> _sessionStatusForChatIdRepo;

        public bool IsApplicable(Update update)
            => update.Type == UpdateType.Message && update.Message.Text != null && int.TryParse(update.Message.Text, out int result);

        public IdProcessor()
        {
            _messageRepository = new MessageRepository();
            //_sessionStatusForChatIdRepo = new SessionStatusForChatIdRepository();
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

            var allStatuses = sessionProcessor._sessionStatusForChatIdRepo.GetAll();
            var currentStatusState = allStatuses.OrderByDescending(s => s.Id).FirstOrDefault(s => s.ChatId == update.Message.Chat.Id);
            Console.WriteLine($"{currentStatusState.SessionProcessor}, {currentStatusState.SessionStatus}");
            if (currentStatusState.SessionProcessor == (int)SessionProcessor.NameOfSession.SessionProcessorForDeleteMessage
                && currentStatusState.SessionStatus == (int)SessionProcessorForDeleteMessage.SessionStatus.OpenSession)
            {
                int idToDelete = int.Parse(update.Message.Text);
                var messageToDelete = _messageRepository.Get(idToDelete);
                if (messageToDelete != null)
                {
                    _messageRepository.Delete(messageToDelete);
                    currentStatusState.SessionStatus = (int)SessionProcessorForDeleteMessage.SessionStatus.DeleteIdIsAply;
                    sessionProcessor._sessionStatusForChatIdRepo.Update(currentStatusState);
                    sessionProcessor.IsSessionOpen = false;

                    var sentMessage = botClient
                                    .SendTextMessageAsync(update.Message.Chat.Id, $"Заметка удалена.\n")
                                    .Result;
                }
                else
                {
                    var sentMessage = botClient
                                  .SendTextMessageAsync(update.Message.Chat.Id, $"Заметка с таким Id не найдена.\n")
                                  .Result;
                }
            }

            if (currentStatusState.SessionProcessor == (int)SessionProcessor.NameOfSession.SessionProcessorForMarkAsDoneMessage
                && currentStatusState.SessionStatus == (int)SessionProcessorForMarkAsDoneMessage.SessionStatus.OpenSession)
            {
                int idToMarkAsDone = int.Parse(update.Message.Text);
                var messageToMarkAsDone = _messageRepository.Get(idToMarkAsDone);
                if (messageToMarkAsDone != null)
                {
                    messageToMarkAsDone.IsActive = false;
                    _messageRepository.Update(messageToMarkAsDone);

                    currentStatusState.SessionStatus = (int)SessionProcessorForMarkAsDoneMessage.SessionStatus.CloseSession;
                    sessionProcessor._sessionStatusForChatIdRepo.Update(currentStatusState);

                    sessionProcessor.IsSessionOpen = false;

                    var sentMessage = botClient
                                    .SendTextMessageAsync(update.Message.Chat.Id, $"Заметка отмечена как выполненная.\n")
                                    .Result;
                }
                else
                {
                    var sentMessage = botClient
                                  .SendTextMessageAsync(update.Message.Chat.Id, $"Заметка с таким Id не найдена.\n")
                                  .Result;
                }
            }

            if (currentStatusState.SessionProcessor == (int)SessionProcessor.NameOfSession.SessionProcessorForUpdateMessage
                && currentStatusState.SessionStatus == (int)SessionProcessorForUpdateMessage.SessionStatus.OpenSession)
            {
                int idToUpdate = int.Parse(update.Message.Text);
                var messageToUpdate = _messageRepository.Get(idToUpdate);

                if (messageToUpdate != null)
                {
                    currentStatusState.SessionStatus = (int)SessionProcessorForUpdateMessage.SessionStatus.UpdateIdIsAply;
                    currentStatusState.MessageId = idToUpdate;
                    sessionProcessor._sessionStatusForChatIdRepo.Update(currentStatusState);

                    var sentMessage = botClient
                                    .SendTextMessageAsync(update.Message.Chat.Id, $"Заметка: {messageToUpdate.MessageText}, время напоминания: {messageToUpdate.TimeToRemind}\n" +
                                    $"Введите новый текст заметки или скопируйте старый:\n")
                                   .Result;
                }
                else
                {
                    var sentMessage = botClient
                                  .SendTextMessageAsync(update.Message.Chat.Id, $"Заметка с таким Id не найдена.\n")
                                  .Result;
                }
            }
        }
    }
}
