using Model.Domain_Model;

namespace Repository.UnitOfWork
{
    public class UnitOfWork : UnitOfWorkBase
    {
        public GeneralRepository GeneralRepository { get; internal set; }
        public GenericRepository<AccessRequest> FileAccessRequestRepository { get; internal set; }
        public GenericRepository<File> FileRepository { get; internal set; }
        public GenericRepository<FileVersion> FileVersionRepository { get; internal set; }
        public GenericRepository<SizeRequest> FileSizeRequestRepository { get; internal set; }
        public GenericRepository<User> UserRepository { get; internal set; }
        public SearchRepository SearchRepository { get; internal set; }

        public UnitOfWork() : base()
        {
            GeneralRepository = new GeneralRepository(db);
            FileAccessRequestRepository = new GenericRepository<AccessRequest>(db);
            FileRepository = new GenericRepository<File>(db);
            FileVersionRepository = new GenericRepository<FileVersion>(db);
            FileSizeRequestRepository = new GenericRepository<SizeRequest>(db);
            UserRepository = new GenericRepository<User>(db);
            SearchRepository = new SearchRepository(db);
        }

    }
}
