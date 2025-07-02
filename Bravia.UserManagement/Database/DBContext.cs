using Microsoft.EntityFrameworkCore;
using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Database
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Patient> Patient { get; set; }

        public DbSet<Physician> Physician { get; set; }

        public DbSet<Receptionist> Receptionist { get; set; }

        public DbSet<GeneralPracticioner> GeneralPracticioner { get; set; }

        public DbSet<Referral> Referral { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set Patient.GeneralPracticionerId to null when GP is deleted
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.GeneralPracticioner)
                .WithMany()
                .HasForeignKey(p => p.GeneralPracticionerId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
