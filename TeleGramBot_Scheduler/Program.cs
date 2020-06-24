using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleGramBot_Scheduler.Data;

namespace TeleGramBot_Scheduler
{
    class Program
    {
        public static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            Container = AutofacConfig.ConfigureContainer();

            var botClient = BotInitializer.GetBotClient();
            var processManger = new BotProcessMenager(botClient);
            processManger.Start();
        }
    }
}
