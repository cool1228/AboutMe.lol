using System.Windows;

namespace SelectXYZ_Cheat
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Set up global exception handling
            DispatcherUnhandledException += (s, ex) =>
            {
                MessageBox.Show($"An error occurred: {ex.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ex.Handled = true;
            };
        }
    }
}