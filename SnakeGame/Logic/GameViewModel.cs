using SnakeGame.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SnakeGame.Logic
{
    public class GameViewModel : INotifyPropertyChanged
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

        public GameViewModel()
        {
            Reset();
        }

        private Visibility _showLoadCustomMapButton;
        public Visibility ShowLoadCustomMapButton
        {
            get => _showLoadCustomMapButton;
            set
            {
                _showLoadCustomMapButton = value;
                OnPropertyChanged(nameof(ShowLoadCustomMapButton));
            }
        }

        private GameType _gameType;
        public GameType GameType
        {
            get => _gameType;
            set
            {
                SetProperty(ref _gameType, value);
            }
        }

        private MapSize _mapSize;
        public MapSize MapSize
        {
            get => _mapSize;
            set
            {
                SetProperty(ref _mapSize, value);
                MapChanged();
            }
        }

        private MapType _mapType;
        public MapType MapType
        {
            get => _mapType;
            set
            {
                SetProperty(ref _mapType, value);
                if (value == MapType.Custom)
                    SetProperty(ref _showLoadCustomMapButton, Visibility.Visible, nameof(ShowLoadCustomMapButton));
                MapChanged();
            }
        }

        private TileType[,]? _currentMap;
        public TileType[,]? CurrentMap
        {
            get => _currentMap;
            set => SetProperty(ref _currentMap, value);
        }

        private int _score = 0;
        public int Score
        {
            get => _score;
            set => SetProperty(ref _score, value);
        }

        public void MapChanged()
        {
            if (MapType == MapType.Custom) return;

            int size = MapSize switch
            {
                MapSize.Small => 10,
                MapSize.Medium => 15,
                MapSize.Large => 20,
                _ => 15
            };

            var newMap = new TileType[size, size];

            if (MapType == MapType.Random)
            {
                Random random = new Random();
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        var tile = random.Next(100) < 15 ? TileType.Wall : TileType.Empty;
                        newMap[i, j] = tile;
                    }
                }
            }

            CurrentMap = newMap;
        }

        public void Reset()
        {
            ShowLoadCustomMapButton = Visibility.Hidden;
            GameType = GameType.Classic;
            MapSize = MapSize.Medium;
            MapType = MapType.Classic;
            Score = 0;
            MapChanged();
        }
    }
}
