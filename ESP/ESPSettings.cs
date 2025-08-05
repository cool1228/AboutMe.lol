using System.ComponentModel;

namespace SelectXYZ_Cheat.ESP
{
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
        private string _outlineColor = "#FF8080FF";
        private string _chamsColor = "#FF00FFFF";
        private string _chinaHatColor = "Red";
        private string _sonarColor = "#FF00FFFF";
        private string _boxColor = "White";

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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}