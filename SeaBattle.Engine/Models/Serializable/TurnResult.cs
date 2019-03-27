namespace SeaBattle.Engine.Models.Serializable
{
    using System;
    using Enums;

    [Serializable]
    public struct TurnResult
    {
        public Coordinate Coordinate { get; }
        
        public CellState NewCellState { get; internal set; }
        
        public int IngameId { get; }
        
        internal int ParticipantId { get; }
        
        internal int Id { get; }
        
        internal TurnResult(Coordinate coordinate, int ingameId, int participantId, int id)
        {
            Coordinate = coordinate;
            NewCellState = CellState.Empty;
            IngameId = ingameId;
            ParticipantId = participantId;
            Id = id;
        }

        public override string ToString()
        {
            return $"Player: {IngameId}, Row: {Coordinate.Row}, Column: {Coordinate.Column}, Result: {NewCellState}";
        }
    }
}