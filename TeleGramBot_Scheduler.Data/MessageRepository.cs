using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleGramBot_Scheduler.Data
{
    public class MessageRepository : IRepository<DataMessage>
    {
        private readonly MessageContext _context;
        private readonly DbSet<DataMessage> _dbSet;

        public MessageRepository()
        {
            _context = new MessageContext();
            _dbSet = _context.Set<DataMessage>();
        }
        public void Add(DataMessage entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(DataMessage entity)
        {
            var toDelete = Get(entity.Id);
            if (toDelete != null)
                _dbSet.Remove(toDelete);
        }

        public DataMessage Get(int id)
        {
            return _dbSet.FirstOrDefault(m => m.Id == id);
        }

        public DataMessage GetLast()
        {
            return _dbSet.OrderByDescending(m => m.Id).FirstOrDefault();
        }

        public IEnumerable<DataMessage> GetAll()
        {
            return _dbSet.ToList();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Update(DataMessage entity)
        {
            var toUpdate = Get(entity.Id);
            if (toUpdate != null)
                toUpdate = entity;
        }
    }
}
