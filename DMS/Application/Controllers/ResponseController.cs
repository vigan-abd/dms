using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class ResponseController : Controller
    {
        // GET: Response
        public ActionResult Index()
        {
            ViewBag.Type = Request.QueryString["Type"];
            ViewBag.Code = Request.QueryString["Code"];
            ViewBag.Message = Request.QueryString["Message"];
            return View();
        }
    }
}