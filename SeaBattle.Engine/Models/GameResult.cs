namespace SeaBattle.Engine.Models
{
    using System;
    using System.Collections.Generic;

    public class GameResult
    {
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        public PlayerDto Winner { get; set; }
        
        public ParticipantModel Participant1 { get; set; }
        
        public ParticipantModel Participant2 { get; set; }
        
        public Field.Field Participant1StartField { get; set; }
        
        public Field.Field Participant2StartField { get; set; }
        
        public IEnumerable<ExtendedTurnResult> TurnsHistory { get; set; }
    }
}