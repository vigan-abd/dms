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
    public static class FileAccessService
    {
        public static void GetAccessList()
        {
            if (UserPayload.FileAccessList == null)
            {
                using (var db = UnitOfWorkFactory.Create())
                {
                    UserPayload.FileAccessList = new SortedSet<int>();
                    var ownedlist = db.UserRepository.GetByID(UserPayload.UserID).
                        Files.Select(f => f.FileID).ToList();

                    var accessFilelist = db.FileRepository.Query()
                    .Where(f => f.AccessLevel == "L" && f.Owner.Active).Select(f => new { FileID = f.FileID, Users = f.AccessUsers.Select(u => u.UserID) }).ToList();

                    var accesslist = accessFilelist.Where(u => u.Users.Contains(UserPayload.UserID)).
                        Select(u => u.FileID).ToList();

                    var publicList = db.FileRepository.Query().
                        Where(f => f.AccessLevel == "P" && !ownedlist.Contains(f.FileID) && f.Owner.Active).
                        Select(f => f.FileID).AsParallel().Distinct().ToList();

                    var notOwnedFiles = db.FileRepository.Query().Where(f => f.UserID == null || f.UserID == 0)
                        .Select(f => f.FileID).ToList();

                    for (int i = 0; i < ownedlist.Count; i++)
                        UserPayload.FileAccessList.Add(ownedlist[i]);
                    for (int i = 0; i < publicList.Count; i++)
                        UserPayload.FileAccessList.Add(publicList[i]);
                    for (int i = 0; i < accesslist.Count; i++)
                        UserPayload.FileAccessList.Add(accesslist[i]);
                    for (int i = 0; i < notOwnedFiles.Count; i++)
                        UserPayload.FileAccessList.Add(notOwnedFiles[i]);
                }
            }
        }

        public static void CheckAccess(int fileID)
        {
            GetAccessList();
            if (!UserPayload.FileAccessList.Contains(fileID))
                throw new Exception("Access denied to user for file");
        }

        public static List<SimpleFileViewModel> ReadAccessList()
        {
            GetAccessList();
            List<SimpleFileViewModel> list = new List<SimpleFileViewModel>();
            using (var db = UnitOfWorkFactory.Create())
            {
                var accessList = UserPayload.FileAccessList;
                for (int i = 0; i < accessList.Count; i++)
                {
                    var file = db.FileRepository.GetByID(accessList.ElementAt(i));
                    var item = new SimpleFileViewModel()
                    {
                        FileID = file.FileID,
                        LastModified = file.LastModified,
                        LastVersion = file.LastVersion,
                        ShortDesc = file.ShortDesc,
                        Title = file.Title,
                        OwnerID = file.Owner.UserID,
                        OwnerName = file.Owner.Username
                    };
                    item.Versions = file.FileVersions.Select(f => f.VerNo).AsParallel().ToList();
                    list.Add(item);
                }
            }
            return list;
        }

        public static Dictionary<string, List<AccessRequestViewModel>> RequestList()
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var unsortedList = db.FileAccessRequestRepository.Query().
                    Where(r => r.File.Owner.UserID == UserPayload.UserID).ToList();
                return OrderAccessList(unsortedList);
            }
        }

        public static Dictionary<string, List<AccessRequestViewModel>> UserRequestList()
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var unsortedList = db.FileAccessRequestRepository.Query().
                    Where(r => r.UserID == UserPayload.UserID).ToList();
                return OrderAccessList(unsortedList);
            }
        }

        private static Dictionary<string, List<AccessRequestViewModel>> OrderAccessList(List<AccessRequest> unsortedList)
        {
            string accepted = RequestStatusConverter.ToDBEntity(RequestStatus.Accepted);
            string pending = RequestStatusConverter.ToDBEntity(RequestStatus.Pending);
            string denied = RequestStatusConverter.ToDBEntity(RequestStatus.Denied);

            var sortedList = new Dictionary<string, List<AccessRequestViewModel>>();
            sortedList.Add(RequestStatus.Accepted.ToString(), new List<AccessRequestViewModel>());
            sortedList.Add(RequestStatus.Pending.ToString(), new List<AccessRequestViewModel>());
            sortedList.Add(RequestStatus.Denied.ToString(), new List<AccessRequestViewModel>());

            var acceptedList = unsortedList.Where(r => r.Status == accepted)
                .OrderByDescending(r => r.Stamp).ToList();
            var pendingList = unsortedList.Where(r => r.Status == pending)
                .OrderByDescending(r => r.Stamp).ToList();
            var deniedList = unsortedList.Where(r => r.Status == denied)
                .OrderByDescending(r => r.Stamp).ToList();

            for (int i = 0; i < acceptedList.Count; i++)
                sortedList[RequestStatus.Accepted.ToString()].
                    Add(AccessRequestConverter.DomainToView(acceptedList[i]));
            for (int i = 0; i < pendingList.Count; i++)
                sortedList[RequestStatus.Pending.ToString()].
                    Add(AccessRequestConverter.DomainToView(pendingList[i]));
            for (int i = 0; i < deniedList.Count; i++)
                sortedList[RequestStatus.Denied.ToString()].
                    Add(AccessRequestConverter.DomainToView(deniedList[i]));

            return sortedList;
        }

        public static void RequestAccess(int fileid, int ownerID, int rUserID)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var model = new AccessRequest()
                {
                    FileID = fileid,
                    UserID = rUserID,
                    Stamp = DateTime.Now,
                    Status = RequestStatusConverter.ToDBEntity(RequestStatus.Pending)
                };

                db.FileAccessRequestRepository.Add(model);
                string email = db.UserRepository.GetByID(ownerID).Email;
                string requester = db.UserRepository.GetByID(rUserID).Username;
                string filename = db.FileRepository.GetByID(fileid).Title;

                db.Commit();

                MailService.SendMail(email, string.Format("<p>User {0} has requested access for file {1}</p>", requester, filename),
                    isHtml: true, subject: "DMS - File Access Request");

                db.Commit();
            }
        }

        public static void AccessResponse(int fileid, int rUserID, RequestStatus status)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var requests = db.FileAccessRequestRepository.Query().
                    Where(r => r.FileID == fileid && r.UserID == rUserID && r.File.Owner.UserID == UserPayload.UserID && r.Status == "P").
                    ToList();
                if (requests.Count < 1)
                    throw new Exception("No request found");
                
                var file = db.FileRepository.GetByID(fileid);

                if (status == RequestStatus.Accepted)
                {
                    file.AccessUsers.Add(db.UserRepository.GetByID(rUserID));
                    if (file.AccessLevel == "N")
                        file.AccessLevel = "L";
                }
                else
                {
                    if (file.AccessUsers.Where(u => u.UserID == rUserID).FirstOrDefault() != null)
                        file.AccessUsers.Remove(db.UserRepository.GetByID(rUserID));
                }

                for (int i = 0; i < requests.Count; i++)
                {
                    requests[i].Status = RequestStatusConverter.ToDBEntity(status);
                    db.FileAccessRequestRepository.Update(requests[i]);
                }
                string email = db.UserRepository.GetByID(rUserID).Email;
                db.Commit();

                MailService.SendMail(email, string.Format("<p>Your request has been {0}</p>", status.ToString()),
                    isHtml: true, subject: "DMS - File Access Request");
            }
        }

        public static byte[] ReadFile(int fileID, int version, out string filename)
        {
            GetAccessList();
            filename = "temp";
            using (var db = UnitOfWorkFactory.Create())
            {
                var file = db.FileRepository.GetByID(fileID);
                CheckAccess(fileID);
                var name = file.FileVersions.Where(f => f.FileID == fileID && f.VerNo == version).Select(f => f.Name).FirstOrDefault();
                filename = name;
                return FileEncryptor.DecryptFile(DirectoryHelper.GetRootStorage() + file.RelativeDirectory
                    + "/" + name);
            }
        }

        //External Read
        public static byte[] ReadFile(string link, int version, out string filename)
        {
            filename = "temp";
            using (var db = UnitOfWorkFactory.Create())
            {
                var file = db.FileRepository.Query().Where(f => f.ExternalLink == link).FirstOrDefault();
                var name = file.FileVersions.Where(f => f.FileID == file.FileID && f.VerNo == version).
                    Select(f => f.Name).FirstOrDefault();
                filename = name;
                return FileEncryptor.DecryptFile(DirectoryHelper.GetRootStorage() + file.RelativeDirectory
                    + "/" + name);
            }
        }
    }
}
