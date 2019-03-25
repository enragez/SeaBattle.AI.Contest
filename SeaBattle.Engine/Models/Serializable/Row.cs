namespace SeaBattle.Engine.Models.Serializable
{
    using System;

    [Serializable]
    public class Row
    {
        public Column[] Cells { get; set; }
    }
}