using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.EntityFrameworkCore;
using TaskTrackerAdminPanel.Models;
using TaskTrackerAdminPanel.Services;

namespace TaskTrackerAdminPanel;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    #region Fields for Window Maximize and Taskbar Handling

    // Fields for Window Maximize and Taskbar Handling
    private const int WM_GETMINMAXINFO = 0x0024;
    private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

    #endregion

    #region Constructors

    // Constructor 
    public MainWindow()
    {
        InitializeComponent();
        SourceInitialized += MainWindow_SourceInitialized;
        Loaded += async (_, _) => await LoadEmployeeReportsAsync();
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

    // Closes the window when the close button is clicked.
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    // Drags the window when the header is clicked.
    private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
            DragMove();
    }

    // Minimizes the window when the minimize button is clicked.
    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    // Maximizes or restores the window when the maximize button is clicked.
    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
            MaximizeButton.Content = "□";
            MaximizeButton.ToolTip = "Maximize";
        }
        else
        {
            // Keep the Windows taskbar visible
            SystemParameters.WorkArea.ToString();
            WindowState = WindowState.Maximized;
            MaximizeButton.Content = "❐";
            MaximizeButton.ToolTip = "Restore";
        }
    }

    // Refreshes the employee reports when the refresh button is clicked.
    private async void RefreshReportsButton_Click(object sender,RoutedEventArgs e)
    {
        await LoadEmployeeReportsAsync();
    }

    // Opens the Search Window when the navbar Search button is clicked.
    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        SearchWindow searchWindow = new SearchWindow();
        Application.Current.MainWindow = searchWindow;
        searchWindow.Show();
        Close();
    }

    #endregion

    #region Methods for Loading Employee Reports

    // Loads the employee reports from the database and displays them in the report cards.
    private async Task LoadEmployeeReportsAsync()
    {
        try
        {
            using var db = ((App)Application.Current).CreateDbContext();

            var reportService = new EmployeeReportService(db);

            List<EmployeeReportResult> reports =
                await reportService.GetEmployeeReportsAsync();

            // Display today's reports in the report cards.
            ReportsItemsControl.ItemsSource = reports;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to load employee reports.\n\n{ex.Message}",
                "Unable to Load Reports",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    #endregion

}