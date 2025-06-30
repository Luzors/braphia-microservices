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
        public DbSet<Test> Test { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Decimal precision voor Invoice Amount
            modelBuilder.Entity<Invoice>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);
            // Patient - Insurer relationship (many patients to one insurer)
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Insurer)
                .WithMany()
                .HasForeignKey(p => p.InsurerId)
                .OnDelete(DeleteBehavior.SetNull);
            // Invoice - Insurer relationship (many invoices to one insurer)
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Insurer)
                .WithMany(ins => ins.Invoices)
                .HasForeignKey(i => i.InsurerId)
                .OnDelete(DeleteBehavior.Restrict);
            // Test - Patient relationship (many tests to one patient)
            modelBuilder.Entity<Test>()
                .HasOne(t => t.Patient)
                .WithMany()
                .HasForeignKey(t => t.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
