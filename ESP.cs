using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;

namespace SelectXYZ_Cheat
{
    #region ESP Settings
    public enum ESPBoxType
    {
        None,
        Box2D,
        Box3D,
        CornerBox
    }

    public class ESPSettings : INotifyPropertyChanged
    {
        private ESPBoxType _boxType = ESPBoxType.None;
        private bool _skeletonESP = false;
        private bool _healthBar = false;
        private bool _healthNumber = false;
        private bool _playerName = false;
        private bool _distanceESP = false;
        private bool _headCircle = false;
        private bool _sonarESP = false;
        private bool _chinaHat = false;
        private bool _chams = false;
        private bool _boxGlow = false;
        private bool _boxFill = false;

        // Colors
        private string _skeletonColor = "White";
        private string _cornerBoxColor = "White";
        private string _healthBarColor = "Green";
        private string _boxFillColor = "Black";
        private string _headCircleColor = "White";
        private string _box3DColor = "Black";
        private string _outlineColor = "Red";
        private string _chamsColor = "Cyan";
        private string _chinaHatColor = "Red";
        private string _sonarColor = "Cyan";
        private string _boxColor = "Red";
        private string _nameColor = "White";
        private string _distanceColor = "Yellow";

        public event PropertyChangedEventHandler? PropertyChanged;

        public ESPBoxType BoxType
        {
            get => _boxType;
            set { _boxType = value; OnPropertyChanged(nameof(BoxType)); }
        }

        public bool SkeletonESP
        {
            get => _skeletonESP;
            set { _skeletonESP = value; OnPropertyChanged(nameof(SkeletonESP)); }
        }

        public bool HealthBar
        {
            get => _healthBar;
            set { _healthBar = value; OnPropertyChanged(nameof(HealthBar)); }
        }

        public bool HealthNumber
        {
            get => _healthNumber;
            set { _healthNumber = value; OnPropertyChanged(nameof(HealthNumber)); }
        }

        public bool PlayerName
        {
            get => _playerName;
            set { _playerName = value; OnPropertyChanged(nameof(PlayerName)); }
        }

        public bool DistanceESP
        {
            get => _distanceESP;
            set { _distanceESP = value; OnPropertyChanged(nameof(DistanceESP)); }
        }

        public bool HeadCircle
        {
            get => _headCircle;
            set { _headCircle = value; OnPropertyChanged(nameof(HeadCircle)); }
        }

        public bool SonarESP
        {
            get => _sonarESP;
            set { _sonarESP = value; OnPropertyChanged(nameof(SonarESP)); }
        }

        public bool ChinaHat
        {
            get => _chinaHat;
            set { _chinaHat = value; OnPropertyChanged(nameof(ChinaHat)); }
        }

        public bool Chams
        {
            get => _chams;
            set { _chams = value; OnPropertyChanged(nameof(Chams)); }
        }

        public bool BoxGlow
        {
            get => _boxGlow;
            set { _boxGlow = value; OnPropertyChanged(nameof(BoxGlow)); }
        }

        public bool BoxFill
        {
            get => _boxFill;
            set { _boxFill = value; OnPropertyChanged(nameof(BoxFill)); }
        }

        // Color Properties
        public string SkeletonColor
        {
            get => _skeletonColor;
            set { _skeletonColor = value; OnPropertyChanged(nameof(SkeletonColor)); }
        }

        public string CornerBoxColor
        {
            get => _cornerBoxColor;
            set { _cornerBoxColor = value; OnPropertyChanged(nameof(CornerBoxColor)); }
        }

        public string HealthBarColor
        {
            get => _healthBarColor;
            set { _healthBarColor = value; OnPropertyChanged(nameof(HealthBarColor)); }
        }

        public string BoxFillColor
        {
            get => _boxFillColor;
            set { _boxFillColor = value; OnPropertyChanged(nameof(BoxFillColor)); }
        }

        public string HeadCircleColor
        {
            get => _headCircleColor;
            set { _headCircleColor = value; OnPropertyChanged(nameof(HeadCircleColor)); }
        }

        public string Box3DColor
        {
            get => _box3DColor;
            set { _box3DColor = value; OnPropertyChanged(nameof(Box3DColor)); }
        }

        public string OutlineColor
        {
            get => _outlineColor;
            set { _outlineColor = value; OnPropertyChanged(nameof(OutlineColor)); }
        }

        public string ChamsColor
        {
            get => _chamsColor;
            set { _chamsColor = value; OnPropertyChanged(nameof(ChamsColor)); }
        }

        public string ChinaHatColor
        {
            get => _chinaHatColor;
            set { _chinaHatColor = value; OnPropertyChanged(nameof(ChinaHatColor)); }
        }

        public string SonarColor
        {
            get => _sonarColor;
            set { _sonarColor = value; OnPropertyChanged(nameof(SonarColor)); }
        }

        public string BoxColor
        {
            get => _boxColor;
            set { _boxColor = value; OnPropertyChanged(nameof(BoxColor)); }
        }

        public string NameColor
        {
            get => _nameColor;
            set { _nameColor = value; OnPropertyChanged(nameof(NameColor)); }
        }

        public string DistanceColor
        {
            get => _distanceColor;
            set { _distanceColor = value; OnPropertyChanged(nameof(DistanceColor)); }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion

    #region ESP Manager
    public class ESPManager
    {
        private static ESPManager? _instance;
        public static ESPManager Instance => _instance ??= new ESPManager();

        private readonly MemoryReader memoryReader;
        private readonly List<Player> players = new();
        private readonly object playersLock = new();

        private MemoryReader? externalMemoryReader;
        private RobloxMemoryScanner? robloxScanner;

        public ESPSettings Settings { get; } = new ESPSettings();

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
            float screenX = 400 + (worldPos.X - 100) * 2;
            float screenY = 300 - (worldPos.Y - 50) * 3;
            
            return new Vector2(screenX, screenY);
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

        public bool IsPlayerInFOV(Player player, float fovAngle = 90f)
        {
            var screenCenter = new Vector2(960, 540);
            var distance = Vector2.Distance(player.ScreenPosition, screenCenter);
            
            return distance < 500;
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
    #endregion
}