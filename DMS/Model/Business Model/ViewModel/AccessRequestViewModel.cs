using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Domain_Model;

namespace Model.Business.ViewModel
{
    public class AccessRequestViewModel
    {
        public int RequestID { get; set; }
        public int FileID { get; set; }
        public int UserID { get; set; }
        [Display(Name = "Owner")]
        public string Owner { get; set; }
        [Display(Name = "Requester")]
        public string Requester { get; set; }
        [Display(Name = "Status")]
        public RequestStatus RequestStatus { get; set; }
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
    }
}
