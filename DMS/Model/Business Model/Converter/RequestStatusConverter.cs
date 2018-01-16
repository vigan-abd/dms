using Model.Business.ViewModel;
using Model.Domain_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Business.Converter
{
    public static class RequestStatusConverter
    {
        public static string ToDBEntity(RequestStatus status)
        {
            switch (status)
            {
                case RequestStatus.Accepted: return "A";
                case RequestStatus.Denied: return "D";
                case RequestStatus.Pending: return "P";
                default:
                    return "D";
            }
        }

        public static RequestStatus FromDBEntity(string status)
        {
            switch (status)
            {
                case "A": return RequestStatus.Accepted;
                case "D": return RequestStatus.Denied;
                case "P": return RequestStatus.Pending;
                default:
                    return RequestStatus.Denied;
            }
        }
    }
}
