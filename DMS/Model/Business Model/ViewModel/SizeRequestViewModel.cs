using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Domain_Model;

namespace Model.Business.ViewModel
{
    public class SizeRequestViewModel
    {
        public int RequestID { get; set; }
        [Display(Name = "User")]
        [Required]
        public string Username { get; set; }
        [Display(Name = "User's email")]
        public string UserEmail { get; set; }
        [Display(Name = "Actual Storage Size")]
        public int CurrentSize { get; set; }
        [Display(Name = "Additional Size Request")]
        [Required]
        [Range(1, 10)]
        public int AdditionalSize { get; set; }
        [Display(Name = "Status")]
        public RequestStatus RequestStatus { get; set; }
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
    }
}
