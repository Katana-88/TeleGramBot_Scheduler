using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleGramBot_Scheduler.Data;

namespace TeleGramBot_Scheduler.Sessions
{
    public class SessionProcessor
    {
        public NameOfSession Name_Of_Session{ get; set; }

        public bool IsSessionOpen { get; set; }

        public enum NameOfSession
        {
            SessionProcessorForNewMessage,
            SessionProcessorForUpdateMessage,
            SessionProcessorForDeleteMessage,
            SessionProcessorForMarkAsDoneMessage
        }

        public SessionProcessor()
        {
            IsSessionOpen = false;
        }
    }
}