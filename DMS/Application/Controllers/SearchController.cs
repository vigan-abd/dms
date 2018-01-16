using Application.Security;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        [HttpGet]
        [ExtendedAuthorize(Roles = "U")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ExtendedAuthorize(Roles = "U")]
        public ActionResult Search(string term)
        {
            try
            {
                var model = SearchService.Search(term);
                if (model == null || model.Count == 0)
                    throw new Exception("No results found!");
                return View(model);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }
    }
}