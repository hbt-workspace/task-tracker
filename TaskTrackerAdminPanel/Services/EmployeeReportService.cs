using Microsoft.EntityFrameworkCore;
using TaskTrackerAdminPanel.Data;
using TaskTrackerAdminPanel.Models;

namespace TaskTrackerAdminPanel.Services;

// Employee Report Service for the application.
public class EmployeeReportService
{
    #region Fields and Constructor

    private readonly AppDbContext _dbContext;

    public EmployeeReportService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #endregion

    #region Method for Getting Employee Reports

    // Get Employee Reports from the database and return as a list.
    public async Task<List<EmployeeReportResult>> GetEmployeeReportsAsync()
    {
        DateTime today = DateTime.Today;
        DateTime tomorrow = today.AddDays(1);

        // Fetch today's reports from SQL Server.
        var todaysReports = await (
            from task in _dbContext.EmployeeTasks.AsNoTracking()
            join user in _dbContext.Users.AsNoTracking()
                on task.UserId equals user.Id
            where !task.IsDeleted
                  && !user.IsDeleted
                  && user.IsActive
                  && task.CreatedOn >= today
                  && task.CreatedOn < tomorrow
            orderby task.CreatedOn descending, task.Id descending
            select new
            {
                EmployeeId = user.Id,
                EmployeeName = user.UserName,
                task.YesterdaysTask,
                task.TodaysTask,
                task.Obstacle,
                task.CreatedOn
            }
        ).ToListAsync();

        // Keep only the latest report for each employee.
        var reports = todaysReports
            .GroupBy(report => report.EmployeeId)
            .Select(group => group.First())
            .Select(report => new EmployeeReportResult
            {
                EmployeeName = report.EmployeeName,
                YesterdaysTask = report.YesterdaysTask,
                TodaysTask = report.TodaysTask,
                Obstacle = report.Obstacle,
                ReportDate = report.CreatedOn
            })
            .OrderByDescending(report => report.ReportDate)
            .ToList();

        return reports;
    }

    #endregion
}