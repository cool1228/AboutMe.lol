using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SelectXYZ_Cheat.Core
{
    public class ProcessMonitor
    {
        private readonly MemoryReader memoryReader;
        private CancellationTokenSource? cancellationTokenSource;
        private bool isMonitoring = false;

        public event EventHandler<bool>? RobloxStatusChanged;
        public event EventHandler<string>? StatusMessage;

        public bool IsRobloxRunning { get; private set; } = false;

        public ProcessMonitor(MemoryReader memoryReader)
        {
            this.memoryReader = memoryReader;
        }

        public void StartMonitoring()
        {
            if (isMonitoring) return;

            isMonitoring = true;
            cancellationTokenSource = new CancellationTokenSource();
            
            Task.Run(() => MonitorLoop(cancellationTokenSource.Token));
            OnStatusMessage("Process monitoring started...");
        }

        public void StopMonitoring()
        {
            if (!isMonitoring) return;

            isMonitoring = false;
            cancellationTokenSource?.Cancel();
            OnStatusMessage("Process monitoring stopped.");
        }

        private async Task MonitorLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    bool wasRunning = IsRobloxRunning;
                    IsRobloxRunning = await CheckRobloxProcessAsync();

                    if (wasRunning != IsRobloxRunning)
                    {
                        OnRobloxStatusChanged(IsRobloxRunning);
                        
                        if (IsRobloxRunning)
                        {
                            OnStatusMessage("Roblox detected! Attempting to attach...");
                            if (memoryReader.AttachToRoblox())
                            {
                                OnStatusMessage("Successfully attached to Roblox process.");
                            }
                            else
                            {
                                OnStatusMessage("Failed to attach to Roblox process.");
                            }
                        }
                        else
                        {
                            OnStatusMessage("Roblox process not found.");
                            memoryReader.Detach();
                        }
                    }

                    // Check if we're still attached to a valid process
                    if (IsRobloxRunning && !memoryReader.IsAttached)
                    {
                        OnStatusMessage("Lost connection to Roblox. Attempting to reconnect...");
                        if (memoryReader.AttachToRoblox())
                        {
                            OnStatusMessage("Reconnected to Roblox process.");
                        }
                    }

                    await Task.Delay(2000, cancellationToken); // Check every 2 seconds
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    OnStatusMessage($"Error in process monitoring: {ex.Message}");
                    await Task.Delay(5000, cancellationToken); // Wait longer on error
                }
            }
        }

        private async Task<bool> CheckRobloxProcessAsync()
        {
            return await Task.Run(() =>
            {
                // Method 1: Check using Process.GetProcessesByName
                var processes = Process.GetProcessesByName("RobloxPlayerBeta");
                if (processes.Length > 0)
                {
                    foreach (var process in processes)
                        process.Dispose();
                    return true;
                }

                // Method 2: Check UWP version
                processes = Process.GetProcessesByName("Windows10Universal");
                if (processes.Length > 0)
                {
                    foreach (var process in processes)
                        process.Dispose();
                    return true;
                }

                // Method 3: Use CMD to check (as requested)
                return CheckRobloxUsingCmd();
            });
        }

        private bool CheckRobloxUsingCmd()
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c tasklist /FI \"IMAGENAME eq RobloxPlayerBeta.exe\" 2>NUL | find /I \"RobloxPlayerBeta.exe\" >NUL && echo FOUND || echo NOTFOUND",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processInfo);
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    
                    return output.Trim().Equals("FOUND", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                OnStatusMessage($"CMD check failed: {ex.Message}");
            }

            return false;
        }

        public async Task<ProcessInfo?> GetRobloxProcessInfoAsync()
        {
            return await Task.Run(() =>
            {
                var processes = Process.GetProcessesByName("RobloxPlayerBeta");
                if (processes.Length == 0)
                {
                    processes = Process.GetProcessesByName("Windows10Universal");
                }

                if (processes.Length > 0)
                {
                    var process = processes[0];
                    var info = new ProcessInfo
                    {
                        ProcessId = process.Id,
                        ProcessName = process.ProcessName,
                        WindowTitle = process.MainWindowTitle,
                        WindowHandle = process.MainWindowHandle,
                        IsResponding = process.Responding,
                        StartTime = process.StartTime,
                        WorkingSet = process.WorkingSet64
                    };

                    foreach (var p in processes)
                        p.Dispose();

                    return info;
                }

                return null;
            });
        }

        protected virtual void OnRobloxStatusChanged(bool isRunning)
        {
            RobloxStatusChanged?.Invoke(this, isRunning);
        }

        protected virtual void OnStatusMessage(string message)
        {
            StatusMessage?.Invoke(this, message);
        }
    }

    public class ProcessInfo
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string WindowTitle { get; set; } = string.Empty;
        public IntPtr WindowHandle { get; set; }
        public bool IsResponding { get; set; }
        public DateTime StartTime { get; set; }
        public long WorkingSet { get; set; }
    }
}