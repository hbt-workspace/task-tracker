using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskTrackerBot.Services;

namespace TaskTrackerBot;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    #region Fields

    private int currentStep = 1;
    private string yesterdayTask = "";
    private string todayTask = "";
    private string obstacle = "";
    private int currentUserId;

    #endregion
    
    #region Constructor

    // Constructor 
    public MainWindow()
    {
        InitializeComponent();
        Loaded += async (sender, e) =>
        {
            var userService = new UserService();
            var user = await userService.GetOrCreateUserAsync();
            currentUserId = user.Id;
        };
    }

    #endregion

    #region Event Handlers

    // Window Mouse Left Button Down event handler
    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    // TitleBar Mouse Left Button Down event handler
    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    // CloseButton Click event handler
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    // NextButton Click event handler
    private async void NextButton_Click(object sender, RoutedEventArgs e)
    {
        string answer = AnswerTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(answer))
        {
            MessageBox.Show("Please enter your answer.");
            AnswerTextBox.Focus();
            return;
        }

        if (currentStep == 1)
        {
            yesterdayTask = answer;
            currentStep = 2;
            AnswerTextBox.Clear();

            var stackPanel = (StackPanel)AnswerTextBox.Parent;
            ((TextBlock)stackPanel.Children[0]).Text = "STEP 2 OF 3";
            ((TextBlock)stackPanel.Children[1]).Text = "What will you do today?";
            ((TextBlock)stackPanel.Children[2]).Text = "Describe the work you plan to complete today.";
        }
        else if (currentStep == 2)
        {
            todayTask = answer;
            currentStep = 3;
            AnswerTextBox.Clear();
            NextButton.Content = "Submit";

            var stackPanel = (StackPanel)AnswerTextBox.Parent;
            ((TextBlock)stackPanel.Children[0]).Text = "STEP 3 OF 3";
            ((TextBlock)stackPanel.Children[1]).Text = "Are you facing any obstacles?";
            ((TextBlock)stackPanel.Children[2]).Text = "Describe any blockers or challenges.";
        }
        else if (currentStep == 3)
        {
            obstacle = answer;

            try
            {
                var userService = new UserService();

                await userService.SaveEmployeeTasksAsync(
                    currentUserId,
                    yesterdayTask,
                    todayTask,
                    obstacle);

                MessageBox.Show("Your daily report has been submitted successfully!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not save your report:\n{ex.Message}",
                    "Save Error");
            }
        }
    }

    #endregion
}