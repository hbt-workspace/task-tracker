
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TaskTrackerBot.Data;

// Database Context Factory for the application to create the database context. 
public class TaskTrackerDbContextFactory : IDesignTimeDbContextFactory<TaskTrackerDbContext>
{
    #region Fields and Constructor

    private readonly IConfiguration _configuration;

    public TaskTrackerDbContextFactory()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    #endregion

    #region CreateDbContext

    // CreateDbContext method for the database context factory. 
    public TaskTrackerDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TaskTrackerDbContext>();

        string? connectionString =
                   _configuration.GetConnectionString("DefaultConnection");
                   
        options.UseSqlServer(connectionString);

        return new TaskTrackerDbContext(options.Options);
    }

    #endregion
}
