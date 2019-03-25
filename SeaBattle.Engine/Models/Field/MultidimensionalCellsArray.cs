namespace SeaBattle.Engine.Models.Field
{
    using System;

    internal class CellsArray
    {
        private Cell[,] Cells { get; } = new Cell[10, 10];

        private bool _safeModeEnabled = true;

        public CellsArray()
        {
            for (var row = 0; row < 10; row++)
            {
                for (var column = 0; column < 10; column++)
                {
                    Cells[row, column] = new Cell(new Coordinate(row, column));
                }
            }
        }
        
        internal Cell this[int row, int column]
        {
            get
            {
                if (ValidateCoordinate(row) && ValidateCoordinate(column))
                {
                    return Cells[row, column];
                }
                
                if (_safeModeEnabled)
                {
                    throw new IndexOutOfRangeException();
                }
                
                return new Cell(new Coordinate(-1, -1));
            }
        }

        internal void EnterSafeMode()
        {
            _safeModeEnabled = true;
        }

        internal void ExitSafeMode()
        {
            _safeModeEnabled = false;
        }
        
        private bool ValidateCoordinate(int coordinate)
        {
            return coordinate >= 0 && coordinate <= 9;
        }
    }
}