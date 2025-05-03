namespace SnakeGame.Data
{
    public class HighScore
    {
        public int Id { get; set; }
        public required string PlayerName { get; set; }
        public int Score { get; set; }
        public int GameType { get; set; }
        public int MapSize { get; set; }
        public int MapType { get; set; }
        public DateTime AchievedAt { get; set; } = DateTime.Now;
    }
}
