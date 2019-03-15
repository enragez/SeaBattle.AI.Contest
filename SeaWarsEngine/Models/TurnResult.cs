namespace SeaWarsEngine.Models
{
    using System;

    [Serializable]
    public struct TurnResult
    {
        public Coordinate Coordinate { get; }
        
        public CellState NewCellState { get; internal set; }
        
        public int PlayerId { get; }
        
        internal int ParticipantId { get; }
        
        internal int Id { get; }
        
        internal TurnResult(Coordinate coordinate, int playerId, int participantId, int id)
        {
            Coordinate = coordinate;
            NewCellState = CellState.Empty;
            PlayerId = playerId;
            ParticipantId = participantId;
            Id = id;
        }

        public override string ToString()
        {
            return $"Player: {PlayerId}, Row: {Coordinate.Row}, Column: {Coordinate.Column}, Result: {NewCellState}";
        }
    }
}