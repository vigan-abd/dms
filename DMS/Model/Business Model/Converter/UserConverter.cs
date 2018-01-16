using Model.Business.ViewModel;
using Model.Domain_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Business.Converter
{
    public static class UserConverter
    {
        public static User ViewToDomain(UserViewModel u)
        {
            return new User()
            {
                Username = u.Username,
                Password = u.Password,
                Type = u.Type,
                Email = u.Email
            };
        }
    }
}
