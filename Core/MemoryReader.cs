using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SelectXYZ_Cheat.Core
{
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
}