using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

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

    #region Color Picker Control
    public class ColorPickerControl : Control
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(ColorPickerControl), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty DefaultColorProperty =
            DependencyProperty.Register("DefaultColor", typeof(string), typeof(ColorPickerControl), new PropertyMetadata("White", OnDefaultColorChanged));

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Brush), typeof(ColorPickerControl), new PropertyMetadata(Brushes.White));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public string DefaultColor
        {
            get { return (string)GetValue(DefaultColorProperty); }
            set { SetValue(DefaultColorProperty, value); }
        }

        public Brush SelectedColor
        {
            get { return (Brush)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        static ColorPickerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPickerControl), new FrameworkPropertyMetadata(typeof(ColorPickerControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_ColorDisplay") is FrameworkElement colorDisplay)
            {
                colorDisplay.MouseLeftButtonUp += ColorDisplay_MouseLeftButtonUp;
            }
        }

        private void ColorDisplay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var colors = new[]
            {
                Brushes.White, Brushes.Black, Brushes.Red, Brushes.Green, Brushes.Blue,
                Brushes.Yellow, Brushes.Cyan, Brushes.Magenta, Brushes.Orange, Brushes.Purple
            };

            var window = new ColorPickerWindow(colors);
            if (window.ShowDialog() == true)
            {
                SelectedColor = window.SelectedBrush;
            }
        }

        private static void OnDefaultColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPickerControl control)
            {
                control.SelectedColor = GetBrushFromColorName((string)e.NewValue);
            }
        }

        private static Brush GetBrushFromColorName(string colorName)
        {
            return colorName switch
            {
                "White" => Brushes.White,
                "Black" => Brushes.Black,
                "Red" => Brushes.Red,
                "Green" => Brushes.Green,
                "Blue" => Brushes.Blue,
                "Yellow" => Brushes.Yellow,
                "Cyan" => Brushes.Cyan,
                "Magenta" => Brushes.Magenta,
                "Orange" => Brushes.Orange,
                "Purple" => new SolidColorBrush(Color.FromRgb(110, 69, 226)),
                "#FF8080FF" => new SolidColorBrush(Color.FromRgb(128, 128, 255)),
                "#FF00FFFF" => Brushes.Cyan,
                _ => Brushes.White
            };
        }
    }

    public partial class ColorPickerWindow : Window
    {
        public Brush SelectedBrush { get; private set; } = Brushes.White;

        public ColorPickerWindow(Brush[] colors)
        {
            InitializeComponent(colors);
        }

        private void InitializeComponent(Brush[] colors)
        {
            Title = "Select Color";
            Width = 300;
            Height = 200;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Background = new SolidColorBrush(Color.FromRgb(18, 18, 18));

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var colorsPanel = new WrapPanel { Margin = new Thickness(10) };
            
            foreach (var color in colors)
            {
                var button = new Button
                {
                    Width = 40,
                    Height = 40,
                    Margin = new Thickness(5),
                    Background = color,
                    BorderThickness = new Thickness(2),
                    BorderBrush = Brushes.Gray
                };

                button.Click += (s, e) =>
                {
                    SelectedBrush = color;
                    DialogResult = true;
                    Close();
                };

                colorsPanel.Children.Add(button);
            }

            var scrollViewer = new ScrollViewer { Content = colorsPanel };
            Grid.SetRow(scrollViewer, 0);
            grid.Children.Add(scrollViewer);

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 80,
                Height = 30,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            cancelButton.Click += (s, e) =>
            {
                DialogResult = false;
                Close();
            };

            Grid.SetRow(cancelButton, 1);
            grid.Children.Add(cancelButton);

            Content = grid;
        }
    }
    #endregion
    }
}