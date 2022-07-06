using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core_3._1.Models
{
    public class AppDBContext : IdentityDbContext
    {
        private readonly DbContextOptions _options;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<DomainAlias> DomainAliases { get; set; }
        public AppDBContext(DbContextOptions options): base(options)
        {
            _options = options; 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
