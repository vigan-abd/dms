using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Business.ViewModel
{
    public class UserViewModel
    {
        [Required]
        [MinLength(4)]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        [Display(Name = "New Password")]
        [MinLength(6)]
        public string RePassword { get; set; }
        [Required]
        public string Type { get; set; }
    }
}
