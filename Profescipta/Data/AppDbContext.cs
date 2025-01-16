using Microsoft.EntityFrameworkCore;
using Profescipta.Models;

namespace Profescipta.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<COM_CUSTOMER> COM_CUSTOMER { get; set; }
        public DbSet<SO_ITEM> SO_ITEM { get; set; }
        public DbSet<SO_ORDER> SO_ORDER { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // COM_CUSTOMER
            modelBuilder.Entity<COM_CUSTOMER>(entity =>
            {
                entity.ToTable("COM_CUSTOMER");
                entity.HasKey(e => e.COM_CUSTOMER_ID);
            });

            // SO_ORDER
            modelBuilder.Entity<SO_ORDER>(entity =>
            {
                entity.ToTable("SO_ORDER");
                entity.HasKey(e => e.SO_ORDER_ID);

                entity.HasOne(o => o.COM_CUSTOMER)
                      .WithMany(c => c.SO_ORDERS)
                      .HasForeignKey(o => o.COM_CUSTOMER_ID);
            });

            // SO_ITEM
            modelBuilder.Entity<SO_ITEM>(entity =>
            {
                entity.ToTable("SO_ITEM");
                entity.HasKey(e => e.SO_ITEM_ID);

                entity.HasOne(i => i.SO_ORDER)
                      .WithMany(o => o.SO_ITEM)
                      .HasForeignKey(i => i.SO_ORDER_ID);
            });

        }
    }
}
