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
        public virtual DbSet<Supply> Supply { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderSupplies> OrderSupplies { get; set; }
        public virtual DbSet<Truck> Truck { get; set; }
        public virtual DbSet<Fruit> Fruit { get; set; }
        public virtual DbSet<TruckAssignment> TruckAssignment { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PaymentAllocation> PaymentAllocation { get; set; }
        public virtual DbSet<Bill> Bill { get; set; }
        public virtual DbSet<BillDetail> BillDetail { get; set; }

        public virtual DbSet<AppToken> AppToken { get; set; }

        public virtual DbSet<AuditLog> AuditLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Customer)
                .WithMany() // Add .WithMany(c => c.Payments) if Customer has Payments collection
                .HasForeignKey(p => p.EntityId);

        }
    }
}
