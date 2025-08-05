using System;
using System.Numerics;

namespace SelectXYZ_Cheat
{
    public class Player
    {
        public string Name { get; set; } = "Player";
        public bool IsLocalPlayer { get; set; } = false;
        public bool IsAlive { get; set; } = true;
        public float Health { get; set; } = 100f;
        public float MaxHealth { get; set; } = 100f;
        public float Distance { get; set; } = 0f;
        public Vector2 HeadScreenPosition { get; set; } = Vector2.Zero;
        public BoundingBox2D BoundingBox { get; set; } = new BoundingBox2D();
        
        public float HealthPercentage => Health / MaxHealth;
    }

    public class BoundingBox2D
    {
        public float Left { get; set; } = 0f;
        public float Top { get; set; } = 0f;
        public float Right { get; set; } = 100f;
        public float Bottom { get; set; } = 100f;
        public float Width => Right - Left;
        public float Height => Bottom - Top;
        public Vector2 Center => new Vector2(Left + Width / 2, Top + Height / 2);
    }
}