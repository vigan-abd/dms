using Model.Business.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Repository.UnitOfWork;
using Helpers.Web;
using System.Web;
using Model.Business.Converter;
using Model.Business.Session;
using System.Web.Mvc;
using Model.Domain_Model;
using Helpers.Security;

namespace Service
{
    public static class UserStorageService
    {
        public static string GenerateExternalLink(string fileTitle)
        {
            return PasswordHelper.ComputeHash(fileTitle +
                PasswordHelper.GenerateSalt() +
                DateTime.Now.Millisecond);
        }
        public static Tuple<double, double> ReadStorageSize()
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var c = DirectoryHelper.DirectorySize(UserPayload.UserPath);
                double used = Math.Round(DirectoryHelper.DirectorySize(UserPayload.UserPath) / 1000000000, 10);
                double total = db.UserRepository.GetByID(UserPayload.UserID).StorageSize;
                return new Tuple<double, double>(used, total);
            }
        }

        public static void CheckFileAccess(int userID)
        {
            if (userID != UserPayload.UserID)
                throw new Exception("Access Denied to file for user");
        }

        public static void CheckIfFileExists(string title, int userid, ref UnitOfWork db)
        {
            var list = db.FileRepository.Query().
                Where(f => f.Title.ToLower() == title.ToLower() && f.UserID == userid).ToList();
            if (list.Count > 0)
                throw new Exception("File already exists on storage");
        }

        public static MultiSelectList LoadUserSharableList(int currentUserID)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var users = db.UserRepository.Query().Where(u => u.Type != "A" && u.UserID != currentUserID).Select(u => new { u.UserID, u.Username }).ToList();
                return new MultiSelectList(users, "UserID", "Username");
            }
        }

        public static FileViewModel GetFileViewByID(int fileID, out List<SelectListItem> userList)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var model = db.FileRepository.GetByID(fileID);
                userList = new List<SelectListItem>();
                var accessUsers = model.AccessUsers.Select(u => u.UserID).ToList();
                foreach (var item in model.AccessUsers)
                {
                    userList.Add(new SelectListItem()
                    {
                        Selected = true,
                        Text = item.Username,
                        Value = item.UserID.ToString()
                    });
                }
                var allUsers = db.UserRepository.Query().
                    Where(u => !accessUsers.Contains(u.UserID) && u.UserID != model.UserID && u.Type != "A").
                    Select(u => new SelectListItem()
                    {
                        Selected = false,
                        Text = u.Username,
                        Value = u.UserID.ToString()
                    }).ToList();
                userList.AddRange(allUsers);

                var vm = FileConverter.FileToViewModel(model);
                vm.Versions = model.FileVersions;
                return vm;
            }
        }

        public static FileVersionViewModel PrepareVersion(int fileID)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var file = db.FileRepository.GetByID(fileID);
                int version = file.FileVersions.OrderByDescending(f => f.VerNo).First().VerNo + 1;
                FileVersionViewModel vm = new FileVersionViewModel()
                {
                    VerNo = version,
                    FileID = file.FileID,
                    RelativeDirectory = file.RelativeDirectory,
                    Name = file.Title
                };
                return vm;
            }
        }
        public static FileVersionViewModel PrepareVersion(int fileID, int version)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var fileVer = db.FileVersionRepository.Query()
                    .Where(f => f.FileID == fileID && f.VerNo == version).FirstOrDefault();
                var vm = FileConverter.FileVerToViewModel(fileVer);
                return vm;
            }
        }

        public static UserStorageViewModel ReadDirectory(string dir, string username)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var storage = new UserStorageViewModel() { Files = new List<FileViewModel>() };
                try
                {
                    var files = db.UserRepository.Query().Where(u => u.Username == username).First().
                        Files.Where(f => f.RelativeDirectory.ToLower() == dir.ToLower()).AsParallel().ToList();
                    for (int i = 0; i < files.Count; i++)
                    {
                        storage.Files.Add(new FileViewModel()
                        {
                            FileID = files[i].FileID,
                            RelativeDirectory = dir,
                            Title = files[i].Title
                        });
                    }
                }
                catch { }

                return storage;
            }
        }

        public static void InsertFile(FileViewModel vm, HttpPostedFileBase file)
        {
            try
            {
                var db = UnitOfWorkFactory.Create();
                CheckIfFileExists(vm.Title, UserPayload.UserID, ref db);

                Model.Domain_Model.File model = FileConverter.ViewModelToFile(vm);
                string filename = model.Title + "-v1." + DirectoryHelper.Extension(file.FileName);
                model.UserID = UserPayload.UserID;
                var selectedUsers = (vm.ShareList != null ?
                db.GeneralRepository.GetUsersFromIDCollection(vm.ShareList.ToList()) :
                new List<User>());
                model.AccessUsers = new List<User>();
                for (int i = 0; i < selectedUsers.Count; i++)
                {
                    model.AccessUsers.Add(selectedUsers[i]);
                }
                model.FileVersions = new List<FileVersion>();
                model.FileVersions.Add(new FileVersion() { VerNo = 1, Name = filename });
                model.LastVersion = 1;
                model.Owner = db.UserRepository.GetByID(UserPayload.UserID);
                model.ExternalLink = GenerateExternalLink(model.Title);

                if (file.ContentLength + DirectoryHelper.DirectorySize(UserPayload.UserPath) > DirectoryHelper.GBtoByte(model.Owner.StorageSize))
                {
                    throw new Exception("File size exeeds user's storage size");
                }

                FileEncryptor.EncryptFile(DirectoryHelper.GetRootStorage() + model.RelativeDirectory
                    + "/" + filename,
                    file.InputStream, file.ContentLength);
                db.FileRepository.Add(model);

                db.Commit();
                db.Dispose();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateFileInfo(FileViewModel vm)
        {
            var db = UnitOfWorkFactory.Create();
            var model = db.FileRepository.GetByID(vm.FileID);
            CheckFileAccess(model.Owner.UserID);
            model.Keywords = vm.Keywords;
            if (vm.Title.ToLower() != model.Title.ToLower())
            {
                CheckIfFileExists(vm.Title, UserPayload.UserID, ref db);
                foreach (var filever in model.FileVersions)
                {
                    DirectoryHelper.RenameFile(filever.Name,
                        vm.Title + "-v" + filever.VerNo + "." + DirectoryHelper.Extension(filever.Name),
                        DirectoryHelper.GetRootStorage() + model.RelativeDirectory);
                    filever.Name = vm.Title + "-v" + filever.VerNo + "." + DirectoryHelper.Extension(filever.Name);
                }
            }
            model.Title = vm.Title;
            model.ShortDesc = vm.ShortDesc;
            model.AccessLevel = vm.AccessLevel;
            model.LastModified = DateTime.Now;

            model.AccessUsers.Clear();

            var selectedUsers = (vm.ShareList != null ?
                db.GeneralRepository.GetUsersFromIDCollection(vm.ShareList.ToList()) :
                new List<User>());
            model.AccessUsers = new List<User>();
            for (int i = 0; i < selectedUsers.Count; i++)
            {
                if (!model.AccessUsers.Contains(selectedUsers[i]))
                    model.AccessUsers.Add(selectedUsers[i]);
            }

            db.FileRepository.Update(model);
            db.Commit();

            try
            {
                MailService.SendMail("", string.Format("<p>Document {0} has been modified</p>", vm.Title),
                    isHtml: true, disableTo: true,
                    bccList: model.AccessUsers.Select(u => u.Email).ToArray());
            }
            catch { }
            db.Dispose();
        }

        public static void DeleteFile(int fileID)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var file = db.FileRepository.GetByID(fileID);
                CheckFileAccess(file.Owner.UserID);
                string dir = DirectoryHelper.GetRootStorage() + UserPayload.UserPath;
                var versions = file.FileVersions;
                foreach (var item in versions)
                {
                    DirectoryHelper.DeleteFile(dir, item.Name);
                }
                db.FileRepository.Delete(file.FileID);
                db.Commit();

                try
                {
                    MailService.SendMail("", string.Format("<p>Document {0} has been modified</p>", file.Title),
                        isHtml: true, disableTo: true,
                        bccList: file.AccessUsers.Select(u => u.Email).ToArray());
                }
                catch { }
            }
        }

        public static void InsertFileVersion(FileVersionViewModel vm, HttpPostedFileBase file)
        {
            try
            {
                using (var db = UnitOfWorkFactory.Create())
                {
                    var model = db.FileRepository.GetByID(vm.FileID);

                    if (file.ContentLength + DirectoryHelper.DirectorySize(UserPayload.UserPath) > DirectoryHelper.GBtoByte(model.Owner.StorageSize))
                    {
                        throw new Exception("File size exeeds user's storage size");
                    }

                    model.LastModified = DateTime.Now;
                    var fileVersion = FileConverter.ViewModelToFileVer(vm);
                    fileVersion.Name = fileVersion.Name + "-v" + fileVersion.VerNo
                        + "." + DirectoryHelper.Extension(file.FileName);

                    if (file.ContentLength + DirectoryHelper.DirectorySize(UserPayload.UserPath) > DirectoryHelper.GBtoByte(model.Owner.StorageSize))
                    {
                        throw new Exception("File size exeeds user's storage size");
                    }

                    FileEncryptor.EncryptFile(DirectoryHelper.GetRootStorage() + model.RelativeDirectory
                        + "/" + fileVersion.Name,
                        file.InputStream, file.ContentLength);
                    model.FileVersions.Add(fileVersion);
                    db.FileRepository.Update(model);
                    db.FileVersionRepository.Add(fileVersion);
                    db.Commit();

                    try
                    {
                        MailService.SendMail("", string.Format("<p>Document {0} has been modified</p>", model.Title),
                            isHtml: true, disableTo: true,
                            bccList: model.AccessUsers.Select(u => u.Email).ToArray());
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void EditFileVersion(FileVersionViewModel vm, HttpPostedFileBase file)
        {
            try
            {
                using (var db = UnitOfWorkFactory.Create())
                {
                    var filemodel = db.FileRepository.GetByID(vm.FileID);
                    CheckFileAccess(filemodel.Owner.UserID);

                    if (file.ContentLength + DirectoryHelper.DirectorySize(UserPayload.UserPath) > DirectoryHelper.GBtoByte(filemodel.Owner.StorageSize))
                    {
                        throw new Exception("File size exeeds user's storage size");
                    }

                    filemodel.LastModified = DateTime.Now;
                    var filename = filemodel.Title;
                    var fileversion = FileConverter.ViewModelToFileVer(vm);
                    fileversion.Name = filename + "-v" + fileversion.VerNo
                        + "." + DirectoryHelper.Extension(file.FileName);
                    DirectoryHelper.DeleteFile(DirectoryHelper.GetRootStorage() + vm.RelativeDirectory, vm.Name);
                    FileEncryptor.EncryptFile(DirectoryHelper.GetRootStorage() + vm.RelativeDirectory
                        + "/" + fileversion.Name,
                        file.InputStream, file.ContentLength);
                    db.FileVersionRepository.Update(fileversion);
                    db.FileRepository.Update(filemodel);
                    db.Commit();

                    try
                    {
                        MailService.SendMail("", string.Format("<p>Document {0} has been modified</p>", filemodel.Title),
                            isHtml: true, disableTo: true,
                            bccList: filemodel.AccessUsers.Select(u => u.Email).ToArray());
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteFileVersion(int fileID, int version)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var model = db.FileVersionRepository.Query().
                    Where(f => f.FileID == fileID && f.VerNo == version).FirstOrDefault();
                string filename = model.Name;
                string dir = DirectoryHelper.GetRootStorage() + model.File.RelativeDirectory;
                db.FileVersionRepository.Delete(model);
                var file = db.FileRepository.GetByID(fileID);
                CheckFileAccess(file.Owner.UserID);
                file.LastVersion = file.FileVersions.Where(f => f.VerNo != version)
                    .Max(f => f.VerNo);
                file.LastModified = DateTime.Now;
                db.FileRepository.Update(file);
                db.Commit();
                DirectoryHelper.DeleteFile(dir, filename);
                try
                {
                    MailService.SendMail("", string.Format("<p>Document {0} has been modified</p>", file.Title),
                        isHtml: true, disableTo: true,
                        bccList: file.AccessUsers.Select(u => u.Email).ToArray());
                }
                catch { }
            }
        }

        public static byte[] ReadFileVersion(int fileID, int version, out string filename)
        {
            filename = "temp";
            using (var db = UnitOfWorkFactory.Create())
            {
                var file = db.FileRepository.GetByID(fileID);
                CheckFileAccess(file.Owner.UserID);
                var name = file.FileVersions.Where(f => f.FileID == fileID && f.VerNo == version).Select(f => f.Name).FirstOrDefault();
                filename = name;
                return FileEncryptor.DecryptFile(DirectoryHelper.GetRootStorage() + file.RelativeDirectory
                    + "/" + name);
            }
        }

        public static void RenewExternalLink(int fileID)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var file = db.FileRepository.GetByID(fileID);
                CheckFileAccess(file.Owner.UserID);
                file.ExternalLink = GenerateExternalLink(file.Title);
                db.FileRepository.Update(file);
                db.Commit();
            }
        }
    }
}
