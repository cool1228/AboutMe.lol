using System;
using System.Collections.Generic;

namespace SelectXYZ_Cheat
{
    public class ESPManager
    {
        private static ESPManager? _instance;
        public static ESPManager Instance => _instance ??= new ESPManager();

        public ESPSettings Settings { get; set; } = new ESPSettings();
        private List<Player> _players = new List<Player>();

        private ESPManager() { }

        public List<Player> GetPlayers()
        {
            // This would normally get players from the game
            // For now, return mock data
            if (_players.Count == 0)
            {
                _players.Add(new Player 
                { 
                    Name = "Player1", 
                    IsAlive = true, 
                    Health = 75f,
                    Distance = 15f,
                    BoundingBox = new BoundingBox2D { Left = 100, Top = 100, Right = 200, Bottom = 300 },
                    HeadScreenPosition = new System.Numerics.Vector2(150, 120)
                });
                _players.Add(new Player 
                { 
                    Name = "Player2", 
                    IsAlive = true, 
                    Health = 50f,
                    Distance = 25f,
                    BoundingBox = new BoundingBox2D { Left = 300, Top = 150, Right = 400, Bottom = 350 },
                    HeadScreenPosition = new System.Numerics.Vector2(350, 170)
                });
            }
            return _players;
        }
    }
}