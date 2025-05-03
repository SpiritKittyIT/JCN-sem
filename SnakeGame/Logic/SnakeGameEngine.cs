using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Logic
{
    class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Coordinate operator +(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.X + b.X, a.Y + b.Y);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Coordinate other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public static readonly Coordinate Up = new(0, -1);
        public static readonly Coordinate Down = new(0, 1);
        public static readonly Coordinate Left = new(-1, 0);
        public static readonly Coordinate Right = new(1, 0);
    }

    class SnakeGameEngine
    {
        private Timer? _gameTimer;
        private GameViewModel _gameViewModel;
        private List<Coordinate> _snake;
        private Coordinate Direction = Coordinate.Up;
        private int _interval = 350;
        private Action<int, int> _UpdateTile;
        private Action _OnGameOver;

        public bool IsGameOver = false;
        public bool Paused = false;

        private bool isOutOfBounds(Coordinate coord, int size)
        {
            return coord.X < 0 || coord.X >= size || coord.Y < 0 || coord.Y >= size;
        }

        public SnakeGameEngine(GameViewModel gameViewModel, Action<int, int> UpdateTile, Action OnGameOver)
        {
            _gameViewModel = gameViewModel;
            _UpdateTile = UpdateTile;
            _OnGameOver = OnGameOver;

            var map = _gameViewModel.CurrentMap;

            if (map == null)
                throw new InvalidOperationException("Map must be set before starting the game.");

            int size = map.GetLength(0);
            var center = new Coordinate(size / 2, size / 2);

            _snake = [center];

            var isSnakeValid = () => _snake.All(coord =>
            {
                bool result = map[coord.X, coord.Y].Equals(TileType.Empty);
                var coordUp = coord + Coordinate.Up;
                if (isOutOfBounds(coordUp, size))
                    return false;

                return result && map[coordUp.X, coordUp.Y].Equals(TileType.Empty);
            });

            while (!isSnakeValid())
            {
                for (int i = 0; i < _snake.Count; i++)
                {
                    var newCoord = _snake[i] + Coordinate.Right;
                    if (isOutOfBounds(newCoord, size))
                    {
                        newCoord.X = newCoord.X % size;
                        newCoord.Y = newCoord.Y % size;
                    }
                    _snake[i] = newCoord;
                }
            }

            _snake.ForEach(coord =>
            {
                map[coord.X, coord.Y] = TileType.SnakeHead;
            });
        }

        public void StartGame()
        {
            PlaceFood();
            _gameTimer = new Timer(OnGameTick, null, 0, _interval);
        }

        private void UpdateSpeed()
        {
            _interval = Math.Max(100, _interval - 3);
            _gameTimer?.Change(_interval, _interval);
        }

        private void SetTile(int x, int y, TileType tileType)
        {
            var map = _gameViewModel.CurrentMap;
            if (map == null)
                return;

            map[x, y] = tileType;
            _UpdateTile(x, y);
        }

        private void OnGameTick(object? state)
        {
            var map = _gameViewModel.CurrentMap;
            if (map == null)
                return;

            var newHead = _snake[0] + Direction;

            if (isOutOfBounds(newHead, map.GetLength(0)) || map[newHead.X, newHead.Y] == TileType.Wall || map[newHead.X, newHead.Y] == TileType.SnakeBody)
            {
                StopGame();
                return;
            }

            bool ateFood = map[newHead.X, newHead.Y] == TileType.Food;

            Coordinate? tail = null;
            if (!ateFood)
            {
                tail = _snake[^1];
                SetTile(tail.X, tail.Y, TileType.Empty);
                _snake.RemoveAt(_snake.Count - 1);
            }

            // Insert new head
            _snake.Insert(0, newHead);
            SetTile(newHead.X, newHead.Y, TileType.SnakeHead);

            // Update previous head to body
            if (_snake.Count > 1)
            {
                var oldHead = _snake[1];
                SetTile(oldHead.X, oldHead.Y, TileType.SnakeBody);
            }

            // Update score and speed
            if (ateFood)
            {
                _gameViewModel.Score += 20;
                PlaceFood();
                if (_gameViewModel.GameType == Data.GameType.Advanced)
                {
                    UpdateSpeed();
                }
            }
            else
            {
                _gameViewModel.Score++;
            }
        }

        private void PlaceFood()
        {
            var map = _gameViewModel.CurrentMap;
            if (map == null)
                return;

            int size = map.GetLength(0);
            Random random = new Random();

            var coord = new Coordinate(random.Next(size), random.Next(size));

            while (map[coord.X, coord.Y] != TileType.Empty)
            {
                coord += Coordinate.Right;
                if (isOutOfBounds(coord, size))
                {
                    coord.X = coord.X % size;
                    coord.Y = coord.Y % size;
                }
            }

            map[coord.X, coord.Y] = TileType.Food;
            _UpdateTile(coord.X, coord.Y);
        }

        public void ChangeDirection(Coordinate newDirection)
        {
            if (_snake.Count < 2)
            {
                Direction = newDirection;
                return;
            }

            var nextStep = _snake[0] + newDirection;
            if (_snake[1].Equals(nextStep))
            {
                return;
            }
            Direction = newDirection;
        }

        public void StopGame()
        {
            _gameTimer?.Dispose();
            IsGameOver = true;
            _OnGameOver?.Invoke();
        }

        public void TogglePause()
        {
            if (Paused)
            {
                _gameTimer?.Change(0, _interval);
                Paused = false;
            }
            else
            {
                _gameTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                Paused = true;
            }
        }
    }
}
