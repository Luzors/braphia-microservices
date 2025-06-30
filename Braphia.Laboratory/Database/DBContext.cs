using Braphia.Laboratory.Models;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Laboratory.Database
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<CentralLaboratory> CentralLaboratory { get; set; }
        public DbSet<Test> Test { get; set; }
        public DbSet<Patient> Patient { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Test>()
                .Property(t => t.Cost)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Tests);
        }
    }
}
