namespace TaskTrackerAdminPanel.Models;

// Employee Report Result Model.
public class EmployeeReportResult
{
    #region result properties

    public string EmployeeName { get; set; } = string.Empty;
    public string YesterdaysTask { get; set; } = string.Empty;
    public string TodaysTask { get; set; } = string.Empty;
    public string Obstacle { get; set; } = string.Empty;
    public DateTime? ReportDate { get; set; }

    #endregion
}
