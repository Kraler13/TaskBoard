using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Persistence
{
    public class TaskBoardDbContext : DbContext
    {
        public TaskBoardDbContext(DbContextOptions<TaskBoardDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Column> Columns => Set<Column>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Projects)
                .WithOne(p => p.Owner)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Columns)
                .WithOne(c => c.Project)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Column>()
                .HasMany(c => c.Tasks)
                .WithOne(t => t.Column)
                .HasForeignKey(t => t.ColumnId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}