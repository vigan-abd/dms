using Model.Domain_Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IClassicRepository : IDisposable
    {
        void GetSQLConnection(DMSDBEntities db);
        void Open();
        void Close();
    }
}
