using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TeleGramBot_Scheduler.Data;

namespace TeleGramBot_Scheduler
{
    public static class AutofacConfig
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<SessionStatusForChatIdRepository>().As<IRepository<SessionStatusForChatId>>().SingleInstance();
            builder.RegisterType<MessageRepository>().As<IRepository<DataMessage>>().SingleInstance();
            builder.RegisterType<MessageContext>().As<MessageContext>().SingleInstance();
            builder.RegisterType<DbAsDictionary>().As<DbAsDictionary>().SingleInstance();
            var container = builder.Build();

            return container;
        }
    }
}
