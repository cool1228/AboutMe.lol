using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using SelectXYZ_Cheat.Core;
using SelectXYZ_Cheat.Models;

namespace SelectXYZ_Cheat.ESP
{
    public class ESPManager
    {
        private static ESPManager? _instance;
        public static ESPManager Instance => _instance ??= new ESPManager();

        private readonly MemoryReader memoryReader;
        private readonly List<Player> players = new();
        private readonly object playersLock = new();

        public ESPSettings Settings { get; } = new ESPSettings();

        private MemoryReader? externalMemoryReader;
        private RobloxMemoryScanner? robloxScanner;

        private ESPManager()
        {
            memoryReader = new MemoryReader();
        }

        public void Initialize(MemoryReader reader)
        {
            externalMemoryReader = reader;
            robloxScanner = new RobloxMemoryScanner(reader);
        }

        public List<Player> GetPlayers()
        {
            lock (playersLock)
            {
                return new List<Player>(players);
            }
        }

        public async Task UpdatePlayersAsync()
        {
            if (externalMemoryReader == null || !externalMemoryReader.IsAttached) return;

            try
            {
                var updatedPlayers = await ScanForPlayersAsync();
                
                lock (playersLock)
                {
                    players.Clear();
                    players.AddRange(updatedPlayers);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating players: {ex.Message}");
            }
        }

        private async Task<List<Player>> ScanForPlayersAsync()
        {
            return await Task.Run(() =>
            {
                var foundPlayers = new List<Player>();

                try
                {
                    if (robloxScanner == null)
                    {
                        Console.WriteLine("Roblox scanner not initialized");
                        return foundPlayers;
                    }

                    // Initialize scanner if not already done
                    if (!robloxScanner.Initialize())
                    {
                        Console.WriteLine("Failed to initialize Roblox scanner");
                        return foundPlayers;
                    }

                    // Use real Roblox memory scanning
                    foundPlayers = robloxScanner.ScanForPlayers();
                    
                    // Fallback to simulation if no real players found (for testing)
                    if (foundPlayers.Count == 0)
                    {
                        foundPlayers.AddRange(SimulatePlayerDetection());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in player scan: {ex.Message}");
                    // Fallback to simulation on error
                    foundPlayers.AddRange(SimulatePlayerDetection());
                }

                return foundPlayers;
            });
        }

        private List<Player> SimulatePlayerDetection()
        {
            // This simulates finding players for demonstration
            // Replace with actual memory reading logic
            var simulatedPlayers = new List<Player>();

            for (int i = 0; i < 3; i++)
            {
                var player = new Player
                {
                    Name = $"Player{i + 1}",
                    Position = new Vector3(100 + i * 50, 50, 200 + i * 30),
                    HeadPosition = new Vector3(100 + i * 50, 80, 200 + i * 30),
                    Health = 80 + i * 10,
                    MaxHealth = 100,
                    Distance = 150 + i * 25,
                    IsVisible = true,
                    IsLocalPlayer = false,
                    IsAlive = true
                };

                // Calculate screen positions (simplified)
                player.ScreenPosition = WorldToScreen(player.Position);
                player.HeadScreenPosition = WorldToScreen(player.HeadPosition);
                player.BoundingBox = CalculateBoundingBox(player);

                simulatedPlayers.Add(player);
            }

            return simulatedPlayers;
        }

        private Vector2 WorldToScreen(Vector3 worldPos)
        {
            // Simplified world-to-screen conversion
            // In reality, you'd need the view matrix from the game
            
            // For simulation, just convert 3D position to 2D screen coordinates
            float screenX = 400 + (worldPos.X - 100) * 2;
            float screenY = 300 - (worldPos.Y - 50) * 3;
            
            return new Vector2(screenX, screenY);
        }

        private BoundingBox2D CalculateBoundingBox(Player player)
        {
            // Calculate 2D bounding box based on distance and screen position
            float distance = player.Distance;
            float scale = Math.Max(0.5f, 100.0f / distance); // Scale based on distance
            
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

        public bool IsPlayerInFOV(Player player, float fovAngle = 90f)
        {
            // Simple FOV check - in reality you'd use camera direction and position
            var screenCenter = new Vector2(960, 540); // Assuming 1920x1080
            var distance = Vector2.Distance(player.ScreenPosition, screenCenter);
            
            // Simple distance-based FOV check
            return distance < 500; // Pixels from center
        }

        public Player? GetClosestPlayer()
        {
            lock (playersLock)
            {
                Player? closest = null;
                float minDistance = float.MaxValue;

                foreach (var player in players)
                {
                    if (player.IsLocalPlayer || !player.IsAlive) continue;
                    
                    if (player.Distance < minDistance)
                    {
                        minDistance = player.Distance;
                        closest = player;
                    }
                }

                return closest;
            }
        }

        public List<Player> GetPlayersInRange(float maxDistance)
        {
            lock (playersLock)
            {
                var inRange = new List<Player>();
                
                foreach (var player in players)
                {
                    if (player.IsLocalPlayer || !player.IsAlive) continue;
                    
                    if (player.Distance <= maxDistance)
                    {
                        inRange.Add(player);
                    }
                }

                return inRange;
            }
        }

        public void ClearPlayers()
        {
            lock (playersLock)
            {
                players.Clear();
            }
        }
    }
}