using System.Windows;

namespace SnakeGame;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        using var context = new Data.GameContext();
        context.Database.EnsureCreated();

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}
