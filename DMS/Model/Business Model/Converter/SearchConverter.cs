using Model.Business.ViewModel;
using Model.Domain_Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Business.Converter
{
    public static class SearchConverter
    {
        public static SimpleFileViewModel ReaderToViewModel(IDataReader dr)
        {
            var file = new SimpleFileViewModel()
            {
                FileID = (int)dr["FileID"],
                OwnerID = (int)dr["UserID"],
                OwnerName = (string)dr["Username"],
                Title = (string)dr["Title"],
                LastModified = (DateTime)dr["LastModified"],
                LastVersion = (int)dr["LastVersion"],
                Score = new SearchScore(),
                Versions = new List<int>()
            };
            file.Score.KCR = (int)dr["KeywordCount"];

            string[] versions = ((string)dr["Versions"]).Split(',');
            for (int i = 0; i < versions.Length; i++)
            {
                file.Versions.Add(int.Parse(versions[i]));
            }
            return file;
        }
    }
}
