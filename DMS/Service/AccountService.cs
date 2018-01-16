using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Repository.UnitOfWork;
using Helpers.Security;
using System.Web.Security;
using Model.Business.Session;
using Model.Business.ViewModel;
using Model.Business.Converter;
using System.IO;
using Helpers.Web;
using Model.Domain_Model;

namespace Service
{
    public static class AccountService
    {
        public static void Login(string username, string password)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var user = db.UserRepository.Query().Where(u => (u.Username == username || u.Username == u.Email) && u.Active == true).FirstOrDefault();
                if (PasswordHelper.ComputePassword(password, user.Salt) != user.Password)
                {
                    throw new Exception("Invalid credentials");
                }

                FormsAuthentication.SetAuthCookie(user.Username, false);
                UserPayload.UserID = user.UserID;
                UserPayload.UserPath = "/" + user.Username;
                UserPayload.UserType = user.Type;
                UserPayload.UserEmail = user.Email;
                FileAccessService.GetAccessList();
            }
        }

        public static void ForceLogin(string username)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var user = db.UserRepository.Query().Where(u => u.Username == username).FirstOrDefault();
                FormsAuthentication.SetAuthCookie(user.Username, false);
                UserPayload.UserID = user.UserID;
                UserPayload.UserPath = "/" + user.Username;
                UserPayload.UserType = user.Type;
                UserPayload.UserEmail = user.Email;
                FileAccessService.GetAccessList();
            }
        }

        public static void Logout()
        {
            FormsAuthentication.SignOut();
            UserPayload.UserID = -1;
            UserPayload.UserPath = string.Empty;
            UserPayload.UserType = string.Empty;
            UserPayload.UserEmail = string.Empty;
            UserPayload.FileAccessList = null;
            UserPayload.ForgetStamp = string.Empty;
        }

        public static void Register(UserViewModel vm)
        {
            try
            {
                using (var db = UnitOfWorkFactory.Create())
                {
                    if (vm.Password != vm.RePassword)
                    {
                        throw new Exception("Passwords dont match");
                    }
                    var user = UserConverter.ViewToDomain(vm);
                    user.Salt = PasswordHelper.GenerateSalt();
                    user.Password = PasswordHelper.ComputePassword(user.Password, user.Salt);
                    user.StorageSize = 15;
                    user.Type = "U";
                    user.Active = false;
                    db.UserRepository.Add(user);

                    Directory.CreateDirectory(DirectoryHelper.GetRootStorage() + "/" + user.Username);
                    MailService.SendMail(user.Email, string.Format("<p>Please visit the following link to activate your account<br/> <a href=\"{0}/Account/Activate/{1}\">{0}/Account/Activate/{1}</a></p>", DirectoryHelper.GetWebHostUrl(), user.Username),
                        isHtml: true, subject: "DMS - Activation");

                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                RemoveNewDirectory(vm.Username);
                throw new Exception(ex.Message);
            }
        }

        private static void RemoveNewDirectory(string username)
        {
            try
            {
                using (var db = UnitOfWorkFactory.Create())
                {
                    var user = db.UserRepository.Query().Where(u => u.Username == username).FirstOrDefault();
                    if (user == null)//User nuk eshte register ne db por eshte krijuar storage root
                    {
                        if (Directory.Exists(DirectoryHelper.GetRootStorage() + "/" + username))
                        {
                            Directory.Delete(DirectoryHelper.GetRootStorage() + "/" + username);
                        }
                    }
                }
            }
            catch { }
        }

        public static void ActDeactAccount(string username, bool active)
        {
            try
            {
                using (var db = UnitOfWorkFactory.Create())
                {
                    var user = db.UserRepository.Query().Where(u => u.Username == username).FirstOrDefault();
                    user.Active = active;
                    db.UserRepository.Update(user);
                    db.Commit();
                }
            }
            catch
            {
                throw new Exception("Could not change the state");
            }
        }

        public static List<User> ListUsers(bool alsoAdmin = false)
        {
            try
            {
                using (var db = UnitOfWorkFactory.Create())
                {
                    return (alsoAdmin ?
                        db.UserRepository.Query().ToList() :
                        db.UserRepository.Query().Where(u => u.Type != "A").AsParallel().ToList());
                }
            }
            catch
            {
                throw new Exception("Could not change the state");
            }
        }

        public static void ChangePassword(string username, string oldpassword, string newpassword)
        {
            try
            {
                using (var db = UnitOfWorkFactory.Create())
                {
                    var user = db.UserRepository.Query().Where(u => u.Username == username).FirstOrDefault();
                    if (user.Password == PasswordHelper.ComputePassword(oldpassword, user.Salt))
                    {
                        user.Password = PasswordHelper.ComputePassword(newpassword, user.Salt);
                        db.UserRepository.Update(user);
                        db.Commit();
                    }
                    else
                    {
                        throw new Exception("Password doesn't match");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void ForgetPassword(string username)
        {
            try
            {
                using (var db = UnitOfWorkFactory.Create())
                {
                    var user = db.UserRepository.Query().Where(u => u.Username == username || u.Email == username).FirstOrDefault();
                    UserPayload.ForgetStamp = PasswordHelper.GenerateSalt();
                    UserPayload.UserEmail = user.Email;
                    MailService.SendMail(user.Email, string.Format("<p>Please visit the following link to reset your password<br/> <a href=\"{0}/Account/ForgetRenew/{1}?Stamp={2}\">{0}/Account/Forget/{1}?Stamp={2}</a></p>",
                        DirectoryHelper.GetWebHostUrl(), user.Username,
                        UserPayload.ForgetStamp),
                        isHtml: true, subject: "DMS - Forget Password");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteUser(int id)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var user = db.UserRepository.GetByID(id);
                string email = user.Email;
                string dir = DirectoryHelper.GetRootStorage() + "/" + user.Username;
                db.UserRepository.Delete(user);
                db.Commit();

                try
                {
                    if (Directory.GetFiles(dir).Length == 0)
                        Directory.Delete(dir, true);
                    MailService.SendMail(user.Email, "<p>Your account has been deleted</p>",
                        isHtml: true, subject: "DMS - Activation");
                }
                catch { }
            }
        }

        public static void CheckForgetStamp(string stamp)
        {
            if (UserPayload.ForgetStamp != stamp)
            {
                throw new Exception("Bad request");
            }
            else
            {
                UserPayload.ForgetStamp = string.Empty;
            }
        }

        public static void RenewPassword(string username, string newpassword)
        {
            try
            {
                using (var db = UnitOfWorkFactory.Create())
                {
                    var user = db.UserRepository.Query().Where(u => u.Username == username || u.Email == username).FirstOrDefault();
                    user.Password = PasswordHelper.ComputePassword(newpassword, user.Salt);
                    db.UserRepository.Update(user);
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void ChangeEmail(string email)
        {
            try
            {
                using (var db = UnitOfWorkFactory.Create())
                {
                    var user = db.UserRepository.GetByID(UserPayload.UserID);
                    user.Email = email;
                    db.UserRepository.Update(user);
                    db.Commit();
                    UserPayload.UserEmail = email;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
