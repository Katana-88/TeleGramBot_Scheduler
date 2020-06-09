using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleGramBot_Scheduler.Data
{
    public class SessionStatusForChatId
    {
        public int Id { get; set; }

        public long ChatId { get; set; }

        public int SessionProcessor { get; set; }

        public int SessionStatus { get; set; }

        public int MessageId { get; set; }
    }
}
