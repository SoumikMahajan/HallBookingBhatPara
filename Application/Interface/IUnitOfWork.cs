namespace HallBookingBhatPara.Application.Interface
{
    public interface IUnitOfWork
    {
        Task SaveAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        bool HasActiveTransaction { get; }
    }
}
