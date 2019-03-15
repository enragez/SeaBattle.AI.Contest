namespace SeaWarsEngine.Models
{
    using System.Collections.Generic;

    public class GameResult
    {
        public int WinnerId { get; set; }
        
        public Field Player1StartField { get; set; }
        
        public Field Player2StartField { get; set; }
        
        public IEnumerable<TurnResult> Player1TurnsHistory { get; set; }
        
        public IEnumerable<TurnResult> Player2TurnsHistory { get; set; }
    }
}