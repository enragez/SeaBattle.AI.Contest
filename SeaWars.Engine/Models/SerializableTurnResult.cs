namespace SeaWarsEngine.Models
{
    using System;

    [Serializable]
    public class SerializableTurnResult
    {
        public SerializableTurnResult()
        {
            
        }

        public SerializableTurnResult(TurnResult turnResult)
        {
            Row = turnResult.Coordinate.Row;
            Column = turnResult.Coordinate.Column;
            NewCellState = turnResult.NewCellState;
            PlayerId = turnResult.ParticipantId;
            Id = turnResult.Id;
        }
        
        public int Row { get; set; }
        
        public int Column { get; set; }
        
        public CellState NewCellState { get; set; }
        
        public int PlayerId { get; set; }
        
        public int Id { get; set; }
    }
}