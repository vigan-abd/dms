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
    public class FileViewModel
    {
        public FileViewModel()
        {

        }
        public int FileID { get; set; }
        [Required]
        public string RelativeDirectory { get; set; }
        [Required]
        [MinLength(3)]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Short description")]
        [MinLength(10)]
        public string ShortDesc { get; set; }
        [Display(Name = "Share")]
        [Required]
        public string AccessLevel { get; set; }
        public IEnumerable<SelectListItem> ShareTypes
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "Public", Value = "P"},
                    new SelectListItem() { Text = "List", Value = "L" },
                    new SelectListItem() { Text = "None", Value = "N" }
                };
            }
        }
        [Display(Name = "Select multiple users using CTRL (Win) or CMD (Mac)")]
        public IEnumerable<int> ShareList { get; set; }
        [Required]
        public int LastVersion { get; set; }
        [Required]
        [RegularExpression(@"^(([\w \#\!\@\$\%\^\&\*\(\)\-\\\/\+]+,)+([\w \#\!\@\$\%\^\&\*\(\)\-\\\/\+]+))|([\w \#\!\@\$\%\^\&\*\(\)\\\/\\\/\+]*)$", ErrorMessage = "Please enter , seperated words")]
        public string Keywords { get; set; }
        [Required]
        public DateTime LastModified { get; set; }
        public IEnumerable<FileVersion> Versions { get; set; }
        [Required]
        [Display(Name = "External Access Link")]
        public string ExternalLink { get; set; }
    }
}
