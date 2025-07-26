namespace HallBookingBhatPara.Application.Interface
{
    public interface IUnitOfWork
    {
        #region:: Stored Procedure              
        ISPRepository SPRepository { get; }
        #endregion
        Task SaveAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        bool HasActiveTransaction { get; }
    }
}
