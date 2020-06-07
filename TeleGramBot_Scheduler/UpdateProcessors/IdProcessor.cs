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
    public class IdProcessor : IUpdateProcessor
    {
        private readonly IRepository<DataMessage> _messageRepository;
        public bool IsApplicable(Update update)
            => update.Type == UpdateType.Message && update.Message.Text != null && int.TryParse(update.Message.Text, out int result);

        public IdProcessor()
        {
            _messageRepository = new MessageRepository();
        }

        public void Apply(Update update, TelegramBotClient botClient, SessionProcessor sessionProcessor)
        {
            if (sessionProcessor.Session_Status == SessionProcessor.SessionStatus.DeleteIsSelected)
            {
                int idToDelete = int.Parse(update.Message.Text);
                var messageToDelete = _messageRepository.Get(idToDelete);
                if (messageToDelete != null)
                {
                    _messageRepository.Delete(messageToDelete);
                    _messageRepository.SaveChanges();
                    sessionProcessor.Session_Status = SessionProcessor.SessionStatus.DeleteIdIsAply;

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

            if (sessionProcessor.Session_Status == SessionProcessor.SessionStatus.DoneIsSelected)
            {
                int idToMarkAsDone = int.Parse(update.Message.Text);
                var messageToMarkAsDone = _messageRepository.Get(idToMarkAsDone);
                if (messageToMarkAsDone != null)
                {
                    messageToMarkAsDone.IsActive = false;
                    _messageRepository.Update(messageToMarkAsDone);
                    _messageRepository.SaveChanges();
                    sessionProcessor.Session_Status = SessionProcessor.SessionStatus.OpenSession;

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

            if (sessionProcessor.Session_Status == SessionProcessor.SessionStatus.UpdateIsSelected)
            {
                int idToUpdate = int.Parse(update.Message.Text);
                var messageToUpdate = _messageRepository.Get(idToUpdate);
                if (messageToUpdate != null)
                {
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

            if (sessionProcessor.Session_Status == SessionProcessor.SessionStatus.UpdateDeteTimeIsAply)
            {
                int idToUpdateDelete = int.Parse(update.Message.Text);
                var messageToUpdateDelete = _messageRepository.Get(idToUpdateDelete);
                if (messageToUpdateDelete != null)
                {
                    _messageRepository.Delete(messageToUpdateDelete);
                    _messageRepository.SaveChanges();
                    sessionProcessor.Session_Status = SessionProcessor.SessionStatus.OpenSession;

                    var sentMessage = botClient
                                    .SendTextMessageAsync(update.Message.Chat.Id, $"Сообщение сохранено с новым Id.\n")
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
