using System.Windows;

namespace TaskTrackerBot;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MessageBoxWindow : Window
{ 
    #region Constructor

    // Constructor 
    public MessageBoxWindow()
    {
        InitializeComponent();
        
    }

    #endregion

    #region Event Handlers

    // Ok Button Click event handler
    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    #endregion
}