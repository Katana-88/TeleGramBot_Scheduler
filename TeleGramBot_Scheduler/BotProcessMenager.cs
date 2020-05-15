using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleGramBot_Scheduler
{
    public class BotProcessMenager
    {
        private readonly TelegramBotClient _botClient;
        private TextMessageUpdateProcessor updateProcessor = new TextMessageUpdateProcessor();
        private SessionProcessor sessionProcessor = new SessionProcessor();
        private DateTimeUpdateProcessor dateTimeUpdateProcessor = new DateTimeUpdateProcessor();

        public BotProcessMenager(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public void Start()
        {
            while (true)
            {
                var updates = _botClient.GetUpdatesAsync(BotSettings.MessageOffset).Result;
                foreach (var update in updates)
                {
                    while (sessionProcessor.Session_Status != SessionProcessor.SessionStatus.CloseSession)
                    {
                        if (updateProcessor.IsApplicable(update))
                        {
                            updateProcessor.Apply(update, _botClient);
                            sessionProcessor.Session_Status = SessionProcessor.SessionStatus.MessageIsApply;
                        }
                        if (dateTimeUpdateProcessor.IsApplicable(update))
                        {
                            dateTimeUpdateProcessor.Apply(update, _botClient);
                            sessionProcessor.Session_Status = SessionProcessor.SessionStatus.CloseSession;
                        }
                    }
                    ChangeOffset(updates);
                }
            }
        }

        private static void ChangeOffset(Update[] updates)
        {
            var lastUpdate = updates.LastOrDefault();
            if (lastUpdate != null)
                BotSettings.MessageOffset = lastUpdate.Id + 1;
        }
    }
}
