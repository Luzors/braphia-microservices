using Braphia.Laboratory.Models;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Laboratory.Database
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<CentralLaboratory> CentralLaboratories { get; set; }
        public DbSet<Test> Tests { get; set; }
    }
}
