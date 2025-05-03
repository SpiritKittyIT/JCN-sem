using Microsoft.Win32;
using SnakeGame.Data;
using SnakeGame.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for NewGameForm.xaml
    /// </summary>
    public partial class NewGameForm : UserControl
    {
        private MainWindow _mainWindow;

        public NewGameForm(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            _mainWindow.GameViewModel.PropertyChanged += GameOptions_PropertyChanged;
            DataContext = _mainWindow.GameViewModel;

            GameTypeComboBox.ItemsSource = Enum.GetValues(typeof(GameType)).Cast<GameType>();
            GameTypeComboBox.SelectedIndex = 0;

            MapSizeComboBox.ItemsSource = Enum.GetValues(typeof(MapSize)).Cast<MapSize>();
            MapSizeComboBox.SelectedIndex = 1;

            MapTypeComboBox.ItemsSource = Enum.GetValues(typeof(MapType)).Cast<MapType>();
            MapTypeComboBox.SelectedIndex = 0;

            RenderMap();
        }

        private void LoadCustomMap_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Select Custom Map File"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                var lines = File.ReadAllLines(dialog.FileName);
                int expectedSize = _mainWindow.GameViewModel.MapSize switch
                {
                    MapSize.Small => 10,
                    MapSize.Medium => 15,
                    MapSize.Large => 20,
                    _ => 15
                };

                if (lines.Length != expectedSize)
                {
                    MessageBox.Show(
                        $"Invalid number of rows. Expected {expectedSize}, got {lines.Length}.",
                        "Invalid Map",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                var newMap = new TileType[expectedSize, expectedSize];

                for (int y = 0; y < expectedSize; y++)
                {
                    var parts = lines[y].Trim().Split(' ');
                    if (parts.Length != expectedSize)
                    {
                        MessageBox.Show(
                            $"Invalid number of columns on row {y + 1}. Expected {expectedSize}, got {parts.Length}.",
                            "Invalid Map",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );
                        return;
                    }

                    for (int x = 0; x < expectedSize; x++)
                    {
                        if (!int.TryParse(parts[x], out int tileVal) || (tileVal != 0 && tileVal != 1))
                        {
                            MessageBox.Show(
                                $"Invalid tile at row {y + 1}, column {x + 1}. Must be 0 (Empty) or 1 (Wall).",
                                "Invalid Map",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning
                            );
                            return;
                        }

                        newMap[x, y] = tileVal == 0 ? TileType.Empty : TileType.Wall;
                    }
                }

                _mainWindow.GameViewModel.CurrentMap = newMap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load map: " + ex.Message,
                    "Invalid Map",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }

        private void RenderMap()
        {
            var map = _mainWindow.GameViewModel.CurrentMap;
            if (map == null) return;

            int size = map.GetLength(0);

            MapPreviewGrid.Rows = size;
            MapPreviewGrid.Columns = size;
            MapPreviewGrid.Children.Clear();

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    var rectangle = new Border
                    {
                        Background = map[x, y] switch
                        {
                            TileType.Empty => Brushes.White,
                            TileType.Wall => Brushes.Black,
                            _ => Brushes.Red
                        },
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(0.5)
                    };

                    MapPreviewGrid.Children.Add(rectangle);
                }
            }
        }

        private void GameOptions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_mainWindow.GameViewModel.CurrentMap))
            {
                RenderMap();
            }
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.GameViewModel.PropertyChanged -= GameOptions_PropertyChanged;
            _mainWindow.ShowGame();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.ShowMainMenu();
        }
    }
}
