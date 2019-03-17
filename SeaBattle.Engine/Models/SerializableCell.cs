namespace SeaBattle.Engine.Models
{
    public class SerializableCell
    {
        public int Row { get; set; }
        
        public int Column { get; set; }
        
        public CellState State { get; set; }
    }
}