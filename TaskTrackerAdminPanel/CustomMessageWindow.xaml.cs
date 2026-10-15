using System.Windows;

namespace TaskTrackerAdminPanel;

// Custom Message Box.
public partial class CustomMessageBox : Window
{
    #region Constructor

    // Constructor
    public CustomMessageBox(string message,string title = "Notice")
    {
        InitializeComponent();
        MessageText.Text = message;
        MessageTitleText.Text = title;
    }

    #endregion

    #region Event Handlers

    // Closes the window when the close button is clicked.
    private void CloseButton_Click(object sender,RoutedEventArgs e)
    {
        Close();
    }

    // Shows the custom message box.
    public static void Show(string message,string title = "Notice",Window? owner = null)
    {
        var dialog = new CustomMessageBox(message, title);

        if (owner != null)
        {
            dialog.Owner = owner;
        }

        dialog.ShowDialog();
    }

    #endregion
}
