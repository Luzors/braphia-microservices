using Braphia.Pharmacy.Models;
using Braphia.Pharmacy.Models.ExternalObjects;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Pharmacy.Database
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Patient> Patient { get; set; }
        public DbSet<Prescription> Prescription { get; set; }
        public DbSet<Medication> Medication { get; set; }
        public DbSet<MedicationOrder> MedicationOrder { get; set; }
        public DbSet<Models.Pharmacy> Pharmacy { get; set; }

    }
}
