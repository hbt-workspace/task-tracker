using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.EntityFrameworkCore;
using TaskTrackerAdminPanel.Models;

namespace TaskTrackerAdminPanel;

public partial class SearchWindow : Window
{

    #region Fields for Window Maximize and Taskbar Handling

    // Fields for Window Maximize and Taskbar Handling
    private const int WM_GETMINMAXINFO = 0x0024;
    private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

    #endregion

    #region Constructors

    // Constructor
    public SearchWindow()
    {
        InitializeComponent();
        SourceInitialized += MainWindow_SourceInitialized;
        Loaded += SearchWindow_Loaded;

    }

    #endregion

    #region Methods for Window Maximize and Taskbar Handling

    // Registers a Windows message hook when the window is initialized. And this allows us to handle the maximize behavior of our custom window.
    private void MainWindow_SourceInitialized(object? sender, EventArgs e)
    {
        IntPtr handle = new WindowInteropHelper(this).Handle;
        HwndSource? source = HwndSource.FromHwnd(handle);
        source?.AddHook(WindowProc);
    }

    // Processes Windows messages received by the custom window.
    private IntPtr WindowProc(
        IntPtr hwnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled)
    {
        if (msg == WM_GETMINMAXINFO)
        {
            UpdateMaximizedBounds(hwnd, lParam);
            handled = true;
        }

        return IntPtr.Zero;
    }

    // Calculates the maximum size and position of the window based on the current monitor.
    private void UpdateMaximizedBounds(IntPtr hwnd, IntPtr lParam)
    {
        IntPtr monitor = MonitorFromWindow(
            hwnd, MONITOR_DEFAULTTONEAREST);

        if (monitor == IntPtr.Zero)
            return;

        MONITORINFO monitorInfo = new MONITORINFO();
        monitorInfo.cbSize = Marshal.SizeOf<MONITORINFO>();

        if (!GetMonitorInfo(monitor, ref monitorInfo))
            return;

        MINMAXINFO info =
            Marshal.PtrToStructure<MINMAXINFO>(lParam);

        info.ptMaxPosition.x =
            monitorInfo.rcWork.Left - monitorInfo.rcMonitor.Left;
        info.ptMaxPosition.y =
            monitorInfo.rcWork.Top - monitorInfo.rcMonitor.Top;

        info.ptMaxSize.x =
            monitorInfo.rcWork.Right - monitorInfo.rcWork.Left;
        info.ptMaxSize.y =
            monitorInfo.rcWork.Bottom - monitorInfo.rcWork.Top;

        Marshal.StructureToPtr(info, lParam, false);
    }

    // Imports the Windows API function that identifies the monitor that contains the specified window.
    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(
        IntPtr hwnd, uint dwFlags);

    // Imports the Windows API function that retrieves information about a display monitor.
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(
        IntPtr hMonitor, ref MONITORINFO lpmi);

    // Structure to represent a point in a two-dimensional plane.
    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    // Contains the minimum and maximum tracking sizes and the maximum position and size of a window.
    [StructLayout(LayoutKind.Sequential)]
    private struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    // Represents the boundaries of a rectangle using its left, top, right, and bottom coordinates.
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    // Contains information about a display monitor.
    [StructLayout(LayoutKind.Sequential)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    #endregion

    #region Event Handlers


    // Searches reports using the selected employee and date range.
    private async void SearchReportsButton_Click(object sender, RoutedEventArgs e)
    {
        // Check that both dates have been selected.
        if (FromDatePicker.SelectedDate == null ||
            ToDatePicker.SelectedDate == null)
        {
            CustomMessageBox.Show(
                "Please select both From Date and To Date.",
                "Missing Dates",
                this);

            return;
        }

        DateTime fromDate = FromDatePicker.SelectedDate.Value.Date;
        DateTime toDate = ToDatePicker.SelectedDate.Value.Date;

        // Make sure the date range is valid.
        if (fromDate > toDate)
        {
            CustomMessageBox.Show(
                "From Date cannot be later than To Date.\nPlease check your selected dates.",
                "Invalid Date Range",
                this);

            return;
        }

        try
        {
            using var db = ((App)Application.Current).CreateDbContext();

            // Get reports within the selected date range.
            var query =
                from task in db.EmployeeTasks.AsNoTracking()
                join user in db.Users.AsNoTracking()
                    on task.UserId equals user.Id
                where !task.IsDeleted
                      && !user.IsDeleted
                      && user.IsActive
                      && task.CreatedOn.HasValue
                      && task.CreatedOn.Value >= fromDate
                      && task.CreatedOn.Value < toDate.AddDays(1)
                select new { task, user };

            // Filter by employee if ALL is not selected.
            if (EmployeeComboBox.SelectedValue is int employeeId)
            {
                query = query.Where(item => item.user.Id == employeeId);
            }

            // Retrieve matching reports from the database.
            var reports = await query
                .OrderBy(item => item.task.CreatedOn)
                .ThenBy(item => item.user.UserName)
                .Select(item => new EmployeeReportResult
                {
                    EmployeeName = item.user.UserName,
                    YesterdaysTask = item.task.YesterdaysTask,
                    TodaysTask = item.task.TodaysTask,
                    Obstacle = item.task.Obstacle,
                    ReportDate = item.task.CreatedOn
                })
                .ToListAsync();

            // Group reports by date.
            var groupedReports = reports
                .GroupBy(report => report.ReportDate?.Date)
                .OrderBy(group => group.Key)
                .ToList();

            SearchResultsItemsControl.ItemsSource = groupedReports;

            // Show a message if no reports were found.
            if (reports.Count == 0)
            {
                CustomMessageBox.Show(
                    "No reports found for the selected employee and date range.",
                    "No Results",
                    this);
            }
        }
        catch (Exception)
        {
            CustomMessageBox.Show(
                $"Unable to search reports.",
                "Something Went Wrong",
                this);
        }
    }


    // Allows the user to drag the borderless window.
    private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
        {
            if (e.ClickCount == 2)
            {
                MaximizeButton_Click(sender, e);
                return;
            }

            DragMove();
        }
    }

    // Minimizes the Search Window.
    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    // Maximizes or restores the Search Window.
    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
            MaximizeButton.Content = "□";
        }
        else
        {
            WindowState = WindowState.Maximized;
            MaximizeButton.Content = "❐";
        }
    }

    // Closes the Search Window.
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    // Returns to the main Employee Reports window.
    private void HomeButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        Application.Current.MainWindow = mainWindow;
        mainWindow.Show();
        Close();
    }

    #endregion

    #region Private Methods for Loading Employee Reports

    private async void SearchWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadEmployeesAsync();
    }

    // Loads the employees from the database and displays them in the employee dropdown.
    private async Task LoadEmployeesAsync()
    {
        try
        {
            using var db = ((App)Application.Current).CreateDbContext();

            var employees = await db.Users
                .AsNoTracking()
                .Where(user => user.IsActive && !user.IsDeleted)
                .OrderBy(user => user.UserName)
                .Select(user => new EmployeeDropdownModel
                {
                    Id = user.Id,
                    UserName = user.UserName
                })
                .ToListAsync();

            // Add ALL as the first dropdown option.
            employees.Insert(0, new EmployeeDropdownModel
            {
                Id = null,
                UserName = "ALL"
            });

            EmployeeComboBox.ItemsSource = employees;
            EmployeeComboBox.SelectedIndex = 0;
        }
        catch (Exception)
        {
            CustomMessageBox.Show(
                $"Unable to load employees.",
                "Employee Loading Error",
                this);
        }
    }

    #endregion
}