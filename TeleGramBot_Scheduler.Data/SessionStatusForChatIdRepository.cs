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

        public SessionStatusForChatIdRepository()
        {
            _context = new MessageContext();
        }

        public void Add(SessionStatusForChatId entity)
        {
            _context.SessionStatusForChatId.Add(entity);
            _context.Entry(entity).State = EntityState.Added;
            SaveChanges();
        }

        public void Delete(SessionStatusForChatId entity)
        {
            throw new NotImplementedException();
        }

        public SessionStatusForChatId Get(int id)
        {
            return _context.SessionStatusForChatId.FirstOrDefault(s => s.Id == id);
        }

        public IEnumerable<SessionStatusForChatId> GetAll()
        {
            return _context.SessionStatusForChatId.ToList();
        }

        public SessionStatusForChatId GetLast()
        {
            return _context.SessionStatusForChatId.OrderByDescending(s => s.Id).FirstOrDefault();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Update(SessionStatusForChatId entity)
        {
            var toUpdate = _context.SessionStatusForChatId.FirstOrDefault(s => s.Id == entity.Id);
            if (toUpdate != null)
            {
                _context.Entry(entity).State = EntityState.Modified;
                SaveChanges();
            }
        }
    }
}
