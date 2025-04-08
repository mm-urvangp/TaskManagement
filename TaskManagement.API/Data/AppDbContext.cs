using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models;

namespace TaskManagement.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<TaskAssignmentModel> TaskAssignment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique constraint on Email and Mobile
            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.Mobile)
                .IsUnique();

            // Relationships
            modelBuilder.Entity<TaskAssignmentModel>()
                .HasOne(t => t.Task)
                .WithMany()
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskAssignmentModel>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
