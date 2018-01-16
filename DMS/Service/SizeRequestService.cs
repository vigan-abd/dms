using Model.Business.Converter;
using Model.Business.ViewModel;
using Model.Domain_Model;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Service
{
    public static class SizeRequestService
    {
        public static Dictionary<string, List<SizeRequestViewModel>> GroupedRequests()
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                Dictionary<string, List<SizeRequestViewModel>> sortedList = new Dictionary<string, List<SizeRequestViewModel>>();
                sortedList.Add(RequestStatus.Accepted.ToString(), new List<SizeRequestViewModel>());
                sortedList.Add(RequestStatus.Denied.ToString(), new List<SizeRequestViewModel>());
                sortedList.Add(RequestStatus.Pending.ToString(), new List<SizeRequestViewModel>());

                var unsortedList = db.GeneralRepository.GetSizeRequestUserRelationalModel() as List<SizeRequestViewModel>;
                foreach (var item in unsortedList)
                {
                    switch (item.RequestStatus)
                    {
                        case RequestStatus.Accepted:
                            sortedList[RequestStatus.Accepted.ToString()].Add(item);
                            break;
                        case RequestStatus.Denied:
                            sortedList[RequestStatus.Denied.ToString()].Add(item);
                            break;
                        case RequestStatus.Pending:
                            sortedList[RequestStatus.Pending.ToString()].Add(item);
                            break;
                    }
                }
                return sortedList;
            }
        }

        public static void Request(string username, int size)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var user = db.UserRepository.Query().Where(u => u.Username == username).FirstOrDefault();
                var request = new SizeRequest()
                {
                    Amount = size,
                    UserID = user.UserID,
                    Status = RequestStatusConverter.ToDBEntity(RequestStatus.Pending),
                    Stamp = DateTime.Now
                };
                db.FileSizeRequestRepository.Add(request);
                db.Commit();
            }
        }

        public static void Respond(int requestID, string email, RequestStatus status)
        {
            using (var db = UnitOfWorkFactory.Create())
            {
                var request = db.FileSizeRequestRepository.GetByID(requestID);
                request.Status = RequestStatusConverter.ToDBEntity(status);
                db.FileSizeRequestRepository.Update(request);

                var user = db.UserRepository.GetByID(request.UserID);
                user.StorageSize = user.StorageSize + request.Amount;
                db.UserRepository.Update(user);

                db.Commit();
                MailService.SendMail(email, string.Format("<p>Your request has been {0} by administrator</p>", status.ToString()),
                    isHtml: true, subject: "DMS - Size Request");
            }
        }
    }
}
