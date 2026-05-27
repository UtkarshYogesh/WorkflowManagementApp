using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Api.Data
{
    public class SqliteAppDbContext : AppDbContext
    {
        public SqliteAppDbContext(DbContextOptions<SqliteAppDbContext> options) : base(options) { }
    }
}
