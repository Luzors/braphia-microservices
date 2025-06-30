using Microsoft.EntityFrameworkCore;
using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Database
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Prescription> Prescription { get; set; }

        public DbSet<Patient> Patient { get; set; }

        public DbSet<Physician> Physician { get; set; }

        public DbSet<MedicalAnalysis> MedicalAnalysis { get; set; }

        public DbSet<Appointment> Appointment { get; set; }

        public DbSet<Test> Test { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Prescription relationships
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany()
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Physician)
                .WithMany()
                .HasForeignKey(p => p.PhysicianId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure MedicalAnalysis relationships
            modelBuilder.Entity<MedicalAnalysis>()
                .HasOne(ma => ma.Patient)
                .WithMany(p => p.MedicalAnalyses)
                .HasForeignKey(ma => ma.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MedicalAnalysis>()
                .HasOne(ma => ma.Physician)
                .WithMany()
                .HasForeignKey(ma => ma.PhysicianId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MedicalAnalysis>()
                .HasOne(ma => ma.Appointment)
                .WithMany()
                .HasForeignKey(ma => ma.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
