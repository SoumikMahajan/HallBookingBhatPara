using HallBookingBhatPara.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HallBookingBhatPara.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.SetCommandTimeout(9000);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // DbSet properties for your entities can be added here
        // public DbSet<YourEntity> YourEntities { get; set; }
        public DbSet<public_user_registration> public_user_registrations { get; set; }
    }
}
