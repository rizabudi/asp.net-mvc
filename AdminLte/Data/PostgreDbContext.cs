using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminLte.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminLte.Data
{
    public class PostgreDbContext : IdentityDbContext
    {
        public PostgreDbContext(DbContextOptions<PostgreDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseNpgsql("Server=103.23.22.126;Port=5533;Database=adminlte;Username=postgres;Password=!@#123qwe;Persist Security Info=True");
    
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeGroup> EmployeeGroup { get; set; }
    }
}
