using Braphia.AppointmentManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase
{
    public class WriteDbContext : DbContext
    {
        public WriteDbContext(DbContextOptions<DbContext> options) : base(options) { }
        public DbSet<Referral> Referral { get; set; }
        public DbSet<Receptionist> receptionists { get; set; }
        public DbSet<Physician> physicians { get; set; }
        public DbSet<Patient> patients { get; set; }
        public DbSet<Appointment> appointments { get; set; }


    }
}
