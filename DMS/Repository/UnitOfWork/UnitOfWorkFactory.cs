namespace Repository.UnitOfWork
{
    public class UnitOfWorkFactory
    {
        public static UnitOfWork Create()
        {
            return new UnitOfWork();
        }
    }
}
