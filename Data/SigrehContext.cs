using Microsoft.EntityFrameworkCore;
using sigreh.Models;

namespace sigreh.Data
{
    public class SigrehContext : DbContext
    {
        public SigrehContext(DbContextOptions<SigrehContext> opt) : base(opt) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Establishment> Establishments { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Partner> Partners { get; set; }
    }
}
