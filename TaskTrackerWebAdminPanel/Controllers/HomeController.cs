using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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


    public async Task<IActionResult> Search(
        int? selectedUserId,
        DateTime? fromDate,
        DateTime? toDate)
    {
        // Prepare the ViewModel.
        var model = new EmployeeTaskSearchViewModel
        {
            SelectedUserId = selectedUserId,
            FromDate = fromDate,
            ToDate = toDate,

            SearchPerformed =Request.Query.ContainsKey("selectedUserId") ||
                Request.Query.ContainsKey("fromDate") ||Request.Query.ContainsKey("toDate")
        };

        // Load active employees for the dropdown.
        var employees = await _context.Users
            .AsNoTracking()
            .Where(u => u.IsActive && !u.IsDeleted)
            .OrderBy(u => u.UserName)
            .Select(u => new
            {
                u.Id,
                u.UserName
            })
            .ToListAsync();

        model.Employees = employees
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.UserName,
                Selected = selectedUserId == u.Id
            })
            .ToList();

        // Add the All Employees option.
        model.Employees.Insert(0, new SelectListItem
        {
            Value = "",
            Text = "All Employees",
            Selected = !selectedUserId.HasValue
        });

        // Show the page without results on the initial visit.
        if (!model.SearchPerformed)
        {
            return View(model);
        }

        // Validate the required dates.
        if (!fromDate.HasValue || !toDate.HasValue)
        {
            ModelState.AddModelError(string.Empty,"Please select both From Date and To Date.");
            return View(model);
        }

        // Validate the date range.
        if (fromDate.Value.Date > toDate.Value.Date)
        {
            ModelState.AddModelError(string.Empty,"From Date cannot be later than To Date. Please check your selected dates.");
            return View(model);
        }

        // Prepare date boundaries.
        var startDate = fromDate.Value.Date;
        var endDateExclusive = toDate.Value.Date.AddDays(1);

        // Query reports and their employees.
        var query =from task in _context.EmployeeTasks.AsNoTracking()
                join user in _context.Users.AsNoTracking()
                on task.UserId equals user.Id
                where !task.IsDeleted
                  && !user.IsDeleted
                  && user.IsActive
                  && task.CreatedOn.HasValue
                  && task.CreatedOn.Value >= startDate
                  && task.CreatedOn.Value < endDateExclusive
                select new { task, user };

        // Filter by employee when one is selected.
        if (selectedUserId.HasValue)
        {
            query = query.Where(x => x.user.Id == selectedUserId.Value);
        }

        // Retrieve matching reports.
        model.Reports = await query
            .OrderByDescending(x => x.task.CreatedOn)
            .ThenBy(x => x.user.UserName)
            .Select(x => new EmployeeTaskSearchResultViewModel
            {
                EmployeeName = x.user.UserName,
                YesterdaysTask = x.task.YesterdaysTask,
                TodaysTask = x.task.TodaysTask,
                Obstacle = x.task.Obstacle,
                ReportDate = x.task.CreatedOn
            })
            .ToListAsync();

        // Group reports by date, newest date first.
        model.DateGroups = model.Reports
            .GroupBy(report => report.ReportDate!.Value.Date)
            .OrderByDescending(group => group.Key)
            .Select(group => new EmployeeTaskDateGroupViewModel
            {
                ReportDate = group.Key,
                Reports = group.ToList()
            })
            .ToList();

        return View(model);
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
