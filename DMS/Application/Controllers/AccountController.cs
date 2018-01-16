using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Application.Models;
using Service;
using Model.Domain_Model;
using System.Web.Security;
using Model.Business.Session;
using Model.Business.ViewModel;
using System.Collections.Generic;
using Application.Security;

namespace Application.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [ExtendedAuthorize]
        public ActionResult Index()
        {
            return View(new UserViewModel() { Email = UserPayload.UserEmail });
        }

        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.RemoveMenuItems = true;
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Username, Password")] UserViewModel user)
        {
            try
            {
                AccountService.Login(user.Username, user.Password);
                if (UserPayload.UserType == "A")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = "Authentification failed", Code = 400, Type = "Error" });
            }
        }

        public ActionResult Logout()
        {
            AccountService.Logout();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Username, Email, Password, RePassword")] UserViewModel user)
        {
            try
            {
                AccountService.Register(user);
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        public ActionResult Activate(string id)
        {
            try
            {
                AccountService.ActDeactAccount(id, true);
                AccountService.ForceLogin(id);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [ExtendedAuthorize(Roles ="A")]
        public ActionResult AdminActivate(string id)
        {
            try
            {
                AccountService.ActDeactAccount(id, true);
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [ExtendedAuthorize(Roles = "A")]
        public ActionResult AdminDeactivate(string id)
        {
            try
            {
                AccountService.ActDeactAccount(id, false);
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [HttpPost]
        [ExtendedAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "RePassword, Password")] UserViewModel user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.RePassword) || user.RePassword.Trim().Length < 6)
                    throw new Exception("Password must be at least 6 charcters long");
                AccountService.ChangePassword(User.Identity.Name, user.Password, user.RePassword);
                return RedirectToAction("Index", "Response",
                    new { Message = "Password changed", Code = 200, Type = "Success" });
            }
            catch
            {
                return RedirectToAction("Index", "Response",
                    new { Message = "Could not change password", Code = 400, Type = "Error" });
            }
        }

        [HttpPost]
        [ExtendedAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEmail([Bind(Include = "Email")] UserViewModel user)
        {
            try
            {
                AccountService.ChangeEmail(user.Email);
                return RedirectToAction("Index", "Response",
                    new { Message = "Email changed", Code = 200, Type = "Success" });
            }
            catch
            {
                return RedirectToAction("Index", "Response",
                    new { Message = "Could not change email", Code = 400, Type = "Error" });
            }
        }

        [HttpGet]
        public ActionResult ForgetPassword()
        {
            return View("ForgetPasswordRequest");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPassword([Bind(Include ="Username")] UserViewModel user)
        {
            try
            {
                AccountService.ForgetPassword(user.Username);
                return RedirectToAction("Index", "Response",
                    new { Message = "Email sent to your account", Code = 200, Type = "Success" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response",
                    new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        public ActionResult ForgetRenew(string id)
        {
            try
            {
                string stamp = Request.QueryString["Stamp"] as string;
                AccountService.CheckForgetStamp(stamp);
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response",
                    new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetRenew([Bind(Include ="RePassword")] UserViewModel user)
        {
            try
            {
                AccountService.RenewPassword(UserPayload.UserEmail, user.RePassword);
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response",
                    new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [HttpPost]
        [ExtendedAuthorize(Roles = "A")]
        public string DeleteUser(int id)
        {
            try
            {
                AccountService.DeleteUser(id);
                return "true";
            }
            catch (Exception ex)
            {
                return "false";
            }
        }

        public ActionResult Unauthorize()
        {
            return RedirectToAction("Index", "Response",
                new { Message = "Unauthorized access", Code = 401, Type = "Error" });
        }
    }
}