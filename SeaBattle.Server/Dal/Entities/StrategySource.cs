namespace SeaBattle.Server.Dal.Entities
{
    using System;

    public class StrategySource
    {
        public int Id { get; set; }
        
        public int ParticipantId { get; set; }
        
        public Participant Participant { get; set; }
        
        public byte[] Sources { get; set; }
        
        public DateTime LoadDate { get; set; }
    }
}