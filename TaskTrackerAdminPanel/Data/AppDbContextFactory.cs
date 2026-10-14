using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TaskTrackerAdminPanel.Data;

// Database Context Factory for the application to create the database context. 
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    #region Fields and Constructor

    private readonly IConfiguration _configuration;

    public AppDbContextFactory()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json",optional: false,reloadOnChange: true)
            .Build();
    }

    #endregion

    #region CreateDbContext

    // CreateDbContext method for the database context factory. 
    public AppDbContext CreateDbContext(string[] args)
    {
        var options =
            new DbContextOptionsBuilder<AppDbContext>();

        string connectionString =
            _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        options.UseSqlServer(connectionString);

        return new AppDbContext(options.Options);
    }

    #endregion
}