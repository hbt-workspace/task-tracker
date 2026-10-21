namespace TaskTrackerWebAdminPanel.Models;

// EmployeeTaskDateGroupViewModel Model for the application.
public class EmployeeTaskDateGroupViewModel
{
    #region Date Group Properties

    public DateTime ReportDate { get; set; }
    public List<EmployeeTaskSearchResultViewModel> Reports { get; set; } = new();

    #endregion
}
