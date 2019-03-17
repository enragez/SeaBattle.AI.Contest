namespace SeaBattle.Engine.Models
{
    internal class Turn
    {
        internal Coordinate Coordinate { get; }

        internal Turn(Coordinate coordinate)
        {
            Coordinate = coordinate;
        }

        internal bool IsValid()
        {
            return Coordinate.IsValid();
        }
    }
}