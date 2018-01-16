using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Domain_Model;

namespace Repository
{
    public class GenericRepository<T> : RepositoryBase, IGenericRepository<T> where T : class
    {

        public GenericRepository(DMSDBEntities db) : base(db)
        {

        }

        public T Add(T item)
        {
            return db.Set<T>().Add(item);
        }

        public void Delete(int id)
        {
            var item = GetByID(id);
            db.Set<T>().Remove(item);
        }

        public void Delete(T item)
        {
            db.Set<T>().Remove(item);
        }

        public T GetByID(int id)
        {
            return db.Set<T>().Find(id);
        }

        public IQueryable<T> Query()
        {
            return db.Set<T>();
        }

        public void Update(T update)
        {
            db.Entry(update).State = System.Data.Entity.EntityState.Modified;
        }
    }
}
