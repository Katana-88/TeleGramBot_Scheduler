using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleGramBot_Scheduler.Sessions
{
    public class SessionProcessorForDeleteMessage
    {
        public Dictionary<int, int> ChatId_SassionState;

        public SessionStatus Session_Status { get; set; }

        public enum SessionStatus
        {
            OpenSession,
            CloseSession,
            DeleteIsSelected,
            DeleteIdIsAply
        }

        public SessionProcessorForDeleteMessage()
        {
            ChatId_SassionState = new Dictionary<int, int>();
        }
    }
}
