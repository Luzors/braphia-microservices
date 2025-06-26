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

        public DbSet<Receptionist> Receptionist { get; set; }
    }
}
