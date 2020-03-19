using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TeleGramBot_Scheduler
{
    public static class BotInitializer
    {
        private static TelegramBotClient botClient;

        public static TelegramBotClient GetBotClient()
        {
            if (botClient != null)
                return botClient;

            var ApiKey = ConfigurationManager.AppSettings["telegramApiKey"];

            botClient = new TelegramBotClient(ApiKey);

            return botClient;
        }
    }
}
