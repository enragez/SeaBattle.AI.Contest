namespace SeaWarsEngine.Models
{
    using System;

    [Serializable]
    public struct TurnResult
    {
        public Coordinate Coordinate { get; }
        
        public CellState NewCellState { get; internal set; }
        
        public int PlayerId { get; }
        
        internal TurnResult(Coordinate coordinate, int playerId)
        {
            Coordinate = coordinate;
            NewCellState = CellState.Empty;
            PlayerId = playerId;
        }

        public override string ToString()
        {
            return $"Player: {PlayerId}, Row: {Coordinate.Row}, Column: {Coordinate.Column}, Result: {NewCellState}";
        }
    }
}