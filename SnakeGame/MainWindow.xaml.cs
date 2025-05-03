using SnakeGame.Logic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeGame;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public GameViewModel GameViewModel = new GameViewModel();

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

    public void Exit()
    {
        Application.Current.Shutdown();
    }
}