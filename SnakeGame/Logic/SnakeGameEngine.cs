namespace SnakeGame.Logic
{
    class SnakeGameEngine
    {
        private readonly Timer? _gameTimer;
        private readonly GameViewModel _gameViewModel;
        private readonly List<Coordinate> _snake;
        private Coordinate Direction = Coordinate.Up;
        private int _interval = 350;
        private readonly Action _OnGameOver;
        private readonly EventHandler<CoordinateEventArgs>? _tileUpdateEvent;

        public bool IsGameOver = false;

        private static bool IsOutOfBounds(Coordinate coord, int size)
        {
            return coord.X < 0 || coord.X >= size || coord.Y < 0 || coord.Y >= size;
        }

        public SnakeGameEngine(GameViewModel gameViewModel, EventHandler<CoordinateEventArgs>? tileUpdateEvent, Action OnGameOver)
        {
            _gameViewModel = gameViewModel;
            _tileUpdateEvent = tileUpdateEvent;
            _OnGameOver = OnGameOver;

            var map = _gameViewModel.CurrentMap ?? throw new InvalidOperationException("Map must be set before starting the game.");
            int size = map.GetLength(0);
            var center = new Coordinate(size / 2, size / 2);

            _snake = [center];

            bool isSnakeValid() => _snake.All(coord =>
            {
                bool result = map[coord.X, coord.Y].Equals(TileType.Empty);
                var coordUp = coord + Coordinate.Up;
                if (IsOutOfBounds(coordUp, size))
                    return false;

                return result && map[coordUp.X, coordUp.Y].Equals(TileType.Empty);
            });

            while (!isSnakeValid())
            {
                for (int i = 0; i < _snake.Count; i++)
                {
                    var newCoord = _snake[i] + Coordinate.Right;
                    if (IsOutOfBounds(newCoord, size))
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


            _gameTimer = new Timer(OnGameTick, null, Timeout.Infinite, Timeout.Infinite);
            PlaceFood();
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
            _tileUpdateEvent?.Invoke(this, new CoordinateEventArgs(new Coordinate(x, y)));
        }

        private void OnGameTick(object? state)
        {
            var map = _gameViewModel.CurrentMap;
            if (map == null)
                return;

            var newHead = _snake[0] + Direction;

            if (IsOutOfBounds(newHead, map.GetLength(0)) || map[newHead.X, newHead.Y] == TileType.Wall || map[newHead.X, newHead.Y] == TileType.SnakeBody)
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
            Random random = new();

            var coord = new Coordinate(random.Next(size), random.Next(size));

            while (map[coord.X, coord.Y] != TileType.Empty)
            {
                coord += Coordinate.Right;
                if (IsOutOfBounds(coord, size))
                {
                    coord.X = coord.X % size;
                    coord.Y = coord.Y % size;
                }
            }

            map[coord.X, coord.Y] = TileType.Food;
            _tileUpdateEvent?.Invoke(this, new CoordinateEventArgs(coord));
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

        public void SetPaused(bool paused)
        {
            if (paused)
            {
                _gameTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            }
            else
            {
                _gameTimer?.Change(0, _interval);
            }
        }
    }
}
