using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleGramBot_Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            var botClient = BotInitializer.GetBotClient();
            var processManger = new BotProcessMenager(botClient);
            processManger.Start();
        }
    }
}
