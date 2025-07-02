using Braphia.NotificationDispatcher.Models;
using Microsoft.EntityFrameworkCore;

namespace Braphia.NotificationDispatcher.Database
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Notification> Notification { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Pharmacy> Pharmacy { get; set; }
        public DbSet<Laboratory> Laboratory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Make sure any of the three ids are filled in sql
            modelBuilder.Entity<Notification>().ToTable(t =>
                t.HasCheckConstraint("CK_Notification_AtLeastOneId",
                    "(UserId IS NOT NULL OR PharmacyId IS NOT NULL OR LaboratoryId IS NOT NULL)"));
            // Prevent multiple cascade paths for Patient -> GeneralPracticioner
            modelBuilder.Entity<User>()
                .HasOne(p => p.GeneralPracticioner)
                .WithMany()
                .HasForeignKey(p => p.GeneralPracticionerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
