using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Feature> Features => Set<Feature>();
        public DbSet<BacklogItem> BacklogItems => Set<BacklogItem>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured) return;

            options.UseSqlite(o =>
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<User>()
                .HasIndex(x => x.UserId)
                .IsUnique();

            mb.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            mb.Entity<Project>()
                .HasIndex(x => x.ProjectId)
                .IsUnique();

            mb.Entity<Project>()
                .HasMany(p => p.Features)
                .WithOne(p => p.Project)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<Feature>()
            .HasMany(f => f.BacklogItems)
            .WithOne(b => b.Feature)
            .HasForeignKey(b => b.FeatureId)
            .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<BacklogItem>()
            .HasMany(b => b.Tasks)
            .WithOne(t => t.BacklogItem)
            .HasForeignKey(t => t.BacklogItemId)
            .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<TaskItem>()
            .HasOne(t => t.AssignedToUser)
            .WithMany(t => t.AssignedTasks)
            .HasForeignKey(t => t.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);


            mb.Entity<Feature>()
            .HasOne(f => f.AssignedToUser)
            .WithMany(u => u.AssignedFeatures)
            .HasForeignKey(f => f.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);



            mb.Entity<BacklogItem>()
            .HasOne(f => f.AssignedToUser)
            .WithMany(u => u.AssignBacklogItems)
            .HasForeignKey(f => f.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);


            mb.Entity<User>().HasData(new User
            {
                UserId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                Username = "admin",
                Email = "admin@taskmanager.com",
                PasswordHash = "$2a$11$yourStaticHashHere",
                Role = "Admin",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

           
        }

        //public override int SaveChanges()
        //{
        //    Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");
        //    return base.SaveChanges();
        //}

        //public override async Task<int> SaveChangesAsync(
        //    CancellationToken cancellationToken = default)
        //{
        //    await Database.ExecuteSqlRawAsync(
        //        "PRAGMA foreign_keys = ON;", cancellationToken);
        //    return await base.SaveChangesAsync(cancellationToken);
        //}
    }
}
