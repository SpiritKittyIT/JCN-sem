using SnakeGame.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : UserControl
    {
        private MainWindow _mainWindow;
        private UIElement[,]? _tileElements;
        private SnakeGameEngine? _engine;
        private int _countdownSeconds = 3;
        private Timer? _countdownTimer;
        private bool _firstCountdown = true;

        public Game(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            DataContext = _mainWindow.GameViewModel;

            _engine = new SnakeGameEngine(_mainWindow.GameViewModel, UpdateTile, OnGameOver);

            Loaded += (s, e) =>
            {
                Focusable = true;
                Keyboard.Focus(this);
            };

            var size = _mainWindow.GameViewModel.CurrentMap?.GetLength(0) ?? 0;

            GameTypeTextBlock.Text = _mainWindow.GameViewModel.GameType.ToString();
            MapSizeTextBlock.Text = $"{size}x{size}";
            MapTypeTextBlock.Text = _mainWindow.GameViewModel.MapType.ToString();

            InitializeMapGrid();
            ShowCountDown();
        }

        private void CountdownTick(object? state)
        {
            _countdownSeconds--;
            if (_countdownSeconds <= 0)
            {
                _countdownTimer?.Dispose();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OverlayGrid.Visibility = Visibility.Hidden;
                });
                if (_firstCountdown)
                {
                    _engine?.StartGame();
                    _firstCountdown = false;
                }
                else
                {
                    _engine?.TogglePause();
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OverlayText.Text = _countdownSeconds.ToString();
                });
            }
        }

        private void ShowCountDown()
        {
            _countdownSeconds = 3;
            Application.Current.Dispatcher.Invoke(() =>
            {
                OverlayText.Text = _countdownSeconds.ToString();
                OverlayGrid.Visibility = Visibility.Visible;
            });
            _countdownTimer = new Timer(CountdownTick, null, 1000, 1000);
        }

        private Brush GetBrushForTile(TileType tile)
        {
            return tile switch
            {
                TileType.Empty => Brushes.White,
                TileType.Wall => Brushes.Black,
                TileType.SnakeHead => Brushes.DarkGreen,
                TileType.SnakeBody => Brushes.Green,
                TileType.Food => Brushes.Yellow,
                _ => Brushes.Red
            };
        }

        private void InitializeMapGrid()
        {
            var map = _mainWindow.GameViewModel.CurrentMap;
            if (map == null) return;

            int size = map.GetLength(0);

            MapGrid.Rows = size;
            MapGrid.Columns = size;
            MapGrid.Children.Clear();

            _tileElements = new UIElement[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    var rectangle = new Border
                    {
                        Background = GetBrushForTile(map[x, y]),
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(0.5)
                    };

                    _tileElements[x, y] = rectangle;
                    MapGrid.Children.Add(rectangle);
                }
            }
        }

        private void UpdateTile(int x, int y)
        {
            var map = _mainWindow.GameViewModel.CurrentMap;
            if (map == null || _tileElements == null) return;

            int size = map.GetLength(0);

            if (_tileElements[x, y] is Border border)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    border.Background = GetBrushForTile(map[x, y]);
                });
            }
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (_engine?.IsGameOver == true)
            {
                _mainWindow.ShowNewScoreForm();
                return;
            }

            switch (e.Key)
            {
                case Key.W:
                case Key.Up:
                    _engine?.ChangeDirection(Coordinate.Up);
                    break;
                case Key.S:
                case Key.Down:
                    _engine?.ChangeDirection(Coordinate.Down);
                    break;
                case Key.A:
                case Key.Left:
                    _engine?.ChangeDirection(Coordinate.Left);
                    break;
                case Key.D:
                case Key.Right:
                    _engine?.ChangeDirection(Coordinate.Right);
                    break;
                case Key.P:
                    if (_engine?.Paused == true)
                    {
                        ShowCountDown();
                    }
                    else
                    {
                        OverlayText.Text = "Paused";
                        OverlayGrid.Visibility = Visibility.Visible;
                        _engine?.TogglePause();
                    }
                    break;
            }

            e.Handled = true;
        }

        private void OnGameOver()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                OverlayText.Text = $"Game Over\nScore: {_mainWindow.GameViewModel.Score}";
                OverlayGrid.Visibility = Visibility.Visible;
                OverlayHint.Visibility = Visibility.Visible;
            });
        }
    }
}
