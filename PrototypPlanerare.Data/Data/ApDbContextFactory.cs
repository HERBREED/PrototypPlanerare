using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PrototypPlanerare.Data
{
    // This lets EF Tools create the DbContext at design time (migrations) 
    // without launching the WinUI app.
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={AppDbContext.GetDefaultDbPath()}")
                .Options;

            return new AppDbContext(options);
        }
    }
}
