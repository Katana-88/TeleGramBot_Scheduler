using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleGramBot_Scheduler
{
    public class SessionProcessor
    {
        public string NameOfSession { get; set; }
        public SessionStatus Session_Status { get; set; }

        public enum SessionStatus
        {
            OpenSession,
            MessageIsApply,
            TimeToRemindIsApply,
            CloseSession,
            DeleteIsSelected,
            DeleteIdIsAply,
            UpdateIsSelected,
            UpdateMessageIsAply,
            UpdateDeteTimeIsAply,
            UpdateIdIsAply
        }

        public SessionProcessor()
        {
            Session_Status = SessionStatus.OpenSession;
        }
    }
}