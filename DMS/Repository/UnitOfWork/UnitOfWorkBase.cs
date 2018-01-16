using Model.Domain_Model;

namespace Repository.UnitOfWork
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        protected readonly DMSDBEntities db;

        protected UnitOfWorkBase()
        {
            db = new DMSDBEntities();
        }

        public void Commit()
        {
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
