namespace SeaBattle.Engine.Models.Serializable
{
    using Enums;

    public class SerializableCell
    {
        public int Row { get; set; }
        
        public int Column { get; set; }
        
        public CellState State { get; set; }
    }
}