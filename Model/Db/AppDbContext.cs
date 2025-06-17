using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebAPI.Model.Db
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
