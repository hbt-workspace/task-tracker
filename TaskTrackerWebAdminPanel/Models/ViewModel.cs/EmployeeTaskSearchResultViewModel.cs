namespace TaskTrackerWebAdminPanel.Models;

// Employee Report Result Model.
public class EmployeeTaskSearchResultViewModel
{
    #region Result Properties

    public string EmployeeName { get; set; } = string.Empty;
    public string YesterdaysTask { get; set; } = string.Empty;
    public string TodaysTask { get; set; } = string.Empty;
    public string Obstacle { get; set; } = string.Empty;
    public DateTime? ReportDate { get; set; }

    #endregion
}