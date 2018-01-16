using Application.Security;
using Model.Business.Session;
using Model.Business.ViewModel;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class UserStorageController : Controller
    {
        [HttpGet]
        [ExtendedAuthorize(Roles = "U")]
        public ActionResult Index()
        {
            var dir = (Request.QueryString["Path"] != null ?
                Request.QueryString["Path"] :
                UserPayload.UserPath);
            var model = UserStorageService.ReadDirectory(dir, User.Identity.Name);
            ViewBag.CurrentDir = dir;
            ViewBag.Size = UserStorageService.ReadStorageSize();
            return View(model);
        }

        [HttpGet]
        [ExtendedAuthorize(Roles = "U")]
        public ActionResult SizeRequest()
        {
            var model = new SizeRequestViewModel()
            {
                Username = User.Identity.Name
            };
            return View(model);
        }

        [HttpPost]
        [ExtendedAuthorize(Roles = "U")]
        [ValidateAntiForgeryToken]
        public ActionResult SizeRequest([Bind(Include = "Username, AdditionalSize")]SizeRequestViewModel vm)
        {
            try
            {
                SizeRequestService.Request(vm.Username, vm.AdditionalSize);
                return RedirectToAction("Index", "Response", new { Message = "Request send successfully", Code = 200, Type = "Success" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [ExtendedAuthorize(Roles = "U")]
        public ActionResult Single(int id)
        {
            try
            {
                List<SelectListItem> userList;
                var model = UserStorageService.GetFileViewByID(id, out userList);
                ViewBag.UserList = userList;
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [HttpGet]
        [ExtendedAuthorize(Roles = "U")]
        public ActionResult InsertFile()
        {
            ViewBag.UserList = UserStorageService.LoadUserSharableList(UserPayload.UserID);
            return View(new FileViewModel());
        }

        [HttpPost]
        [ExtendedAuthorize(Roles = "U")]
        [ValidateAntiForgeryToken]
        public ActionResult InsertFile([Bind(Include = "Title, ShortDesc, AccessLevel, ShareList, Keywords")]FileViewModel vm)
        {
            try
            {
                var file = Request.Files["file"] as HttpPostedFileBase;
                vm.LastModified = DateTime.Now;
                vm.RelativeDirectory = UserPayload.UserPath;
                vm.LastVersion = 1;
                if (file != null && file.ContentLength != 0)
                {
                    UserStorageService.InsertFile(vm, file);
                    return RedirectToAction("Index", "Response", new { Message = "File has been inserted succesfully", Code = 200, Type = "Success" });
                }
                else
                {
                    throw new Exception("Please fill all fields");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [HttpPost]
        [ExtendedAuthorize(Roles = "U")]
        public string Delete()
        {
            try
            {
                int id = int.Parse(Request.Form["id"]);
                UserStorageService.DeleteFile(id);
                return "true";
            }
            catch
            {
                return "false";
            }
        }

        [HttpPost]
        [ExtendedAuthorize(Roles = "U")]
        [ValidateAntiForgeryToken]
        public ActionResult EditFileInfo([Bind(Include = "FileID, Title, ShortDesc, AccessLevel, ShareList, Keywords")]FileViewModel vm)
        {
            try
            {
                UserStorageService.UpdateFileInfo(vm);
                return RedirectToAction("Single", new { id = vm.FileID });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [HttpGet]
        [ExtendedAuthorize(Roles = "U")]
        public ActionResult ViewVersion()
        {
            try
            {
                int fileID = int.Parse(Request.QueryString["fileID"]);
                int version = int.Parse(Request.QueryString["version"]);
                string filename;
                byte[] file = UserStorageService.ReadFileVersion(fileID, version, out filename);
                return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [HttpGet]
        [ExtendedAuthorize(Roles = "U")]
        public ActionResult AddVersion()
        {
            var model = UserStorageService.PrepareVersion(int.Parse(Request.QueryString["fileID"]));
            return View(model);
        }

        [HttpPost]
        [ExtendedAuthorize(Roles = "U")]
        [ValidateAntiForgeryToken]
        public ActionResult AddVersion([Bind(Include = "FileID, VerNo, Name, RelativeDirectory")]FileVersionViewModel vm)
        {
            try
            {
                var file = Request.Files["file"] as HttpPostedFileBase;
                if (file != null && file.ContentLength != 0)
                {
                    UserStorageService.InsertFileVersion(vm, file);
                    return RedirectToAction("Single", new { id = vm.FileID });
                }
                else
                {
                    throw new Exception("Please fill all fields");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [HttpGet]
        [ExtendedAuthorize(Roles = "U")]
        public ActionResult EditVersion()
        {
            int fileID = int.Parse(Request.QueryString["fileID"]);
            int version = int.Parse(Request.QueryString["version"]);
            var model = UserStorageService.PrepareVersion(fileID, version);
            return View(model);
        }

        [HttpPost]
        [ExtendedAuthorize(Roles = "U")]
        [ValidateAntiForgeryToken]
        public ActionResult EditVersion([Bind(Include = "FileID, VerNo, Name, RelativeDirectory")]FileVersionViewModel vm)
        {
            try
            {
                var file = Request.Files["file"] as HttpPostedFileBase;
                if (file != null && file.ContentLength != 0)
                {
                    UserStorageService.EditFileVersion(vm, file);
                    return RedirectToAction("Single", new
                    {
                        id = vm.FileID
                    });
                }
                else
                {
                    throw new Exception("Please fill all fields");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Response", new { Message = ex.Message, Code = 400, Type = "Error" });
            }
        }

        [HttpPost]
        [ExtendedAuthorize(Roles = "U")]
        public string DeleteVersion()
        {
            try
            {
                int fileid = int.Parse(Request.Form["id"]);
                int version = int.Parse(Request.Form["version"]);
                UserStorageService.DeleteFileVersion(fileid, version);
                return "true";
            }
            catch
            {
                return "false";
            }
        }

        [HttpPost]
        [ExtendedAuthorize(Roles = "U")]
        public string RenewExternalLink(int fileID)
        {
            try
            {
                UserStorageService.RenewExternalLink(fileID);
                return "true";
            }
            catch
            {
                return "false";
            }
        }
    }
}