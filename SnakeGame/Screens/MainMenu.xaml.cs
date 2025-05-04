using System.Windows;
using System.Windows.Controls;

namespace SnakeGame.Screens
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        private readonly MainWindow _mainWindow;

        public MainMenu(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.ShowNewGameForm();
        }

        private void HighScores_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.ShowHighScores();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Exit();
        }
    }
}
