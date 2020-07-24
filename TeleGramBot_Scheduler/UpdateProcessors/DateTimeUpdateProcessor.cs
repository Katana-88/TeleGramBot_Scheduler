using Autofac;
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
    public class DateTimeUpdateProcessor : IUpdateProcessor
    {
        private readonly IRepository<DataMessage> _messageRepository;
        private readonly IRepository<SessionStatusForChatId> _sessionStatusForChatIdRepo;
        private DbAsDictionary dbAsDictionary { get; set; }

        public bool IsApplicable(Update update)
            => update.Type == UpdateType.Message && DateTime.TryParse(update.Message.Text, out DateTime result) && update.Message.Text != null;

        public DateTimeUpdateProcessor(IRepository<DataMessage> messageRepository, IRepository<SessionStatusForChatId> sessionStatusForChatIdRepo)
        {
            _messageRepository = messageRepository;
            _sessionStatusForChatIdRepo = sessionStatusForChatIdRepo;
            dbAsDictionary = Program.Container.BeginLifetimeScope().Resolve<DbAsDictionary>();
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
            var currentStatusState = allStatuses.OrderByDescending(s => s.Id).FirstOrDefault(s => s.ChatId == update.Message.Chat.Id);

            if (currentStatusState.SessionProcessor == (int)SessionProcessor.NameOfSession.SessionProcessorForNewMessage
                && currentStatusState.SessionStatus != (int)SessionProcessorForNewMessage.SessionStatus.MessageIsApply)
            {
                var errorMessage = botClient
                  .SendTextMessageAsync(message.Chat.Id, $"Текст заметки не сохранился, попробуйте ещё раз.\n")
                  .Result;
                return;
            }
            if (currentStatusState.SessionProcessor == (int)SessionProcessor.NameOfSession.SessionProcessorForUpdateMessage
                && currentStatusState.SessionStatus != (int)SessionProcessorForUpdateMessage.SessionStatus.UpdateMessageIsAply)
            {
                var errorMessage = botClient
                  .SendTextMessageAsync(message.Chat.Id, $"Текст заметки не сохранился, попробуйте ещё раз.\n")
                  .Result;
                return;
            }

            DateTime newDate = DateTime.Parse(message.Text);

            if (currentStatusState.SessionProcessor == (int)SessionProcessor.NameOfSession.SessionProcessorForNewMessage
                && currentStatusState.SessionStatus == (int)SessionProcessorForNewMessage.SessionStatus.MessageIsApply)
            {
                var allMessage = _messageRepository.GetAll();
                var messageWithRecentChatId = allMessage.Where(m => m.ChatId == message.Chat.Id);
                var messageToUpdate = messageWithRecentChatId.OrderByDescending(m => m.Id).FirstOrDefault();
                messageToUpdate.TimeToRemind = newDate;
                _messageRepository.Update(messageToUpdate);

                currentStatusState.SessionStatus = (int)SessionProcessorForNewMessage.SessionStatus.TimeToRemindIsApply;
                _sessionStatusForChatIdRepo.Update(currentStatusState);
            }

            if (currentStatusState.SessionProcessor == (int)SessionProcessor.NameOfSession.SessionProcessorForUpdateMessage
                && currentStatusState.SessionStatus == (int)SessionProcessorForUpdateMessage.SessionStatus.UpdateMessageIsAply)
            {
                var messageToUpdate = _messageRepository.Get(currentStatusState.MessageId);
                messageToUpdate.TimeToRemind = newDate;
                _messageRepository.Update(messageToUpdate);

                currentStatusState.SessionStatus = (int)SessionProcessorForUpdateMessage.SessionStatus.UpdateDeteTimeIsAply;
                _sessionStatusForChatIdRepo.Update(currentStatusState);
            }

            sessionProcessor.IsSessionOpen = false;

            dbAsDictionary.LoadDb();

            var sentMessage = botClient
                .SendTextMessageAsync(message.Chat.Id, $"Дата и время сохранены.\n")
                .Result;
        }
    }
}