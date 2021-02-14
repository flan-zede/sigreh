using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sigreh.Models;

namespace sigreh.Data
{
    public class SigrehContext : DbContext
    {
        public SigrehContext(DbContextOptions<SigrehContext> opt) : base(opt)
        {

        }

        public DbSet<City> Cities { get; set;  }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Establishment> Establishments { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Subprefecture> Subprefectures { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
