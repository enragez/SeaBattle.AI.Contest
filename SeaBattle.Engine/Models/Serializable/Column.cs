namespace SeaBattle.Engine.Models.Serializable
{
    using System;
    using Enums;

    [Serializable]
    public class Column
    {
        public string Name { get; set; }
        
        public int Number { get; set; }
        
        public CellState State { get; set; }
    }
}