using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Domain_Model;

namespace Repository
{
    public abstract class RepositoryBase
    {
        protected DMSDBEntities db;
        protected RepositoryBase(DMSDBEntities db)
        {
            this.db = db;
        }
    }
}
