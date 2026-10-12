using Microsoft.EntityFrameworkCore;
using TaskTrackerBot.Data;
using TaskTrackerBot.Models;

namespace TaskTrackerBot.Services;

// User Service for the application.
public class UserService
{
    #region UserService Methods

    // Get or create user method for the application.
    public async Task<UserModel> GetOrCreateUserAsync()
    {
        var windowsUserName = Environment.UserName;

        var options = new DbContextOptionsBuilder<TaskTrackerDbContext>()
            .UseSqlServer(DatabaseSettings.ConnectionString)
            .Options;

        await using var db = new TaskTrackerDbContext(options);

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.UserName == windowsUserName);

        if (user != null)
        {
            return user;
        }

        user = new UserModel
        {
            UserName = windowsUserName,
            IsActive = true,
            IsDeleted = false,
            CreatedOn = DateTime.Now
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user;
    }

    // Save employee tasks method for the application.
    public async Task SaveEmployeeTasksAsync(int userId,string yesterdayTask,string todayTask,string obstacle)
    {
        var options = new DbContextOptionsBuilder<TaskTrackerDbContext>()
            .UseSqlServer(DatabaseSettings.ConnectionString)
            .Options;

        await using var db = new TaskTrackerDbContext(options);

        var task = new EmployeeTasksModel
        {
            UserId = userId,
            YesterdaysTask = yesterdayTask,
            TodaysTask = todayTask,
            Obstacle = obstacle,
            IsDeleted = false,
            CreatedOn = DateTime.Now
        };

        db.EmployeeTasks.Add(task);
        await db.SaveChangesAsync();
    }

    #endregion
}
