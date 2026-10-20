namespace TaskTrackerWebAdminPanel.Models;

// EmployeeTasks Model for the application.
public class EmployeeTasksModel : BaseModel
{
    #region Data-Model Properties

    public int Id { get; set; }
    public int UserId { get; set; }
    public string YesterdaysTask { get; set; } = string.Empty;
    public string TodaysTask { get; set; } = string.Empty;
    public string Obstacle { get; set; } = string.Empty;

    #endregion
}