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
    public class FileVersionViewModel
    {
        [Required]
        [Display(Name = "Version")]
        public int VerNo { get; set; }
        [Required]
        public int FileID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string RelativeDirectory { get; set; }
    }
}
