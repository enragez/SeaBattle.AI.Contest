namespace SeaBattle.Server.Models
{
    public class PlayerStats
    {
        public int Position { get; set; }
        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public double Rating { get; set; }
        
        public int GamesCount { get; set; }
        
        public int WinsCount { get; set; }
        
        public int LossesCount { get; set; }
    }
}