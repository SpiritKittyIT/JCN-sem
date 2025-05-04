using SnakeGame.Logic;
using System.Windows;

namespace SnakeGame;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public GameViewModel GameViewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        ShowMainMenu();
    }

    public void ShowGame()
    {
        MainContent.Content = new Screens.Game(this);
    }

    public void ShowHighScores()
    {
        MainContent.Content = new Screens.HighScores(this);
    }

    public void ShowMainMenu()
    {
        MainContent.Content = new Screens.MainMenu(this);
    }

    public void ShowNewGameForm()
    {
        MainContent.Content = new Screens.NewGameForm(this);
    }

    public void ShowNewScoreForm()
    {
        MainContent.Content = new Screens.NewScoreForm(this);
    }

    public static void Exit()
    {
        Application.Current.Shutdown();
    }
}