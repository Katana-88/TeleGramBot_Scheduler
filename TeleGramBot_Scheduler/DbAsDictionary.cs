using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleGramBot_Scheduler.Data;

namespace TeleGramBot_Scheduler
{
    public class DbAsDictionary
    {
        public Dictionary<int, DateTime> DB_AsDateTimeId { get; set; }

        public DbAsDictionary()
        {
            DB_AsDateTimeId = new Dictionary<int, DateTime>();
        }

        public void LoadDb()
        {
            if (DB_AsDateTimeId != null)
            {
                DB_AsDateTimeId = null;
                DB_AsDateTimeId = new Dictionary<int, DateTime>();
            }

            var allmessages = new List<DataMessage>();
            var repo = Program.Container.BeginLifetimeScope().Resolve<IRepository<DataMessage>>();
            allmessages = repo.GetAll().ToList();
            var actualmessages = allmessages.Where(l => l.TimeToRemind >= DateTime.Now && l.IsActive == true);
            foreach (DataMessage message in actualmessages)
            {
                DB_AsDateTimeId.Add(message.Id, message.TimeToRemind);
            }

        }
    }
}
