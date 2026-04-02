using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TaskTag> TaskTags { get; set; }
    public DbSet<RemindersLog> RemindersLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global Query Filter for Tasks
        modelBuilder.Entity<TaskEntity>().HasQueryFilter(t => !t.IsDeleted);

        // Unidirectional User -> Task
        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId);

        // Unique indexes
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Tag>().HasIndex(t => t.Name).IsUnique();
        
        // Index for performance on DueDate
        modelBuilder.Entity<TaskEntity>().HasIndex(t => t.DueDate);

        // Many-to-Many TaskTag
        modelBuilder.Entity<TaskTag>()
            .HasKey(tt => new { tt.TaskId, tt.TagId });

        modelBuilder.Entity<TaskTag>()
            .HasOne(tt => tt.Task)
            .WithMany(t => t.TaskTags)
            .HasForeignKey(tt => tt.TaskId);

        modelBuilder.Entity<TaskTag>()
            .HasOne(tt => tt.Tag)
            .WithMany()
            .HasForeignKey(tt => tt.TagId);
            
        // Table Name mapping specifically because entity is TaskEntity
        modelBuilder.Entity<TaskEntity>().ToTable("Tasks");
    }
}
