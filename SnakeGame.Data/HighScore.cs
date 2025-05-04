namespace SnakeGame.Data
{
    public class HighScore
    {
        public int Id { get; set; }
        public required string PlayerName { get; set; }
        public int Score { get; set; }
        public GameType GameType { get; set; }
        public MapSize MapSize { get; set; }
        public MapType MapType { get; set; }
        public DateTime AchievedAt { get; set; } = DateTime.Now;
    }
}
