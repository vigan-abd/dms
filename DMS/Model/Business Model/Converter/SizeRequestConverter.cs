using Model.Business.ViewModel;
using Model.Domain_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Business.Converter
{
    public static class SizeRequestConverter
    {
        public static SizeRequestViewModel DomainToView(User u, SizeRequest r)
        {
            return new SizeRequestViewModel()
            {
                Username = u.Username,
                CurrentSize = u.StorageSize,
                AdditionalSize = r.Amount,
                RequestStatus = RequestStatusConverter.FromDBEntity(r.Status),
                Date = r.Stamp
            };
        }
    }
}
