namespace SeaWars.Engine.Models.Ships
{
    public class Destroyer : Ship
    {
        protected override int Length => 2;
        
        public Destroyer(Coordinate startLocation, Coordinate endLocation)
        {
            StartLocation = startLocation;
            EndLocation = endLocation;
        }
    }
}