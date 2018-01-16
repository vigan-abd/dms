using System.Linq;
using System.Collections.Generic;
using Model.Domain_Model;
using Model.Business.Converter;
using Model.Business.ViewModel;

namespace Repository
{
    public class GeneralRepository : RepositoryBase
    {
        public GeneralRepository(DMSDBEntities db) : base(db)
        {
        }

        public List<SizeRequestViewModel> GetSizeRequestUserRelationalModel()
        {
            var list = db.Users.Join(
                db.SizeRequests,
                u => u.UserID,
                r => r.UserID,
                (u, r) =>
                    new SizeRequestViewModel()
                    {
                        RequestID = r.RequestID,
                        Username = u.Username,
                        UserEmail = u.Email,
                        CurrentSize = u.StorageSize,
                        AdditionalSize = r.Amount,
                        Date = r.Stamp,
                        RequestStatus = (r.Status == "A" ? RequestStatus.Accepted
                        : (r.Status == "D" ? RequestStatus.Denied : RequestStatus.Pending))
                    }
                ).OrderByDescending(u => u.RequestID).AsParallel().ToList();
            return list;
        }
        public List<User> GetUsersFromIDCollection(List<int> ids)
        {
            return db.Users.Where(u => ids.Contains(u.UserID)).ToList();
        }
    }
}
