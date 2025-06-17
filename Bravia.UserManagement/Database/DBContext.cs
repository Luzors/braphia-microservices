using k8s.KubeConfigModels;
using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using Microsoft.EntityFrameworkCore;
using Braphia.UserManagement.Model;

namespace Braphia.UserManagement.Database
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Patient> Patient { get; set; }
    }
}
