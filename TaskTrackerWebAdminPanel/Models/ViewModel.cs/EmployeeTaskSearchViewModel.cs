using Microsoft.AspNetCore.Mvc.Rendering;

namespace TaskTrackerWebAdminPanel.Models;

// EmployeeTaskSearchViewModel Model for the application.
public class EmployeeTaskSearchViewModel
{
    #region Search Properties

    public int? SelectedUserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<SelectListItem> Employees { get; set; } = new();
    public List<EmployeeTaskSearchResultViewModel> Reports { get; set; } = new();
    
    // Reports grouped by date for the Search page.
    public List<EmployeeTaskDateGroupViewModel> DateGroups { get; set; } = new();
    public bool SearchPerformed { get; set; }

    #endregion
}
