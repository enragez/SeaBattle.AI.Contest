namespace SeaWarsEngine.Models
{
    public class Battleship : Ship
    {
        protected override int Length => 4;
        
        public Battleship(Coordinate startLocation, Coordinate endLocation)
        {
            StartLocation = startLocation;
            EndLocation = endLocation;
        }
    }
}