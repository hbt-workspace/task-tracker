
using Microsoft.EntityFrameworkCore;
using TaskTrackerBot.Models;

namespace TaskTrackerBot.Data;

// Database Context for the application to interact with the database.
public class TaskTrackerDbContext : DbContext
{
    #region Constructor

    public TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options)
        : base(options)
    {
    }

    #endregion

    #region DbSets

    // Users table
    public DbSet<UserModel> Users => Set<UserModel>();

    // EmployeeTasks table
    public DbSet<EmployeeTasksModel> EmployeeTasks => Set<EmployeeTasksModel>();

    #endregion

    #region OnModelCreating

    // OnModelCreating method for the database context.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.UserName)
                .IsUnique();
        });

        modelBuilder.Entity<EmployeeTasksModel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.YesterdaysTask)
                .IsRequired();

            entity.Property(e => e.TodaysTask)
                .IsRequired();

            entity.Property(e => e.Obstacle)
                .IsRequired();
        });
    }

    #endregion
}
