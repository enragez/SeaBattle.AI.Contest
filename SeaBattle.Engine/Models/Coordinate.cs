namespace SeaBattle.Engine.Models
{
    public struct Coordinate
    {
        public int Row { get; }
        
        public int Column { get; }

        public Coordinate(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public override string ToString()
        {
            return $"Row: {Row}, Column: {Column}";
        }

        internal bool IsValid()
        {
            return Row >= 0 && Row <= 9 &&
                   Column >= 0 && Column <= 9;
        }
    }
}