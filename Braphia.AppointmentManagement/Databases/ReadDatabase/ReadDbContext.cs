using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase
{
    public class ReadDbContext : DbContext
    {
        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }
        public DbSet<AppointmentViewQueryModel> AppointmentViewQueryModels { get; set; }
       
    }
}
