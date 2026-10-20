using Microsoft.EntityFrameworkCore;
using TaskTrackerWebAdminPanel.Models;

namespace TaskTrackerWebAdminPanel.Data;

// Database context for the admin panel.
public class TaskTrackerDbContext : DbContext
{
    #region Constructor

    public TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options) 
        : base(options)
    {
    }

    #endregion

    #region Db Sets

    public DbSet<EmployeeTasksModel> EmployeeTasks => Set<EmployeeTasksModel>();
    public DbSet<UserModel> Users =>Set<UserModel>();

    #endregion   
}