using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeGame.Screens
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        private MainWindow _mainWindow;

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
            _mainWindow.Exit();
        }
    }
}
