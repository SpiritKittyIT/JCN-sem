using SnakeGame.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace SnakeGame.Screens
{
    /// <summary>
    /// Interaction logic for NewScoreForm.xaml
    /// </summary>
    public partial class NewScoreForm : UserControl, INotifyPropertyChanged
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

        private List<HighScore>? _highScores;
        public List<HighScore> HighScores
        {
            get => _highScores ?? [];
            set
            {
                SetProperty(ref _highScores, value);
            }
        }

        public NewScoreForm(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            DataContext = this;

            var gameViewModel = _mainWindow.GameViewModel;
            Task.Run(() =>
            {
                using var context = new GameContext();
                HighScores = [.. context.HighScores
                        .Where(h => h.GameType == gameViewModel.GameType && h.MapType == gameViewModel.MapType && h.MapSize == gameViewModel.MapSize)
                        .OrderByDescending(h => h.Score)
                        .Take(10)];

                if (HighScores.FirstOrDefault()?.Score < gameViewModel.Score)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ScoreTitle.Text = "New High Score!";
                    });
                }
            });

            var size = _mainWindow.GameViewModel.CurrentMap?.GetLength(0) ?? 0;

            ScoreTextBlock.Text = gameViewModel.Score.ToString();
            GameTypeTextBlock.Text = _mainWindow.GameViewModel.GameType.ToString();
            MapSizeTextBlock.Text = $"{size}x{size}";
            MapTypeTextBlock.Text = _mainWindow.GameViewModel.MapType.ToString();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Please enter a name.");
                return;
            }

            var gameViewModel = _mainWindow.GameViewModel;

            var highScore = new HighScore
            {
                PlayerName = NameTextBox.Text,
                Score = gameViewModel.Score,
                GameType = gameViewModel.GameType,
                MapType = gameViewModel.MapType,
                MapSize = gameViewModel.MapSize
            };

            Task.Run(() =>
            {
                using var context = new GameContext();
                context.HighScores.Add(highScore);
                context.SaveChanges();
            });

            _mainWindow.GameViewModel.Reset();
            _mainWindow.ShowMainMenu();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.GameViewModel.Reset();
            _mainWindow.ShowMainMenu();
        }
    }
}
