using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SelectXYZ_Cheat
{
    // C# doesn't have uintptr_t, so we use UIntPtr
    using uintptr_t = UIntPtr;

    #region Windows API
    public static class WinAPI
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, uint processId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hwnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_QUERY_INFORMATION = 0x0400;
    }
    #endregion

    #region Roblox Offsets
    // Roblox Version: version-b8550645b8834e8a
    public static class RobloxOffsets
    {
        public const uintptr_t Adornee = 0xD8;
        public const uintptr_t AnchoredMask = 0x2;
        public const uintptr_t AnimationId = 0xD8;
        public const uintptr_t AttributeList = 0x138;
        public const uintptr_t AttributeToNext = 0x70;
        public const uintptr_t AttributeToValue = 0x30;
        public const uintptr_t AutoJumpEnabled = 0x1E3;
        public const uintptr_t BeamBrightness = 0x198;
        public const uintptr_t BeamColor = 0x128;
        public const uintptr_t BeamLightEmission = 0x1A4;
        public const uintptr_t BeamLightInfuence = 0x1A8;
        public const uintptr_t Camera = 0x428;
        public const uintptr_t CameraMaxZoomDistance = 0x2B8;
        public const uintptr_t CameraMinZoomDistance = 0x2BC;
        public const uintptr_t CameraMode = 0x2C0;
        public const uintptr_t CameraPos = 0x124;
        public const uintptr_t CameraRotation = 0x100;
        public const uintptr_t CameraSubject = 0xF0;
        public const uintptr_t CameraType = 0x160;
        public const uintptr_t CanCollideMask = 0x8;
        public const uintptr_t CanTouchMask = 0x10;
        public const uintptr_t CharacterAppearanceId = 0x260;
        public const uintptr_t Children = 0x80;
        public const uintptr_t ChildrenEnd = 0x8;
        public const uintptr_t ClassDescriptor = 0x18;
        public const uintptr_t ClassDescriptorToClassName = 0x8;
        public const uintptr_t ClickDetectorMaxActivationDistance = 0x118;
        public const uintptr_t ClockTime = 0x1C0;
        public const uintptr_t CreatorId = 0x190;
        public const uintptr_t DataModelDeleterPointer = 0x6ED6E40;
        public const uintptr_t DataModelPrimitiveCount = 0x410;
        public const uintptr_t DataModelToRenderView1 = 0x1D8;
        public const uintptr_t DataModelToRenderView2 = 0x8;
        public const uintptr_t DataModelToRenderView3 = 0x28;
        public const uintptr_t DecalTexture = 0x1A0;
        public const uintptr_t Deleter = 0x10;
        public const uintptr_t DeleterBack = 0x18;
        public const uintptr_t Dimensions = 0x720;
        public const uintptr_t DisplayName = 0x118;
        public const uintptr_t EvaluateStateMachine = 0x1E3;
        public const uintptr_t FOV = 0x168;
        public const uintptr_t FakeDataModelPointer = 0x6ED6E38;
        public const uintptr_t FakeDataModelToDataModel = 0x1C0;
        public const uintptr_t FogColor = 0x104;
        public const uintptr_t FogEnd = 0x13C;
        public const uintptr_t FogStart = 0x140;
        public const uintptr_t ForceNewAFKDuration = 0x1F8;
        public const uintptr_t FramePositionOffsetX = 0x3C4;
        public const uintptr_t FramePositionOffsetY = 0x3CC;
        public const uintptr_t FramePositionX = 0x3C0;
        public const uintptr_t FramePositionY = 0x3C8;
        public const uintptr_t FrameRotation = 0x190;
        public const uintptr_t FrameSizeX = 0x120;
        public const uintptr_t FrameSizeY = 0x124;
        public const uintptr_t GameId = 0x198;
        public const uintptr_t GameLoaded = 0x680;
        public const uintptr_t Gravity = 0x968;
        public const uintptr_t Health = 0x19C;
        public const uintptr_t HealthDisplayDistance = 0x2E0;
        public const uintptr_t HipHeight = 0x1A8;
        public const uintptr_t HumanoidDisplayName = 0xD8;
        public const uintptr_t HumanoidState = 0x870;
        public const uintptr_t HumanoidStateId = 0x20;
        public const uintptr_t InputObject = 0x108;
        public const uintptr_t InsetMaxX = 0x108;
        public const uintptr_t InsetMaxY = 0x10C;
        public const uintptr_t InsetMinX = 0x100;
        public const uintptr_t InsetMinY = 0x104;
        public const uintptr_t InstanceCapabilities = 0x380;
        public const uintptr_t JobEnd = 0x1D8;
        public const uintptr_t JobId = 0x140;
        public const uintptr_t JobStart = 0x1D0;
        public const uintptr_t Job_Name = 0x18;
        public const uintptr_t JobsPointer = 0x6fa7280;
        public const uintptr_t JumpPower = 0x1B8;
        public const uintptr_t LocalPlayer = 0x128;
        public const uintptr_t LocalScriptByteCode = 0x1B0;
        public const uintptr_t LocalScriptBytecodePointer = 0x10;
        public const uintptr_t LocalScriptHash = 0x1C0;
        public const uintptr_t MaterialType = 0x0;
        public const uintptr_t MaxHealth = 0x1BC;
        public const uintptr_t MaxSlopeAngle = 0x1C0;
        public const uintptr_t MeshPartColor3 = 0xADA8;
        public const uintptr_t ModelInstance = 0x328;
        public const uintptr_t ModuleScriptByteCode = 0x158;
        public const uintptr_t ModuleScriptBytecodePointer = 0x10;
        public const uintptr_t ModuleScriptHash = 0x180;
        public const uintptr_t MoonTextureId = 0xE0;
        public const uintptr_t MousePosition = 0xF4;
        public const uintptr_t MouseSensitivity = 0x6f4d9b4;
        public const uintptr_t MoveDirection = 0x160;
        public const uintptr_t Name = 0x78;
        public const uintptr_t NameDisplayDistance = 0x2EC;
        public const uintptr_t NameSize = 0x10;
        public const uintptr_t OnDemandInstance = 0x30;
        public const uintptr_t OutdoorAmbient = 0x110;
        public const uintptr_t Parent = 0x50;
        public const uintptr_t PartSize = 0x25C;
        public const uintptr_t Ping = 0xD0;
        public const uintptr_t PlaceId = 0x1A0;
        public const uintptr_t PlayerConfigurerPointer = 0x6EB4238;
        public const uintptr_t PlayerMouse = 0xC00;
        public const uintptr_t Position = 0x14C;
        public const uintptr_t Primitive = 0x178;
        public const uintptr_t PrimitiveGravity = 0x120;
        public const uintptr_t PrimitiveValidateValue = 0x6;
        public const uintptr_t PrimitivesPointer1 = 0x3B0;
        public const uintptr_t PrimitivesPointer2 = 0x210;
        public const uintptr_t ProximityPromptActionText = 0xD8;
        public const uintptr_t ProximityPromptEnabled = 0x14A;
        public const uintptr_t ProximityPromptGamepadKeyCode = 0x134;
        public const uintptr_t ProximityPromptHoldDuraction = 0x138;
        public const uintptr_t ProximityPromptMaxActivationDistance = 0x140;
        public const uintptr_t ProximityPromptMaxObjectText = 0xF8;
        public const uintptr_t RenderJobToDataModel = 0x1B0;
        public const uintptr_t RenderJobToFakeDataModel = 0x38;
        public const uintptr_t RenderJobToRenderView = 0x218;
        public const uintptr_t RequireBypass = 0x7E8;
        public const uintptr_t RigType = 0x1D0;
        public const uintptr_t Rotation = 0x130;
        public const uintptr_t RunContext = 0x150;
        public const uintptr_t ScriptContext = 0x3C0;
        public const uintptr_t Sit = 0x1E3;
        public const uintptr_t SkyboxBk = 0x108;
        public const uintptr_t SkyboxDn = 0x130;
        public const uintptr_t SkyboxFt = 0x158;
        public const uintptr_t SkyboxLf = 0x180;
        public const uintptr_t SkyboxRt = 0x1A8;
        public const uintptr_t SkyboxUp = 0x1D0;
        public const uintptr_t SoundId = 0xE0;
        public const uintptr_t StarCount = 0x228;
        public const uintptr_t StringLength = 0x10;
        public const uintptr_t SunTextureId = 0x1F8;
        public const uintptr_t TagList = 0x120;
        public const uintptr_t TaskSchedulerMaxFPS = 0x1B0;
        public const uintptr_t TaskSchedulerPointer = 0x6FA70A0;
        public const uintptr_t Team = 0x248;
        public const uintptr_t TeamColor = 0xD8;
        public const uintptr_t Tool_Grip_Position = 0x45C;
        public const uintptr_t Transparency = 0xF8;
        public const uintptr_t UserId = 0x270;
        public const uintptr_t Value = 0xD8;
        public const uintptr_t Velocity = 0x158;
        public const uintptr_t ViewportSize = 0x300;
        public const uintptr_t VisualEngine = 0x10;
        public const uintptr_t VisualEnginePointer = 0x6D18608;
        public const uintptr_t VisualEngineToDataModel1 = 0x700;
        public const uintptr_t VisualEngineToDataModel2 = 0x1C0;
        public const uintptr_t WalkSpeed = 0x1DC;
        public const uintptr_t WalkSpeedCheck = 0x3B8;
        public const uintptr_t WhitelistedThreads = 0x0; // Encrypted
        public const uintptr_t Workspace = 0x180;
        public const uintptr_t WorkspaceToWorld = 0x3B0;
        public const uintptr_t ViewMatrix = 0x4B0;
    }
    #endregion

    #region Data Models
    public class Player
    {
        public string Name { get; set; } = string.Empty;
        public Vector3 Position { get; set; }
        public Vector3 HeadPosition { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public float Distance { get; set; }
        public bool IsVisible { get; set; }
        public bool IsLocalPlayer { get; set; }
        public Vector2 ScreenPosition { get; set; }
        public Vector2 HeadScreenPosition { get; set; }
        public BoundingBox2D BoundingBox { get; set; }
        
        public float HealthPercentage => MaxHealth > 0 ? Health / MaxHealth : 0;
        public bool IsAlive => Health > 0;
    }

    public struct BoundingBox2D
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }
        
        public float Width => Right - Left;
        public float Height => Bottom - Top;
        public Vector2 Center => new Vector2(Left + Width / 2, Top + Height / 2);
    }

    public struct ViewMatrix
    {
        public float M11, M12, M13, M14;
        public float M21, M22, M23, M24;
        public float M31, M32, M33, M34;
        public float M41, M42, M43, M44;
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
    #endregion

    #region Memory Reader
    public class MemoryReader
    {
        private IntPtr processHandle;
        private Process? robloxProcess;

        public bool IsAttached => processHandle != IntPtr.Zero && robloxProcess != null && !robloxProcess.HasExited;

        public bool AttachToRoblox()
        {
            try
            {
                var processes = Process.GetProcessesByName("RobloxPlayerBeta");
                if (processes.Length == 0)
                {
                    processes = Process.GetProcessesByName("Windows10Universal");
                }

                if (processes.Length > 0)
                {
                    robloxProcess = processes[0];
                    processHandle = WinAPI.OpenProcess(WinAPI.PROCESS_VM_READ | WinAPI.PROCESS_QUERY_INFORMATION, false, (uint)robloxProcess.Id);
                    return processHandle != IntPtr.Zero;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error attaching to Roblox: {ex.Message}");
            }

            return false;
        }

        public void Detach()
        {
            if (processHandle != IntPtr.Zero)
            {
                WinAPI.CloseHandle(processHandle);
                processHandle = IntPtr.Zero;
            }
            robloxProcess = null;
        }

        public T ReadMemory<T>(IntPtr address) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] buffer = new byte[size];

            if (WinAPI.ReadProcessMemory(processHandle, address, buffer, size, out _))
            {
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
                }
                finally
                {
                    handle.Free();
                }
            }

            return default(T);
        }

        public byte[] ReadBytes(IntPtr address, int size)
        {
            byte[] buffer = new byte[size];
            WinAPI.ReadProcessMemory(processHandle, address, buffer, size, out _);
            return buffer;
        }

        public string ReadString(IntPtr address, int maxLength = 256)
        {
            byte[] buffer = ReadBytes(address, maxLength);
            int nullIndex = Array.IndexOf(buffer, (byte)0);
            if (nullIndex >= 0)
            {
                Array.Resize(ref buffer, nullIndex);
            }
            return Encoding.UTF8.GetString(buffer);
        }

        public IntPtr ReadPointer(IntPtr address)
        {
            return ReadMemory<IntPtr>(address);
        }

        public float ReadFloat(IntPtr address)
        {
            return ReadMemory<float>(address);
        }

        public int ReadInt32(IntPtr address)
        {
            return ReadMemory<int>(address);
        }

        public IntPtr GetRobloxWindowHandle()
        {
            if (robloxProcess != null)
            {
                return robloxProcess.MainWindowHandle;
            }
            return IntPtr.Zero;
        }

        public WinAPI.RECT GetRobloxWindowBounds()
        {
            var handle = GetRobloxWindowHandle();
            if (handle != IntPtr.Zero)
            {
                WinAPI.GetWindowRect(handle, out WinAPI.RECT rect);
                return rect;
            }
            return new WinAPI.RECT();
        }
    }
    #endregion

    #region Process Monitor
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

                    if (IsRobloxRunning && !memoryReader.IsAttached)
                    {
                        OnStatusMessage("Lost connection to Roblox. Attempting to reconnect...");
                        if (memoryReader.AttachToRoblox())
                        {
                            OnStatusMessage("Reconnected to Roblox process.");
                        }
                    }

                    await Task.Delay(2000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    OnStatusMessage($"Error in process monitoring: {ex.Message}");
                    await Task.Delay(5000, cancellationToken);
                }
            }
        }

        private async Task<bool> CheckRobloxProcessAsync()
        {
            return await Task.Run(() =>
            {
                var processes = Process.GetProcessesByName("RobloxPlayerBeta");
                if (processes.Length > 0)
                {
                    foreach (var process in processes)
                        process.Dispose();
                    return true;
                }

                processes = Process.GetProcessesByName("Windows10Universal");
                if (processes.Length > 0)
                {
                    foreach (var process in processes)
                        process.Dispose();
                    return true;
                }

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

        protected virtual void OnRobloxStatusChanged(bool isRunning)
        {
            RobloxStatusChanged?.Invoke(this, isRunning);
        }

        protected virtual void OnStatusMessage(string message)
        {
            StatusMessage?.Invoke(this, message);
        }
    }
    #endregion

    #region Roblox Memory Scanner
    public class RobloxMemoryScanner
    {
        private readonly MemoryReader memoryReader;
        private IntPtr dataModelAddress = IntPtr.Zero;
        private IntPtr workspaceAddress = IntPtr.Zero;
        private IntPtr localPlayerAddress = IntPtr.Zero;
        private IntPtr cameraAddress = IntPtr.Zero;

        public RobloxMemoryScanner(MemoryReader memoryReader)
        {
            this.memoryReader = memoryReader;
        }

        public bool Initialize()
        {
            try
            {
                dataModelAddress = GetDataModelAddress();
                if (dataModelAddress == IntPtr.Zero)
                    return false;

                workspaceAddress = GetWorkspaceAddress();
                localPlayerAddress = GetLocalPlayerAddress();
                cameraAddress = GetCameraAddress();

                return dataModelAddress != IntPtr.Zero;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Roblox scanner: {ex.Message}");
                return false;
            }
        }

        private IntPtr GetDataModelAddress()
        {
            try
            {
                var baseAddress = memoryReader.ReadPointer(new IntPtr((long)RobloxOffsets.FakeDataModelPointer));
                if (baseAddress != IntPtr.Zero)
                {
                    var dataModel = memoryReader.ReadPointer(baseAddress + (int)RobloxOffsets.FakeDataModelToDataModel);
                    if (IsValidDataModel(dataModel))
                        return dataModel;
                }

                var visualEngine = memoryReader.ReadPointer(new IntPtr((long)RobloxOffsets.VisualEnginePointer));
                if (visualEngine != IntPtr.Zero)
                {
                    var dataModel1 = memoryReader.ReadPointer(visualEngine + (int)RobloxOffsets.VisualEngineToDataModel1);
                    if (dataModel1 != IntPtr.Zero)
                    {
                        var dataModel = memoryReader.ReadPointer(dataModel1 + (int)RobloxOffsets.VisualEngineToDataModel2);
                        if (IsValidDataModel(dataModel))
                            return dataModel;
                    }
                }

                return IntPtr.Zero;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        private bool IsValidDataModel(IntPtr address)
        {
            if (address == IntPtr.Zero) return false;
            
            try
            {
                var gameLoaded = memoryReader.ReadInt32(address + (int)RobloxOffsets.GameLoaded);
                return gameLoaded >= 0 && gameLoaded <= 1;
            }
            catch
            {
                return false;
            }
        }

        private IntPtr GetWorkspaceAddress()
        {
            if (dataModelAddress == IntPtr.Zero) return IntPtr.Zero;
            
            try
            {
                return memoryReader.ReadPointer(dataModelAddress + (int)RobloxOffsets.Workspace);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        private IntPtr GetLocalPlayerAddress()
        {
            if (dataModelAddress == IntPtr.Zero) return IntPtr.Zero;
            
            try
            {
                return memoryReader.ReadPointer(dataModelAddress + (int)RobloxOffsets.LocalPlayer);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        private IntPtr GetCameraAddress()
        {
            if (workspaceAddress == IntPtr.Zero) return IntPtr.Zero;
            
            try
            {
                return memoryReader.ReadPointer(workspaceAddress + (int)RobloxOffsets.Camera);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        public List<Player> ScanForPlayers()
        {
            var players = new List<Player>();
            
            if (workspaceAddress == IntPtr.Zero)
                return players;

            try
            {
                var viewMatrix = GetViewMatrix();
                var children = GetChildren(workspaceAddress);
                
                foreach (var child in children)
                {
                    var player = TryGetPlayerFromInstance(child, viewMatrix);
                    if (player != null)
                    {
                        players.Add(player);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scanning for players: {ex.Message}");
            }

            return players;
        }

        private Matrix4x4 GetViewMatrix()
        {
            try
            {
                if (cameraAddress == IntPtr.Zero)
                    return Matrix4x4.Identity;

                var matrixBytes = memoryReader.ReadBytes(cameraAddress + (int)RobloxOffsets.ViewMatrix, 64);
                if (matrixBytes.Length >= 64)
                {
                    var matrix = new Matrix4x4();
                    var span = MemoryMarshal.Cast<byte, float>(matrixBytes);
                    
                    matrix.M11 = span[0]; matrix.M12 = span[1]; matrix.M13 = span[2]; matrix.M14 = span[3];
                    matrix.M21 = span[4]; matrix.M22 = span[5]; matrix.M23 = span[6]; matrix.M24 = span[7];
                    matrix.M31 = span[8]; matrix.M32 = span[9]; matrix.M33 = span[10]; matrix.M34 = span[11];
                    matrix.M41 = span[12]; matrix.M42 = span[13]; matrix.M43 = span[14]; matrix.M44 = span[15];
                    
                    return matrix;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading view matrix: {ex.Message}");
            }

            return Matrix4x4.Identity;
        }

        private List<IntPtr> GetChildren(IntPtr parent)
        {
            var children = new List<IntPtr>();
            
            try
            {
                var childrenStart = memoryReader.ReadPointer(parent + (int)RobloxOffsets.Children);
                var childrenEnd = memoryReader.ReadPointer(parent + (int)RobloxOffsets.Children + (int)RobloxOffsets.ChildrenEnd);
                
                if (childrenStart == IntPtr.Zero || childrenEnd == IntPtr.Zero)
                    return children;

                var current = childrenStart;
                var maxChildren = 1000;
                var count = 0;
                
                while (current != childrenEnd && count < maxChildren)
                {
                    var child = memoryReader.ReadPointer(current);
                    if (child != IntPtr.Zero)
                    {
                        children.Add(child);
                    }
                    
                    current += IntPtr.Size;
                    count++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting children: {ex.Message}");
            }

            return children;
        }

        private Player? TryGetPlayerFromInstance(IntPtr instance, Matrix4x4 viewMatrix)
        {
            try
            {
                var className = GetClassName(instance);
                if (className != "Model")
                    return null;

                var humanoid = FindChildByClassName(instance, "Humanoid");
                if (humanoid == IntPtr.Zero)
                    return null;

                var rootPart = FindChildByName(instance, "HumanoidRootPart");
                if (rootPart == IntPtr.Zero)
                    return null;

                var head = FindChildByName(instance, "Head");
                if (head == IntPtr.Zero)
                    return null;

                var player = new Player();
                
                player.Name = GetInstanceName(instance);
                player.Health = memoryReader.ReadFloat(humanoid + (int)RobloxOffsets.Health);
                player.MaxHealth = memoryReader.ReadFloat(humanoid + (int)RobloxOffsets.MaxHealth);
                player.Position = GetPartPosition(rootPart);
                player.HeadPosition = GetPartPosition(head);
                
                if (localPlayerAddress != IntPtr.Zero)
                {
                    var localCharacter = GetLocalPlayerCharacter();
                    if (localCharacter != IntPtr.Zero)
                    {
                        var localRootPart = FindChildByName(localCharacter, "HumanoidRootPart");
                        if (localRootPart != IntPtr.Zero)
                        {
                            var localPos = GetPartPosition(localRootPart);
                            player.Distance = Vector3.Distance(player.Position, localPos);
                        }
                    }
                }
                
                player.ScreenPosition = WorldToScreen(player.Position, viewMatrix);
                player.HeadScreenPosition = WorldToScreen(player.HeadPosition, viewMatrix);
                player.BoundingBox = CalculateBoundingBox(player);
                player.IsVisible = IsOnScreen(player.ScreenPosition);
                player.IsAlive = player.Health > 0;
                
                return player;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing player instance: {ex.Message}");
                return null;
            }
        }

        private string GetClassName(IntPtr instance)
        {
            try
            {
                var classDescriptor = memoryReader.ReadPointer(instance + (int)RobloxOffsets.ClassDescriptor);
                if (classDescriptor == IntPtr.Zero)
                    return string.Empty;

                var classNamePtr = memoryReader.ReadPointer(classDescriptor + (int)RobloxOffsets.ClassDescriptorToClassName);
                if (classNamePtr == IntPtr.Zero)
                    return string.Empty;

                return memoryReader.ReadString(classNamePtr, 50);
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetInstanceName(IntPtr instance)
        {
            try
            {
                var namePtr = memoryReader.ReadPointer(instance + (int)RobloxOffsets.Name);
                if (namePtr == IntPtr.Zero)
                    return string.Empty;

                var nameLength = memoryReader.ReadInt32(namePtr + (int)RobloxOffsets.NameSize);
                if (nameLength <= 0 || nameLength > 100)
                    return string.Empty;

                return memoryReader.ReadString(namePtr, nameLength);
            }
            catch
            {
                return string.Empty;
            }
        }

        private IntPtr FindChildByClassName(IntPtr parent, string className)
        {
            var children = GetChildren(parent);
            
            foreach (var child in children)
            {
                if (GetClassName(child) == className)
                    return child;
            }
            
            return IntPtr.Zero;
        }

        private IntPtr FindChildByName(IntPtr parent, string name)
        {
            var children = GetChildren(parent);
            
            foreach (var child in children)
            {
                if (GetInstanceName(child) == name)
                    return child;
            }
            
            return IntPtr.Zero;
        }

        private Vector3 GetPartPosition(IntPtr part)
        {
            try
            {
                var x = memoryReader.ReadFloat(part + (int)RobloxOffsets.Position);
                var y = memoryReader.ReadFloat(part + (int)RobloxOffsets.Position + 4);
                var z = memoryReader.ReadFloat(part + (int)RobloxOffsets.Position + 8);
                
                return new Vector3(x, y, z);
            }
            catch
            {
                return Vector3.Zero;
            }
        }

        private IntPtr GetLocalPlayerCharacter()
        {
            try
            {
                if (localPlayerAddress == IntPtr.Zero)
                    return IntPtr.Zero;

                return memoryReader.ReadPointer(localPlayerAddress + 0x120);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        private Vector2 WorldToScreen(Vector3 worldPos, Matrix4x4 viewMatrix)
        {
            try
            {
                var screenPos = Vector3.Transform(worldPos, viewMatrix);
                
                if (screenPos.Z > 0)
                {
                    var screenX = (screenPos.X / screenPos.Z + 1.0f) * 0.5f;
                    var screenY = (1.0f - screenPos.Y / screenPos.Z) * 0.5f;
                    
                    var viewportSize = GetViewportSize();
                    
                    return new Vector2(
                        screenX * viewportSize.X,
                        screenY * viewportSize.Y
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in WorldToScreen: {ex.Message}");
            }
            
            return new Vector2(-1, -1);
        }

        private Vector2 GetViewportSize()
        {
            try
            {
                if (cameraAddress == IntPtr.Zero)
                    return new Vector2(1920, 1080);

                var width = memoryReader.ReadFloat(cameraAddress + (int)RobloxOffsets.ViewportSize);
                var height = memoryReader.ReadFloat(cameraAddress + (int)RobloxOffsets.ViewportSize + 4);
                
                return new Vector2(width, height);
            }
            catch
            {
                return new Vector2(1920, 1080);
            }
        }

        private BoundingBox2D CalculateBoundingBox(Player player)
        {
            float distance = Math.Max(player.Distance, 1.0f);
            float scale = Math.Max(0.5f, 100.0f / distance);
            
            float width = 40 * scale;
            float height = 80 * scale;
            
            return new BoundingBox2D
            {
                Left = player.ScreenPosition.X - width / 2,
                Top = player.ScreenPosition.Y - height,
                Right = player.ScreenPosition.X + width / 2,
                Bottom = player.ScreenPosition.Y
            };
        }

        private bool IsOnScreen(Vector2 screenPos)
        {
            return screenPos.X >= 0 && screenPos.Y >= 0 && 
                   screenPos.X <= 1920 && screenPos.Y <= 1080;
        }
    }
    #endregion
}