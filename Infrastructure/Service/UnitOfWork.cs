using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Application.Interface.User;
using HallBookingBhatPara.Infrastructure.Data;
using HallBookingBhatPara.Infrastructure.Service.User;
using Microsoft.EntityFrameworkCore.Storage;

namespace HallBookingBhatPara.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        private IDbContextTransaction? _transaction;
        private readonly LogService _logService;
        private readonly IConfiguration _configuration;
        public bool HasActiveTransaction => _transaction != null;

        #region :: Stored Procedure          
        public ISPRepository SPRepository { get; private set; }
        #endregion

        #region :: User
        public ICategoryMasterRepository CategoryMasterRepository { get; private set; }
        public ISubCategoryMasterRepository SubCategoryMasterRepository { get; private set; }
        public IHallAvailMasterRepository HallAvailMasterRepository { get; private set; }
        public IHallFloorMasterRepository HallFloorMasterRepository { get; private set; }
        #endregion

        public UnitOfWork(ApplicationDbContext db, IConfiguration configuration, LogService logService)
        {
            _db = db;
            _configuration = configuration;
            _logService = logService;

            #region :: Stored Procedure           
            SPRepository = new SPService(_configuration, _logService);
            #endregion

            #region :: User
            CategoryMasterRepository = new CategoryMasterService(_db);
            SubCategoryMasterRepository = new SubCategoryMasterService(_db);
            HallAvailMasterRepository = new HallAvailMasterService(_db);
            HallFloorMasterRepository = new HallFloorService(_db);
            #endregion
        }



        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _db.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
