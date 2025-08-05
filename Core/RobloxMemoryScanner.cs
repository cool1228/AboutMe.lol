using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using SelectXYZ_Cheat.Models;

namespace SelectXYZ_Cheat.Core
{
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
                // Get base addresses using the provided offsets
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
                // Method 1: Try FakeDataModelPointer -> DataModel
                var baseAddress = memoryReader.ReadPointer(new IntPtr((long)RobloxOffsets.FakeDataModelPointer));
                if (baseAddress != IntPtr.Zero)
                {
                    var dataModel = memoryReader.ReadPointer(baseAddress + (int)RobloxOffsets.FakeDataModelToDataModel);
                    if (IsValidDataModel(dataModel))
                        return dataModel;
                }

                // Method 2: Try VisualEnginePointer -> DataModel
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
                // Check if we can read basic properties
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
                // Get view matrix for world-to-screen conversion
                var viewMatrix = GetViewMatrix();
                
                // Scan workspace children for player characters
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

                // Read the view matrix from camera
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
                var maxChildren = 1000; // Safety limit
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
                // Check if this is a Model (potential player character)
                var className = GetClassName(instance);
                if (className != "Model")
                    return null;

                // Look for Humanoid child
                var humanoid = FindChildByClassName(instance, "Humanoid");
                if (humanoid == IntPtr.Zero)
                    return null;

                // Look for HumanoidRootPart
                var rootPart = FindChildByName(instance, "HumanoidRootPart");
                if (rootPart == IntPtr.Zero)
                    return null;

                // Look for Head part
                var head = FindChildByName(instance, "Head");
                if (head == IntPtr.Zero)
                    return null;

                var player = new Player();
                
                // Get player name from model
                player.Name = GetInstanceName(instance);
                
                // Get health from humanoid
                player.Health = memoryReader.ReadFloat(humanoid + (int)RobloxOffsets.Health);
                player.MaxHealth = memoryReader.ReadFloat(humanoid + (int)RobloxOffsets.MaxHealth);
                
                // Get position from root part
                player.Position = GetPartPosition(rootPart);
                player.HeadPosition = GetPartPosition(head);
                
                // Calculate distance from local player
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
                
                // Convert world positions to screen coordinates
                player.ScreenPosition = WorldToScreen(player.Position, viewMatrix);
                player.HeadScreenPosition = WorldToScreen(player.HeadPosition, viewMatrix);
                
                // Calculate bounding box
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

                // Get character from local player
                return memoryReader.ReadPointer(localPlayerAddress + 0x120); // Character offset
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
                // Transform world position using view matrix
                var screenPos = Vector3.Transform(worldPos, viewMatrix);
                
                // Perspective divide
                if (screenPos.Z > 0)
                {
                    var screenX = (screenPos.X / screenPos.Z + 1.0f) * 0.5f;
                    var screenY = (1.0f - screenPos.Y / screenPos.Z) * 0.5f;
                    
                    // Get viewport size
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
            
            return new Vector2(-1, -1); // Off-screen
        }

        private Vector2 GetViewportSize()
        {
            try
            {
                if (cameraAddress == IntPtr.Zero)
                    return new Vector2(1920, 1080); // Default

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
            // Calculate 2D bounding box based on distance and screen position
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
                   screenPos.X <= 1920 && screenPos.Y <= 1080; // Adjust for actual screen size
        }
    }
}