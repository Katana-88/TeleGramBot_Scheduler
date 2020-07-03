﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TeleGramBot_Scheduler.Data;
using TeleGramBot_Scheduler.Sessions;
using TeleGramBot_Scheduler.UpdateProcessors;
using Autofac;

namespace TeleGramBot_Scheduler
{
    public class BotProcessMenager
    {
        private readonly TelegramBotClient _botClient;
        private SessionProcessor sessionProcessor;
        public Dictionary<int, DateTime> DB_AsDateTimeId { get; set; }

        private List<IUpdateProcessor> updateProcessors = new List<IUpdateProcessor>
        {
            new TextMessageUpdateProcessor(Program.Container.BeginLifetimeScope().Resolve<IRepository<DataMessage>>(), 
                Program.Container.BeginLifetimeScope().Resolve<IRepository<SessionStatusForChatId>>()),
            new CallbackQueryProcessor(Program.Container.BeginLifetimeScope().Resolve<IRepository<DataMessage>>(),
                Program.Container.BeginLifetimeScope().Resolve<IRepository<SessionStatusForChatId>>()),
            new DateTimeUpdateProcessor(Program.Container.BeginLifetimeScope().Resolve<IRepository<DataMessage>>(),
                Program.Container.BeginLifetimeScope().Resolve<IRepository<SessionStatusForChatId>>()),
            new IdProcessor(Program.Container.BeginLifetimeScope().Resolve<IRepository<DataMessage>>(),
                Program.Container.BeginLifetimeScope().Resolve<IRepository<SessionStatusForChatId>>())
        };
        

        
        public BotProcessMenager(TelegramBotClient botClient)
        {
            _botClient = botClient;
            sessionProcessor = new SessionProcessor();
            DB_AsDateTimeId = new Dictionary<int, DateTime>();
        }

        public void Start()
        {
            LoadDb();
            while (true)
            {
                ShowMessageIfItsDateTimeToRemindIsNow();
                var updates = _botClient.GetUpdatesAsync(BotSettings.MessageOffset).Result;
                foreach (var update in updates)
                {     
                    if (update.Type == UpdateType.Message && update.Message.Text == "show")
                    {
                        sessionProcessor.IsSessionOpen = true;
                        ShowMenu(update);
                    }

                    var applicableUpdateProcessors = updateProcessors.Where(up => up.IsApplicable(update));

                    foreach (var updateProcessor in applicableUpdateProcessors)
                    {
                        updateProcessor.Apply(update, _botClient, sessionProcessor);
                    }

                    ChangeOffset(updates);
                }
            }
        }

        private void LoadDb()
        {
            if (DB_AsDateTimeId != null)
            {
                DB_AsDateTimeId = null;
                DB_AsDateTimeId = new Dictionary<int, DateTime>();
            }

            var allmessages = new List<DataMessage>();
            var repo = Program.Container.BeginLifetimeScope().Resolve<IRepository<DataMessage>>();
            allmessages = repo.GetAll().ToList();
            var actualmessages = allmessages.Where(l => l.TimeToRemind >= DateTime.Now && l.IsActive == true);
            foreach (DataMessage message in actualmessages)
            {
                DB_AsDateTimeId.Add(message.Id, message.TimeToRemind);
            }

        }

        private void ShowMessageIfItsDateTimeToRemindIsNow()
        {
            if (DB_AsDateTimeId != null)
            {
                var listMessageText = $"Сегодня Вы просили напомнить:\n";
                var dateTimeToCompare = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                var messagesToRemind = DB_AsDateTimeId.Where(k => k.Value.Date.CompareTo(dateTimeToCompare.Date) == 0 
                && k.Value.Hour.CompareTo(dateTimeToCompare.Hour) == 0
                && k.Value.Minute.CompareTo(dateTimeToCompare.Minute) == 0).ToList();

                var repo = Program.Container.BeginLifetimeScope().Resolve<IRepository<DataMessage>>();

                foreach (var messageToRemind in messagesToRemind)
                {
                    var message = repo.Get(messageToRemind.Key);
                    message.IsActive = false;
                    repo.Update(message);
                    ChangeDictionary(messageToRemind.Key);
                    listMessageText += $"\n{message.TimeToRemind.Date.ToString("dd/MM/yyyy H:mm")}, Id {message.Id}: {message.MessageText}\n";
                    Console.Write("2");
                    var sentMessage = _botClient 
                        .SendTextMessageAsync(message.ChatId, $"{listMessageText}\n")
                        .Result;
                }
            }
        }

        private void ChangeDictionary(int key)
        {
            DB_AsDateTimeId.Remove(key);
        }

        private void ShowMenu(Update update)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[] 
            {
                new[]{
                    InlineKeyboardButton.WithCallbackData("Показать все напоминания"),
                    InlineKeyboardButton.WithCallbackData("Добавить новое")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Изменить"),
                    InlineKeyboardButton.WithCallbackData("Удалить"),
                    InlineKeyboardButton.WithCallbackData("Выполнено")
                }
            });
            var sentMessage = _botClient
                .SendTextMessageAsync(update.Message.Chat.Id, $"Выберите пункт меню\n", replyMarkup: inlineKeyboard)
                .Result;
        }

        private static void ChangeOffset(Update[] updates)
        {
            var lastUpdate = updates.LastOrDefault();
            if (lastUpdate != null)
                BotSettings.MessageOffset = lastUpdate.Id + 1;
        }
    }
}
