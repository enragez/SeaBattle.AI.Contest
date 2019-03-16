namespace SeaWars.Engine.Models
{
    using Exceptions;

    internal class Cell
    {
        public CellState State { get; private set; }

        internal Coordinate Coordinate { get; }
        
        internal Cell(Coordinate coordinate)
        {
            Coordinate = coordinate;
        }

        public void UpdateState(CellState newState)
        {
            if (newState == CellState.Unit)
            {
                if (State == CellState.Lock || State == CellState.Unit)
                {
                    throw new CheatDetectedException("Ты куда воюешь блядь?");
                }
            }

            if (newState == CellState.Miss || newState == CellState.Lock || newState == CellState.Unit || newState == CellState.Empty)
            {
                if (State == CellState.Hit || State == CellState.Kill)
                {
                    return;
                }
            }
            
            State = newState;
        }

        public override string ToString()
        {
            return $"Row: {Coordinate.Row}, Column: {Coordinate.Column}, State: {State}";
        }
    }
}