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
       // private readonly DbSet<DataMessage> _dbSet;

        public MessageRepository()
        {
            _context = new MessageContext();
         //   _dbSet = _context.Set<DataMessage>();
        }


        public void Add(DataMessage entity)
        {
            _context.DataMessage.Add(entity);
            _context.Entry(entity).State = EntityState.Added;
            SaveChanges();
        }

        public void Delete(DataMessage entity)
        {
            var toDelete = Get(entity.Id);
            if (toDelete != null)
            {
                _context.DataMessage.Remove(toDelete);
                _context.Entry(toDelete).State = EntityState.Deleted;
                SaveChanges();
            }
        }

        public DataMessage Get(int id)
        {
            return _context.DataMessage.FirstOrDefault(m => m.Id == id);
        }

        public DataMessage GetLast()
        {
            return _context.DataMessage.OrderByDescending(m => m.Id).FirstOrDefault();
        }

        public IEnumerable<DataMessage> GetAll()
        {
            return _context.DataMessage.ToList();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Update(DataMessage entity)
        {
            var toUpdate = _context.DataMessage.AsNoTracking().FirstOrDefault(m => m.Id == entity.Id);
            if (toUpdate != null)
            {
                _context.Entry(entity).State = entity.Id == 0 ? EntityState.Added : EntityState.Modified;
                SaveChanges();
            }
        }
    }
}
