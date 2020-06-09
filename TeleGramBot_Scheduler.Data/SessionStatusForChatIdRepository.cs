using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleGramBot_Scheduler.Data
{
    public class SessionStatusForChatIdRepository : IRepository<SessionStatusForChatId>
    {
        private readonly MessageContext _context;

        private readonly DbSet<SessionStatusForChatId> _dbSet;

        public SessionStatusForChatIdRepository()
        {
            _context = new MessageContext();
            _dbSet = _context.Set<SessionStatusForChatId>();
        }

        public void Add(SessionStatusForChatId entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(SessionStatusForChatId entity)
        {
            throw new NotImplementedException();
        }

        public SessionStatusForChatId Get(int id)
        {
            return _dbSet.FirstOrDefault(m => m.Id == id);
        }

        public IEnumerable<SessionStatusForChatId> GetAll()
        {
            return _dbSet.ToList();
        }

        public SessionStatusForChatId GetLast()
        {
            return _dbSet.OrderByDescending(m => m.Id).FirstOrDefault();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Update(SessionStatusForChatId entity)
        {
            var toUpdate = Get(entity.Id);
            if (toUpdate != null)
                toUpdate = entity;
        }
    }
}
