namespace SeaWarsEngine.Models
{
    public class Cruiser : Ship
    {
        protected override int Length => 3;
        
        public Cruiser(Coordinate startLocation, Coordinate endLocation)
        {
            StartLocation = startLocation;
            EndLocation = endLocation;
        }
    }
}