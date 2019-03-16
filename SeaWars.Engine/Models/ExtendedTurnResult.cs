namespace SeaWars.Engine.Models
{
    using System.Collections.Generic;

    public class ExtendedTurnResult
    {
        public int PlayerId { get; set; }
        
        internal int ParticipantId { get; set; }
        
        internal int Id { get; set; }
        
        internal Dictionary<Coordinate, CellState> ChangedCells { get; } = new Dictionary<Coordinate, CellState>();
    }
}