using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleGramBot_Scheduler.Data
{
    public class MessageContext : DbContext
    {
        public DbSet<DataMessage> DataMessage { get; set; }

        public MessageContext()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<MessageContext>());
        }
    }
}
