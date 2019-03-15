namespace SeaWarsEngine.Models
{
    public class Boat : Ship
    {
        protected override int Length => 1;
        
        public Boat(Coordinate location)
        {
            StartLocation = location;
            EndLocation = location;
        }
    }
}