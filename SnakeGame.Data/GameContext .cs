using Microsoft.EntityFrameworkCore;

namespace SnakeGame.Data
{
    public class GameContext : DbContext
    {
        public DbSet<HighScore> HighScores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=highscores.db");
    }
}
