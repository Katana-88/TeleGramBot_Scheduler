using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleGramBot_Scheduler.Sessions
{
    public class SessionProcessorForNewMessage
    {
        public Dictionary<long, int> ChatId_SassionState;

        public SessionStatus Session_Status { get; set; }

        public enum SessionStatus
        {
            OpenSession,
            CloseSession,
            MessageIsApply,
            TimeToRemindIsApply
        }

        public SessionProcessorForNewMessage()
        {
            ChatId_SassionState = new Dictionary<long, int>();
        }
    }
}
