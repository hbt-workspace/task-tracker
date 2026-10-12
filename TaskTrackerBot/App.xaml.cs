using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using TaskTrackerBot.Data;
using TaskTrackerBot.Services;

namespace TaskTrackerBot;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    #region On Startup Method

    // OnStartup method for the application.
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var userService = new UserService();
            await userService.GetOrCreateUserAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Unable to initialize the employee record.\n" + ex.Message,
                "Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();
        }
    }

    #endregion

    #region Private Methods

    // CreateDbContext method for the application.
    public TaskTrackerDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TaskTrackerDbContext>();

        options.UseSqlServer(DatabaseSettings.ConnectionString);

        return new TaskTrackerDbContext(options.Options);
    }

    #endregion
}

