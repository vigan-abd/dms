using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Domain_Model;
using System.Data.SqlClient;

namespace Repository
{
    public abstract class ClassicRepositoryBase : IClassicRepository
    {
        protected SqlConnection db;
        public void GetSQLConnection(DMSDBEntities db)
        {
            this.db = (SqlConnection)db.Database.Connection;
        }

        public void Open()
        {
            db.Open();
        }

        public void Close()
        {
            db.Close();
        }

        public void Dispose()
        {
            this.Close();
        }

        protected ClassicRepositoryBase(DMSDBEntities db)
        {
            GetSQLConnection(db);
        }
    }
}
