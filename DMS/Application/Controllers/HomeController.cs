using System;
using System.Web.Mvc;
using Service;
using Model.Domain_Model;
using Application.Security;
using Model.Business.Session;

namespace Application.Controllers
{
    public class HomeController : Controller
    {
        [ExtendedAuthorize]
        public ActionResult Index()
        {
            if (UserPayload.UserType != "U")
                return RedirectToAction("Index", "Admin", null);
            return View();
        }
    }
}