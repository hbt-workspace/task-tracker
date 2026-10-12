
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskTrackerBot.Data;

// Database Context Factory for the application to create the database context. 
public class TaskTrackerDbContextFactory
    : IDesignTimeDbContextFactory<TaskTrackerDbContext>
{
    #region CreateDbContext

    // CreateDbContext method for the database context factory. 
    public TaskTrackerDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TaskTrackerDbContext>();

        options.UseSqlServer(DatabaseSettings.ConnectionString);

        return new TaskTrackerDbContext(options.Options);
    }

    #endregion
}
