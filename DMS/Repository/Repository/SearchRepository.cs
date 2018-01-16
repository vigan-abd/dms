using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Domain_Model;
using Model.Business.ViewModel;
using System.Configuration;
using System.Data.SqlClient;
using Model.Business.Converter;

namespace Repository
{
    public class SearchRepository : ClassicRepositoryBase
    {
        public SearchRepository(DMSDBEntities db) : base(db)
        {
        }

        public List<SimpleFileViewModel> SpAtLeastOnceKeyword(string term)
        {
            List<SimpleFileViewModel> list = new List<SimpleFileViewModel>();
            using (SqlCommand cmd = new SqlCommand("spAtLeastOnceKeyword", db))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Keywords",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = term
                });
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(SearchConverter.ReaderToViewModel(dr));
                    }
                }
            }
            return list;
        }
    }
}
