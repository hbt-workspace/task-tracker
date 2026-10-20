namespace TaskTrackerWebAdminPanel.Models;

// Error View Model for the application.
public class ErrorViewModel
{
    #region View Model Properties
    
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    #endregion
}
