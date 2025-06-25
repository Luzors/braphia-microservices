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
    }
}
