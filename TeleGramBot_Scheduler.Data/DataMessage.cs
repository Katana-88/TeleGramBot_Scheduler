using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleGramBot_Scheduler.Data
{
    public class DataMessage
    {
        public int Id { get; set; }

        public string MessageText { get; set; }

        public DateTime TimeToRemind { get; set; } = DateTime.Now;

        public bool IsActive { get; set; }
    }
}
