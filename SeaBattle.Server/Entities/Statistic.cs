namespace SeaBattle.Server.Entities
{
    public class Statistic
    {
        public int Id { get; set; }
        
        public int ParticipantId { get; set; }
        
        public Participant Participant { get; set; }
        
        public int Rating { get; set; }
        
        public int GamesPlayed { get; set; }
        
        public int Wins { get; set; }
        
        public int Losses { get; set; }
    }
}