using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SelectXYZ_Cheat
{
    public class ColorPickerControl : Control
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(ColorPickerControl), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty DefaultColorProperty =
            DependencyProperty.Register("DefaultColor", typeof(string), typeof(ColorPickerControl), new PropertyMetadata("White", OnDefaultColorChanged));

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Brush), typeof(ColorPickerControl), new PropertyMetadata(Brushes.White));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public string DefaultColor
        {
            get { return (string)GetValue(DefaultColorProperty); }
            set { SetValue(DefaultColorProperty, value); }
        }

        public Brush SelectedColor
        {
            get { return (Brush)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        static ColorPickerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPickerControl), new FrameworkPropertyMetadata(typeof(ColorPickerControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_ColorDisplay") is FrameworkElement colorDisplay)
            {
                colorDisplay.MouseLeftButtonUp += ColorDisplay_MouseLeftButtonUp;
            }
        }

        private void ColorDisplay_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var colorDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select Color",
                Filter = "All Files (*.*)|*.*"
            };

            // Simple color picker - in a real app you'd use a proper color picker dialog
            var colors = new[]
            {
                Brushes.White, Brushes.Black, Brushes.Red, Brushes.Green, Brushes.Blue,
                Brushes.Yellow, Brushes.Cyan, Brushes.Magenta, Brushes.Orange, Brushes.Purple
            };

            var window = new ColorPickerWindow(colors);
            if (window.ShowDialog() == true)
            {
                SelectedColor = window.SelectedBrush;
            }
        }

        private static void OnDefaultColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPickerControl control)
            {
                control.SelectedColor = GetBrushFromColorName((string)e.NewValue);
            }
        }

        private static Brush GetBrushFromColorName(string colorName)
        {
            return colorName switch
            {
                "White" => Brushes.White,
                "Black" => Brushes.Black,
                "Red" => Brushes.Red,
                "Green" => Brushes.Green,
                "Blue" => Brushes.Blue,
                "Yellow" => Brushes.Yellow,
                "Cyan" => Brushes.Cyan,
                "Magenta" => Brushes.Magenta,
                "Orange" => Brushes.Orange,
                "Purple" => new SolidColorBrush(Color.FromRgb(110, 69, 226)),
                "#FF8080FF" => new SolidColorBrush(Color.FromRgb(128, 128, 255)),
                "#FF00FFFF" => Brushes.Cyan,
                _ => Brushes.White
            };
        }
    }

    public partial class ColorPickerWindow : Window
    {
        public Brush SelectedBrush { get; private set; } = Brushes.White;

        public ColorPickerWindow(Brush[] colors)
        {
            InitializeComponent(colors);
        }

        private void InitializeComponent(Brush[] colors)
        {
            Title = "Select Color";
            Width = 300;
            Height = 200;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Background = new SolidColorBrush(Color.FromRgb(18, 18, 18));

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var colorsPanel = new WrapPanel { Margin = new Thickness(10) };
            
            foreach (var color in colors)
            {
                var button = new Button
                {
                    Width = 40,
                    Height = 40,
                    Margin = new Thickness(5),
                    Background = color,
                    BorderThickness = new Thickness(2),
                    BorderBrush = Brushes.Gray
                };

                button.Click += (s, e) =>
                {
                    SelectedBrush = color;
                    DialogResult = true;
                    Close();
                };

                colorsPanel.Children.Add(button);
            }

            var scrollViewer = new ScrollViewer { Content = colorsPanel };
            Grid.SetRow(scrollViewer, 0);
            grid.Children.Add(scrollViewer);

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 80,
                Height = 30,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            cancelButton.Click += (s, e) =>
            {
                DialogResult = false;
                Close();
            };

            Grid.SetRow(cancelButton, 1);
            grid.Children.Add(cancelButton);

            Content = grid;
        }
    }
}