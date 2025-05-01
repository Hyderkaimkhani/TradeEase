using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Context
{
    public class Context : DbContext
    {
        public Context() : base()
        {
            //this.ChangeTracker.QueryTrackingBehavior =QueryTrackingBehavior.NoTracking;
        }

        public Context(DbContextOptions options) : base(options)
        {
            //this.ChangeTracker.QueryTrackingBehavior =QueryTrackingBehavior.NoTracking;
            //Database.SetCommandTimeout(20000);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<UserAuth> UserAuth { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<AppToken> AppToken { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //  base.OnModelCreating(modelBuilder);
        //  modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //}
    }
}
