using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application.Security;
using Service;
using Model.Domain_Model;

namespace Application.Controllers
{
    public class AdminController : Controller
    {
        [ExtendedAuthorize(Roles = "A")]
        public ActionResult Index()
        {
            try
            {
                var model = AccountService.ListUsers();
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [HttpGet]
        [ExtendedAuthorize(Roles = "A")]
        public ActionResult SizeRequests()
        {
            try
            {
                var model = SizeRequestService.GroupedRequests();
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [ExtendedAuthorize(Roles = "A")]
        public ActionResult AcceptSize(int id)
        {
            try
            {
                SizeRequestService.Respond(id, Request.QueryString["email"], RequestStatus.Accepted);
                return RedirectToAction("SizeRequests");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [ExtendedAuthorize(Roles = "A")]
        public ActionResult DenySize(int id)
        {
            try
            {
                SizeRequestService.Respond(id, Request.QueryString["email"], RequestStatus.Denied);
                return RedirectToAction("SizeRequests");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }
    }
}