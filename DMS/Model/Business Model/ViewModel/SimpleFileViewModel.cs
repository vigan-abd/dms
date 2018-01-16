using Model.Domain_Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Model.Business.ViewModel
{
    public class SimpleFileViewModel
    {
        public SimpleFileViewModel()
        {

        }
        public int FileID { get; set; }
        public int OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string Title { get; set; }
        public string ShortDesc { get; set; }
        public int LastVersion { get; set; }
        public DateTime LastModified { get; set; }
        public List<int> Versions { get; set; }
        public SearchScore Score { get; set; }
    }

    public class SearchScore
    {
        public double KCR { get; set; }
        public double WCR { get; set; }
        public double Total { get; set; }
    }
}
