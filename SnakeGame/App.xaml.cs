using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
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

        var context = new SnakeGame.Data.GameContext();
        context.Database.Migrate();

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}

