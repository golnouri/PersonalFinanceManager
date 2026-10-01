using Microsoft.EntityFrameworkCore;

namespace Hesabdar.Models
{
    public class SqlServerDbContext : DbContext
    {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options)
        {
        }

        public DbSet<RegisterModel> tbl_register { get; set; }
    }
}
