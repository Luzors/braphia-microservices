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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Prescription relationships
            modelBuilder.Entity<Prescription>()
                .HasOne<Patient>()
                .WithMany()
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Prescription>()
                .HasOne<Physician>()
                .WithMany()
                .HasForeignKey(p => p.PhysicianId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Prescription>()
                .HasOne<MedicalAnalysis>()
                .WithMany()
                .HasForeignKey(p => p.MedicalAnalysisId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure MedicalAnalysis relationships
            modelBuilder.Entity<MedicalAnalysis>()
                .HasOne<Patient>()
                .WithMany(p => p.MedicalAnalyses)
                .HasForeignKey(ma => ma.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MedicalAnalysis>()
                .HasOne<Physician>()
                .WithMany()
                .HasForeignKey(ma => ma.PhysicianId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MedicalAnalysis>()
                .HasOne<Appointment>()
                .WithMany()
                .HasForeignKey(ma => ma.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
