using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Api.Data
{
    public class SqlServerAppDbContext : AppDbContext
    {
        public SqlServerAppDbContext(DbContextOptions<SqlServerAppDbContext> options) : base(options) { }
    }
}
