using System.Windows;
using System.Windows.Media;

namespace TaskTrackerBot;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MessageBoxWindow : Window
{
    #region Constructor

    // Constructor 
    public MessageBoxWindow(string icon, string message, string description)
    {
        InitializeComponent();

        IconTextBlock.Text = icon;
        MessageTextBlock.Text = message;
        DescriptionTextBlock.Text = description;

        if (icon == "✔")
        {
            // Success state
            IconBorder.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#173D32"));

            IconTextBlock.Foreground =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4ADE80"));
        }
        else if (icon == "❌")
        {
            // Error state
            IconBorder.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#451F25"));

            IconTextBlock.Text = "✕";
            IconTextBlock.Foreground =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F87171"));
        }
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