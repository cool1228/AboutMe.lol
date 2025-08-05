using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using SelectXYZ_Cheat.Core;

namespace SelectXYZ_Cheat.Overlay
{
    public partial class OverlayWindow : Window
    {
        private IntPtr robloxHandle;
        private OverlayRenderer? renderer;
        private System.Windows.Threading.DispatcherTimer updateTimer;

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_EXSTYLE = -20;
        private const uint WS_EX_LAYERED = 0x80000;
        private const uint WS_EX_TRANSPARENT = 0x20;
        private const uint WS_EX_TOPMOST = 0x8;

        public OverlayWindow()
        {
            InitializeComponent();
            SetupOverlay();
            
            updateTimer = new System.Windows.Threading.DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            updateTimer.Tick += UpdateTimer_Tick;
        }

        private void InitializeComponent()
        {
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            ShowInTaskbar = false;
            Topmost = true;
            ResizeMode = ResizeMode.NoResize;
            
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Left = 0;
            Top = 0;
        }

        private void SetupOverlay()
        {
            var helper = new WindowInteropHelper(this);
            helper.EnsureHandle();
            
            var hwnd = helper.Handle;
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOPMOST);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                renderer = new OverlayRenderer(hwndSource.Handle);
            }
        }

        public void StartOverlay(IntPtr targetWindow)
        {
            robloxHandle = targetWindow;
            updateTimer.Start();
            Show();
        }

        public void StopOverlay()
        {
            updateTimer.Stop();
            Hide();
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (robloxHandle != IntPtr.Zero)
            {
                UpdatePosition();
                renderer?.Render();
            }
        }

        private void UpdatePosition()
        {
            if (WinAPI.GetWindowRect(robloxHandle, out WinAPI.RECT rect))
            {
                Left = rect.Left;
                Top = rect.Top;
                Width = rect.Right - rect.Left;
                Height = rect.Bottom - rect.Top;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            renderer?.Dispose();
            base.OnClosed(e);
        }
    }
}