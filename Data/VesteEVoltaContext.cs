using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;

namespace VesteEVolta.Data
{
    public class VesteEVoltaContext : DbContext
    {
        public VesteEVoltaContext(DbContextOptions<VesteEVoltaContext> options)
            : base(options)
        {
        }

        public DbSet<TbUser> TbUsers { get; set; }

        // Se existirem no seu banco, descomente:
        // public DbSet<TbCategory> TbCategories { get; set; }
        // public DbSet<TbClothing> TbClothings { get; set; }
    }
}
