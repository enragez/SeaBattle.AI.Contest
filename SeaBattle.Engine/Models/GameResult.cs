namespace SeaBattle.Engine.Models
{
    using System;
    using System.Collections.Generic;

    public class GameResult
    {
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        public Participant Winner { get; set; }
        
        public Participant Participant1 { get; set; }
        
        public Participant Participant2 { get; set; }
        
        public Field Participant1StartField { get; set; }
        
        public Field Participant2StartField { get; set; }
        
        public IEnumerable<ExtendedTurnResult> TurnsHistory { get; set; }
    }
}