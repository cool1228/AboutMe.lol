using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SelectXYZ_Cheat.Core;
using SelectXYZ_Cheat.ESP;
using SelectXYZ_Cheat.Overlay;
using System.Windows.Controls;

namespace SelectXYZ_Cheat
{
    public partial class MainWindow : Window
    {
        private MemoryReader memoryReader;
        private ProcessMonitor processMonitor;
        private OverlayWindow? overlayWindow;
        private System.Windows.Threading.DispatcherTimer espUpdateTimer;
        private bool isOverlayActive = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            SetupEventHandlers();
            SetupTimers();
        }

        private void InitializeServices()
        {
            memoryReader = new MemoryReader();
            processMonitor = new ProcessMonitor(memoryReader);
            
            // Initialize ESP Manager
            ESPManager.Instance.Initialize(memoryReader);
        }

        private void SetupEventHandlers()
        {
            processMonitor.RobloxStatusChanged += OnRobloxStatusChanged;
            processMonitor.StatusMessage += OnStatusMessage;
            
            // Bind ESP settings to GUI controls
            BindESPSettings();
        }

        private void BindESPSettings()
        {
            var settings = ESPManager.Instance.Settings;
            
            // Find controls and bind them to ESP settings
            if (FindName("SkeletonESPCheckBox") is CheckBox skeletonCheckBox)
                skeletonCheckBox.DataContext = settings;
            
            if (FindName("HealthBarCheckBox") is CheckBox healthBarCheckBox)
                healthBarCheckBox.DataContext = settings;
            
            if (FindName("PlayerNameCheckBox") is CheckBox playerNameCheckBox)
                playerNameCheckBox.DataContext = settings;
            
            if (FindName("DistanceESPCheckBox") is CheckBox distanceCheckBox)
                distanceCheckBox.DataContext = settings;
            
            if (FindName("HeadCircleCheckBox") is CheckBox headCircleCheckBox)
                headCircleCheckBox.DataContext = settings;
            
            if (FindName("BoxTypeComboBox") is ComboBox boxTypeComboBox)
            {
                boxTypeComboBox.ItemsSource = Enum.GetValues(typeof(ESPBoxType));
                boxTypeComboBox.DataContext = settings;
            }
        }

        private void SetupTimers()
        {
            espUpdateTimer = new System.Windows.Threading.DispatcherTimer();
            espUpdateTimer.Interval = TimeSpan.FromMilliseconds(50); // 20 FPS update
            espUpdateTimer.Tick += async (s, e) => await UpdateESP();
        }

        private async Task UpdateESP()
        {
            if (memoryReader.IsAttached && isOverlayActive)
            {
                await ESPManager.Instance.UpdatePlayersAsync();
            }
        }

        private void OnRobloxStatusChanged(object? sender, bool isRunning)
        {
            Dispatcher.Invoke(() =>
            {
                if (isRunning)
                {
                    ShowStatusMessage("Roblox detected! Starting overlay...");
                    StartOverlay();
                }
                else
                {
                    ShowStatusMessage("Roblox not found. Stopping overlay...");
                    StopOverlay();
                }
            });
        }

        private void OnStatusMessage(object? sender, string message)
        {
            Dispatcher.Invoke(() => ShowStatusMessage(message));
        }

        private void ShowStatusMessage(string message)
        {
            // You could add a status bar or console output here
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        private void StartOverlay()
        {
            if (isOverlayActive) return;

            try
            {
                var robloxHandle = memoryReader.GetRobloxWindowHandle();
                if (robloxHandle != IntPtr.Zero)
                {
                    overlayWindow = new OverlayWindow();
                    overlayWindow.StartOverlay(robloxHandle);
                    
                    espUpdateTimer.Start();
                    isOverlayActive = true;
                    
                    ShowStatusMessage("Overlay started successfully!");
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Failed to start overlay: {ex.Message}");
            }
        }

        private void StopOverlay()
        {
            if (!isOverlayActive) return;

            try
            {
                espUpdateTimer.Stop();
                overlayWindow?.StopOverlay();
                overlayWindow?.Close();
                overlayWindow = null;
                
                ESPManager.Instance.ClearPlayers();
                isOverlayActive = false;
                
                ShowStatusMessage("Overlay stopped.");
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Error stopping overlay: {ex.Message}");
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Start monitoring for Roblox process
            processMonitor.StartMonitoring();
            ShowStatusMessage("Application started. Monitoring for Roblox...");
        }

        protected override void OnClosed(EventArgs e)
        {
            // Clean up resources
            processMonitor.StopMonitoring();
            StopOverlay();
            memoryReader.Detach();
            
            base.OnClosed(e);
        }

        // Event handlers for ESP settings changes
        private void OnESPSettingChanged(object sender, RoutedEventArgs e)
        {
            // Settings are automatically bound, no additional action needed
            // The ESP renderer will pick up changes from the ESPSettings object
        }
    }
}