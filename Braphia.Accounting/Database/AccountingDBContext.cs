using Braphia.Accounting.Models;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Accounting.Database
{
    public class AccountingDBContext : DbContext
    {
        public AccountingDBContext(DbContextOptions<AccountingDBContext> options) : base(options) { }

        public DbSet<Patient> Patient { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<Insurer> Insurer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Decimal precision voor Invoice Amount
            modelBuilder.Entity<Invoice>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);
        }
    }
}
