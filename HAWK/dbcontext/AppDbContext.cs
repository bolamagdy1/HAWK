using Microsoft.EntityFrameworkCore;
using HAWK.Models;

namespace HAWK.dbcontext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Slider> Sliders { get; set; }
    }
}
