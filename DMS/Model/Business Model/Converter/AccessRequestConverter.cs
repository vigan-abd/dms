using Model.Business.ViewModel;
using Model.Domain_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Business.Converter
{
    public static class AccessRequestConverter
    {
        public static AccessRequestViewModel DomainToView(AccessRequest model)
        {
            var vm = new AccessRequestViewModel()
            {
                FileID = model.FileID,
                UserID = model.UserID,
                RequestID = model.RequestID,
                Date = model.Stamp
            };
            vm.RequestStatus = RequestStatusConverter.FromDBEntity(model.Status);
            vm.Owner = model.File.Owner.Username;
            vm.Requester = model.User.Username;
            return vm;
        }
    }
}
