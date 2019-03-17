namespace SeaBattle.Engine.Models
{
    using System;

    [Serializable]
    public class Column
    {
        public string Name { get; set; }
        
        public int Number { get; set; }
        
        public CellState State { get; set; }
    }
}