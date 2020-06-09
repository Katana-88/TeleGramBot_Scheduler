using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TeleGramBot_Scheduler.Sessions;

namespace TeleGramBot_Scheduler.UpdateProcessors
{
    public interface IUpdateProcessor
    {
        bool IsApplicable(Update update);
        void Apply(Update update, TelegramBotClient botClient, SessionProcessor sessionProcessor);
    }
}
