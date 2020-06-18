using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public class CallbackQueryProcessor : IUpdateProcessor
    {
        private readonly IRepository<DataMessage> _messageRepository;
        private readonly IRepository<SessionStatusForChatId> _sessionStatusForChatIdRepo;
        public bool IsApplicable(Update update)
            => update.Type == UpdateType.CallbackQuery;

        public CallbackQueryProcessor()
        {
            _messageRepository = new MessageRepository();
            _sessionStatusForChatIdRepo = new SessionStatusForChatIdRepository();
        }

        public void Apply(Update update, TelegramBotClient botClient, SessionProcessor sessionProcessor)
        {
            string buttonText = update.CallbackQuery.Data;

            IEnumerable<SessionStatusForChatId> allSessionStates = null;
            SessionStatusForChatId sessionStateForCurrentChat = null;
            try
            {
                allSessionStates = _sessionStatusForChatIdRepo.GetAll();
            }
            catch (Exception ex) { Console.WriteLine($"{ex.Message}"); }

            if (allSessionStates != null)
            {
                sessionStateForCurrentChat = allSessionStates.OrderByDescending(s => s.Id).FirstOrDefault(s => s.ChatId == update.CallbackQuery.Message.Chat.Id);
            }

            if (buttonText == "Добавить новое")
            {
                sessionProcessor.Name_Of_Session = SessionProcessor.NameOfSession.SessionProcessorForNewMessage;
                sessionProcessor.IsSessionOpen = true;

                if (sessionStateForCurrentChat == null)
                {
                    SessionStatusForChatId addSesion = new SessionStatusForChatId
                    {
                        ChatId = update.CallbackQuery.Message.Chat.Id,
                        SessionProcessor = (int)SessionProcessor.NameOfSession.SessionProcessorForNewMessage,
                        SessionStatus = (int)SessionProcessorForNewMessage.SessionStatus.OpenSession
                    };
                    _sessionStatusForChatIdRepo.Add(addSesion);
                }


                if (sessionStateForCurrentChat != null)
                {
                    sessionStateForCurrentChat.SessionProcessor = (int)SessionProcessor.NameOfSession.SessionProcessorForNewMessage;
                    sessionStateForCurrentChat.SessionStatus = (int)SessionProcessorForNewMessage.SessionStatus.OpenSession;
                    _sessionStatusForChatIdRepo.Update(sessionStateForCurrentChat);
                }

                    var sentMessage = botClient
                                .SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Введите новый текст сообщения для напоминания:\n")
                                .Result;
            }

            if (buttonText == "Показать все напоминания")
            {
                sessionProcessor.IsSessionOpen = true;
                var allmessages = new List<DataMessage>();
                allmessages = _messageRepository.GetAll().ToList();
                var allStatusesForCurrentChatId = allmessages.Where(a => a.ChatId == update.CallbackQuery.Message.Chat.Id);
                var actualmessages = allStatusesForCurrentChatId.Where(l => l.TimeToRemind > DateTime.Now && l.IsActive == true);

                var listMessageText = $"Активные заметки:\n";

                foreach (var actualmessage in actualmessages)
                {
                    listMessageText += $"\n{actualmessage.TimeToRemind.Date.ToString("dd/MM/yyyy HH:mm")}, Id {actualmessage.Id}: {actualmessage.MessageText}\n";
                }
                sessionProcessor.IsSessionOpen = false;
                var sentMessage = botClient
                                .SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"{listMessageText}\n")
                                .Result;
            }

            if (buttonText == "Удалить")
            {
                sessionProcessor.Name_Of_Session = SessionProcessor.NameOfSession.SessionProcessorForDeleteMessage;
                sessionProcessor.IsSessionOpen = true;

                if (sessionStateForCurrentChat == null)
                {
                    SessionStatusForChatId addSesion = new SessionStatusForChatId
                    {
                        ChatId = update.CallbackQuery.Message.Chat.Id,
                        SessionProcessor = (int)SessionProcessor.NameOfSession.SessionProcessorForDeleteMessage,
                        SessionStatus = (int)SessionProcessorForDeleteMessage.SessionStatus.OpenSession
                    };
                    _sessionStatusForChatIdRepo.Add(addSesion);
                }

                if (sessionStateForCurrentChat != null)
                {
                    sessionStateForCurrentChat.SessionProcessor = (int)SessionProcessor.NameOfSession.SessionProcessorForDeleteMessage;
                    sessionStateForCurrentChat.SessionStatus = (int)SessionProcessorForDeleteMessage.SessionStatus.OpenSession;
                    _sessionStatusForChatIdRepo.Update(sessionStateForCurrentChat);
                }

                var sentMessage = botClient
                                .SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Введите Id заметки, которую нужно удалить:\n")
                                .Result;
            }

            if (buttonText == "Изменить")
            {               
                sessionProcessor.Name_Of_Session = SessionProcessor.NameOfSession.SessionProcessorForUpdateMessage;
                sessionProcessor.IsSessionOpen = true;

                if (sessionStateForCurrentChat == null)
                {
                    SessionStatusForChatId addSesion = new SessionStatusForChatId
                    {
                        ChatId = update.CallbackQuery.Message.Chat.Id,
                        SessionProcessor = (int)SessionProcessor.NameOfSession.SessionProcessorForUpdateMessage,
                        SessionStatus = (int)SessionProcessorForUpdateMessage.SessionStatus.OpenSession
                    };
                    _sessionStatusForChatIdRepo.Add(addSesion);
                }


                if (sessionStateForCurrentChat != null)
                {
                    sessionStateForCurrentChat.SessionProcessor = (int)SessionProcessor.NameOfSession.SessionProcessorForUpdateMessage;
                    _sessionStatusForChatIdRepo.Update(sessionStateForCurrentChat);
                    sessionStateForCurrentChat.SessionStatus = (int)SessionProcessorForUpdateMessage.SessionStatus.OpenSession;
                    _sessionStatusForChatIdRepo.Update(sessionStateForCurrentChat);
                }

                var sentMessage = botClient
                                .SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Введите Id заметки, которую нужно изменить:\n")
                                .Result;
            }

            if (buttonText == "Выполнено")
            {
                sessionProcessor.Name_Of_Session = SessionProcessor.NameOfSession.SessionProcessorForMarkAsDoneMessage;
                sessionProcessor.IsSessionOpen = true;

                if (sessionStateForCurrentChat == null)
                {
                    SessionStatusForChatId addSesion = new SessionStatusForChatId
                    {
                        ChatId = update.CallbackQuery.Message.Chat.Id,
                        SessionProcessor = (int)SessionProcessor.NameOfSession.SessionProcessorForMarkAsDoneMessage,
                        SessionStatus = (int)SessionProcessorForMarkAsDoneMessage.SessionStatus.OpenSession
                    };
                    _sessionStatusForChatIdRepo.Add(addSesion);
                }


                if (sessionStateForCurrentChat != null)
                {
                    sessionStateForCurrentChat.SessionProcessor = (int)SessionProcessor.NameOfSession.SessionProcessorForMarkAsDoneMessage;
                    sessionStateForCurrentChat.SessionStatus = (int)SessionProcessorForMarkAsDoneMessage.SessionStatus.OpenSession;
                    _sessionStatusForChatIdRepo.Update(sessionStateForCurrentChat);
                }
                
                var sentMessage = botClient
                                .SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Введите Id заметки, которую Вы выполнили:\n")
                                .Result;
            }
        }
    }
}