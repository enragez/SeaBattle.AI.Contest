namespace SeaBattle.Server.Models
{
    using System;
    using Engine.Models;

    public class Game
    {
        public int Id { get; set; }
        
        public int Turn { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        public Participant Participant1 { get; set; }
        
        public Participant Participant2 { get; set; }
        
        public SerializableTurnResult[] TurnsHistory { get; set; }
        
        public Row[] Participant1Field { get; set; }
        
        public Row[] Participant2Field { get; set; }
    }
}