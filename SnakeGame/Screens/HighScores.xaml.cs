using SnakeGame.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace SnakeGame.Screens
{
    public class ComboBoxItem<T>
    {
        public string DisplayName { get; set; } = "";
        public T? Value { get; set; }

        public override string ToString() => DisplayName;
    }

    /// <summary>
    /// Interaction logic for HighScores.xaml
    /// </summary>
    public partial class HighScores : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetProperty<T>(ref T property, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(property, value)) return;
            property = value;
            OnPropertyChanged(propertyName);
            return;
        }

        private readonly MainWindow _mainWindow;

        public List<ComboBoxItem<GameType?>> GameTypeChoices { get; } = new[] { new ComboBoxItem<GameType?> { DisplayName = "Any", Value = null } }
                .Concat(Enum.GetValues(typeof(GameType)).Cast<GameType>().Select(
                    g => new ComboBoxItem<GameType?> { DisplayName = g.ToString(), Value = (GameType?)g }
                    ))
                .ToList();

        private ComboBoxItem<GameType?> _gameType;
        public ComboBoxItem<GameType?> GameType
        {
            get => _gameType;
            set
            {
                SetProperty(ref _gameType, value);
                FilterScores();
            }
        }

        public List<ComboBoxItem<MapSize?>> MapSizeChoices { get; } = new[] { new ComboBoxItem<MapSize?> { DisplayName = "Any", Value = null } }
                .Concat(Enum.GetValues(typeof(MapSize)).Cast<MapSize>().Select(
                    g => new ComboBoxItem<MapSize?> { DisplayName = g.ToString(), Value = (MapSize?)g }
                    ))
                .ToList();
        private ComboBoxItem<MapSize?> _mapSize;
        public ComboBoxItem<MapSize?> MapSize
        {
            get => _mapSize;
            set
            {
                SetProperty(ref _mapSize, value);
                FilterScores();
            }
        }

        public List<ComboBoxItem<MapType?>> MapTypeChoices { get; } = new[] { new ComboBoxItem<MapType?> { DisplayName = "Any", Value = null } }
                .Concat(Enum.GetValues(typeof(MapType)).Cast<MapType>().Select(
                    g => new ComboBoxItem<MapType?> { DisplayName = g.ToString(), Value = (MapType?)g }
                    ))
                .ToList();
        private ComboBoxItem<MapType?> _mapType;
        public ComboBoxItem<MapType?> MapType
        {
            get => _mapType;
            set
            {
                SetProperty(ref _mapType, value);
                FilterScores();
            }
        }

        private List<HighScore>? _highScores;
        public List<HighScore> Scores
        {
            get => _highScores ?? [];
            set
            {
                SetProperty(ref _highScores, value);
            }
        }

        public HighScores(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            DataContext = this;

            _gameType = GameTypeChoices.First();
            _mapSize = MapSizeChoices.First();
            _mapType = MapTypeChoices.First();

            GameTypeComboBox.SelectedIndex = 0;
            MapSizeComboBox.SelectedIndex = 0;
            MapTypeComboBox.SelectedIndex = 0;

            FilterScores();
        }

        private bool ScoreCondition(HighScore score)
        {
            return (GameType.Value == null || score.GameType == GameType.Value) &&
                   (MapSize.Value == null || score.MapSize == MapSize.Value) &&
                   (MapType.Value == null || score.MapType == MapType.Value);
        }

        private void FilterScores()
        {
            Task.Run(() =>
            {
                using var context = new GameContext();
                Scores = [.. context.HighScores
                    .Where(ScoreCondition)
                    .OrderByDescending(h => h.Score)];
            });
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.ShowMainMenu();
        }
    }
}
