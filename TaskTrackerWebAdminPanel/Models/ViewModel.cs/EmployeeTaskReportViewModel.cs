namespace TaskTrackerWebAdminPanel.Models;

// EmployeeTaskReportViewModel Model for the application.
public class EmployeeTaskReportViewModel
{
    #region View Model Properties

    public int Id { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public DateTime? CreatedOn { get; set; }

    public string YesterdaysTask { get; set; } = string.Empty;

    public string TodaysTask { get; set; } = string.Empty;

    public string Obstacle { get; set; } = string.Empty;

    #endregion
}
