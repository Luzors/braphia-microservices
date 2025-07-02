using Braphia.Accounting.EventSourcing;
using Braphia.Accounting.EventSourcing.Events;
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
        public DbSet<BaseEvent> Event { get; set; }

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

            // Configure TPH mapping for BaseEvent
            modelBuilder.Entity<BaseEvent>()
                .ToTable("Event")
                .HasDiscriminator<string>("EventType")
                .HasValue<InvoiceCreatedEvent>("InvoiceCreated")
                .HasValue<PaymentReceivedEvent>("PaymentReceived");
        }
    }
}
