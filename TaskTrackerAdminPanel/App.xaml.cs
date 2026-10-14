using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskTrackerAdminPanel.Data;

namespace TaskTrackerAdminPanel;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    #region Fields and Constructor

    public static IConfiguration Configuration { get; private set; } = null!;
    public static string ConnectionString { get; private set; } = string.Empty;

    public App()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        ConnectionString = Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");
    }

    #endregion

    #region Private Methods

    // Creates the DbContext for the admin panel.
    public AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>();

        options.UseSqlServer(ConnectionString);

        return new AppDbContext(options.Options);
    }

    #endregion
}