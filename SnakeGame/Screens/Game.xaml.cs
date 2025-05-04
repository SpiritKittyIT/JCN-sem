using SnakeGame.Logic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SnakeGame.Screens
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : UserControl
    {
        private readonly MainWindow _mainWindow;
        private UIElement[,]? _tileElements;
        private readonly SnakeGameEngine? _engine;
        CancellationTokenSource _cts = new();
        private bool _isPaused = false;
        private event EventHandler<CoordinateEventArgs>? TileUpdateEvent;

        public Game(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            DataContext = _mainWindow.GameViewModel;

            TileUpdateEvent += TileUpdateEventHandler;

            _engine = new SnakeGameEngine(_mainWindow.GameViewModel, TileUpdateEvent, OnGameOver);

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

            _ = StartCountDown(_cts.Token);
        }

        private async Task StartCountDown(CancellationToken cancellationToken)
        {
            try
            {
                for (int i = 3; i > 0; --i)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        OverlayText.Text = i.ToString();
                        OverlayGrid.Visibility = Visibility.Visible;
                    });

                    await Task.Delay(1000, cancellationToken);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    OverlayGrid.Visibility = Visibility.Hidden;
                    _engine?.SetPaused(false);
                });
            }
            catch (TaskCanceledException) { }
        }

        private static SolidColorBrush GetBrushForTile(TileType tile)
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

        private void TileUpdateEventHandler(object? sender, EventArgs e)
        {
            if (e is CoordinateEventArgs cea)
            {
                var coord = cea.Coordinate;
                var map = _mainWindow.GameViewModel.CurrentMap;
                if (map == null || _tileElements == null) return;

                int size = map.GetLength(0);

                if (_tileElements[coord.X, coord.Y] is Border border)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        border.Background = GetBrushForTile(map[coord.X, coord.Y]);
                    });
                }
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
                    if (_isPaused)
                    {
                        _isPaused = false;
                        _cts = new CancellationTokenSource();
                        _ = StartCountDown(_cts.Token);
                    }
                    else
                    {
                        OverlayText.Text = "Paused";
                        OverlayGrid.Visibility = Visibility.Visible;
                        _engine?.SetPaused(true);
                        _isPaused = true;
                        _cts.Cancel();
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
