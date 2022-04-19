using DeliciousFood.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace DeliciousFood.DataAccess.MsSqlServer
{
    /// <summary>
    /// Entity Framework database context for MS SQL
    /// </summary>
    public class MsSqlServerDbContext : BaseDbContext
    {
        public MsSqlServerDbContext(DbContextOptions<MsSqlServerDbContext> options) : base(options)
        {
        }
    }
}
