namespace SeaWars.Engine.Models
{
    using System;

    [Serializable]
    public class Row
    {
        public Column[] Cells { get; set; }
    }
}