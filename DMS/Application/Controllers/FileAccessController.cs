using Application.Security;
using Model.Domain_Model;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class FileAccessController : Controller
    {
        [HttpGet]
        //[ExtendedAuthorize(Roles = "U")]
        public ActionResult Index()
        {
            var model = FileAccessService.ReadAccessList();
            return View(model);
        }

        [HttpGet]
        [ExtendedAuthorize(Roles = "U")]
        public ActionResult Read(int fileid, int version)
        {
            try
            {
                string filename;
                byte[] file = FileAccessService.ReadFile(fileid, version, out filename);
                return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }
        
        public ActionResult ReadExternal(string id, int v)
        {
            try
            {
                string filename;
                byte[] file = FileAccessService.ReadFile(id, v, out filename);
                return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = "Access Denied", Code = 400, Type = "Error" });
            }
        }

        [HttpGet]
        [ExtendedAuthorize(Roles = "U")]
        public ActionResult AccessRequests()
        {
            var model = FileAccessService.RequestList();
            return View(model);
        }

        [HttpGet]
        [ExtendedAuthorize(Roles = "U")]
        public ActionResult UserAccessRequests()
        {
            var model = FileAccessService.UserRequestList();
            return View(model);
        }

        [HttpGet]
        [ExtendedAuthorize(Roles = "U")]
        public ActionResult AccessRequest(int fileid, int ownerID, int rUserID)
        {
            try
            {
                FileAccessService.RequestAccess(fileid, ownerID, rUserID);
                return RedirectToAction("Index", "Response", new { Message = "Request send successfully", Code = 200, Type = "Success" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [HttpPost]
        [ExtendedAuthorize(Roles ="U")]
        public string AcceptRequest(int fileid, int rUserID)
        {
            try
            {
                FileAccessService.AccessResponse(fileid, rUserID, RequestStatus.Accepted);
                return "true";
            }
            catch
            {
                return "false";
            }
        }

        [HttpPost]
        [ExtendedAuthorize(Roles = "U")]
        public string DenyRequest(int fileid, int rUserID)
        {
            try
            {
                FileAccessService.AccessResponse(fileid, rUserID, RequestStatus.Denied);
                return "true";
            }
            catch
            {
                return "false";
            }
        }
    }
}