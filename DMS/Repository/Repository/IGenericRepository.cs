using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IGenericRepository<T>
    {
        IQueryable<T> Query();
        T GetByID(int id);
        T Add(T item);
        void Update(T update);
        void Delete(int id);
    }
}
