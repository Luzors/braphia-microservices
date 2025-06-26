using Braphia.Pharmacy.Models;
using Braphia.UserManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace Braphia.UserManagement.Database
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Patient> Patient { get; set; }
        public DbSet<Prescription> Prescription { get; set; }

    }
}
