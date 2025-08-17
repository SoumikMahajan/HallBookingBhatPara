using HallBookingBhatPara.Application.Interface.HallBooking;
using HallBookingBhatPara.Application.Interface.User;

namespace HallBookingBhatPara.Application.Interface
{
    public interface IUnitOfWork
    {
        #region:: Stored Procedure              
        ISPRepository SPRepository { get; }
        #endregion

        #region :: User
        ICategoryMasterRepository CategoryMasterRepository { get; }
        ISubCategoryMasterRepository SubCategoryMasterRepository { get; }
        IHallAvailMasterRepository HallAvailMasterRepository { get; }
        IHallFloorMasterRepository HallFloorMasterRepository { get; }
        #endregion

        #region :: Hall Booking
        IHallEventMasterRepository HallEventMasterRepository { get; }
        IUserRegistrationRepository UserRegistrationRepository { get; }
        #endregion

        Task SaveAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        bool HasActiveTransaction { get; }
    }
}
