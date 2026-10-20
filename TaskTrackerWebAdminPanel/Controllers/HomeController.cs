using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTrackerWebAdminPanel.Data;
using TaskTrackerWebAdminPanel.Models;

namespace TaskTrackerWebAdminPanel.Controllers;

// Home controller for the admin panel.
public class HomeController : Controller
{
    #region Fields and Contructor

    private readonly TaskTrackerDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(TaskTrackerDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    #endregion

    #region Default Actions

    public async Task<IActionResult> Index()
    {
        DateTime today = DateTime.Today;
        DateTime tomorrow = today.AddDays(1);

        // Fetch today's reports from the database.
        var todaysReports = await (
            from task in _context.EmployeeTasks.AsNoTracking()
            join user in _context.Users.AsNoTracking()
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
            .Select(report => new EmployeeTaskReportViewModel
            {
                Id = report.EmployeeId,
                EmployeeName = report.EmployeeName,
                YesterdaysTask = report.YesterdaysTask,
                TodaysTask = report.TodaysTask,
                Obstacle = report.Obstacle,
                CreatedOn = report.CreatedOn
            })
            .OrderByDescending(report => report.CreatedOn)
            .ToList();

        return View(reports);
    }

    #endregion

    #region Search Actions

    public IActionResult Search()
    {
        return View();
    }

    #endregion

    #region Privacy Actions

    public IActionResult Privacy()
    {
        return View();
    }

    #endregion

    #region Error Actions

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #endregion
}
