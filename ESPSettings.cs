using System;

namespace SelectXYZ_Cheat
{
    public class ESPSettings
    {
        public ESPBoxType BoxType { get; set; } = ESPBoxType.Box2D;
        public string BoxColor { get; set; } = "Red";
        public string OutlineColor { get; set; } = "White";
        public bool BoxFill { get; set; } = false;
        public string BoxFillColor { get; set; } = "Red";
        public string CornerBoxColor { get; set; } = "Yellow";
        public string Box3DColor { get; set; } = "Blue";
        public bool SkeletonESP { get; set; } = false;
        public string SkeletonColor { get; set; } = "Green";
        public bool HealthBar { get; set; } = true;
        public string HealthBarColor { get; set; } = "Green";
        public bool HealthNumber { get; set; } = true;
        public bool PlayerName { get; set; } = true;
        public string NameColor { get; set; } = "White";
        public bool DistanceESP { get; set; } = true;
        public string DistanceColor { get; set; } = "Cyan";
        public bool HeadCircle { get; set; } = false;
        public string HeadCircleColor { get; set; } = "Red";
    }

    public enum ESPBoxType
    {
        None,
        Box2D,
        CornerBox,
        Box3D
    }
}