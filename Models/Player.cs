using System.Numerics;

namespace SelectXYZ_Cheat.Models
{
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
}