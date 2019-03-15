namespace SeaWarsEngine.Models
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class Field
    {
        private const int IncValue = 1;
        
        internal CellsArray Cells { get; } = new CellsArray();

        internal readonly HashSet<Ship> ShipsPlaced = new HashSet<Ship>();

        private GamePhase _currentGamePhase;

        internal Field()
        {
        }
        
        internal Field(IEnumerable<Ship> ships)
        {
            _currentGamePhase = GamePhase.Planning;
            
            foreach (var ship in ships)
            {
                SetShip(ship);
            }
            
            _currentGamePhase = GamePhase.None;
        }
        
        internal void GamePhaseChanged(GamePhase gamePhase)
        {
            _currentGamePhase = gamePhase;
        }

        public void SetShip(Ship ship)
        {
            if (_currentGamePhase != GamePhase.Planning)
            {
                throw new CheatDetectedException("В очко себе затолкай свой корабль");
            }
            
            ship.Validate();

            CheckSameTypeShipsCount(ship);

            Cells.ExitSafeMode();
            
            if (ship.StartLocation.Row == ship.EndLocation.Row)
            {
                var row = ship.StartLocation.Row;
                
                for (var column = ship.StartLocation.Column; column <= ship.EndLocation.Column; column++)
                {
                    Cells[row - IncValue, column].UpdateState(CellState.Lock);
                    Cells[row, column].UpdateState(CellState.Unit);
                    Cells[row + IncValue, column].UpdateState(CellState.Lock);

                    var coordinate = new Coordinate(row, column);

                    if (!ship.Coordinates.Contains(coordinate))
                    {
                        ship.Coordinates.Add(coordinate);
                    }
                }
                
                Cells[row - IncValue, ship.StartLocation.Column - IncValue].UpdateState(CellState.Lock);
                Cells[row, ship.StartLocation.Column - IncValue].UpdateState(CellState.Lock);
                Cells[row + 1, ship.StartLocation.Column - IncValue].UpdateState(CellState.Lock);
                
                Cells[row - IncValue, ship.EndLocation.Column + IncValue].UpdateState(CellState.Lock);
                Cells[row, ship.EndLocation.Column + IncValue].UpdateState(CellState.Lock);
                Cells[row + 1, ship.EndLocation.Column + IncValue].UpdateState(CellState.Lock);
            }
            else if (ship.StartLocation.Column == ship.EndLocation.Column)
            {
                var column = ship.StartLocation.Column;
                
                for (var row = ship.StartLocation.Row; row <= ship.EndLocation.Row; row++)
                {
                    Cells[row, column - IncValue].UpdateState(CellState.Lock);
                    Cells[row, column].UpdateState(CellState.Unit);
                    Cells[row, column + IncValue].UpdateState(CellState.Lock);
                    
                    var coordinate = new Coordinate(row, column);

                    if (!ship.Coordinates.Contains(coordinate))
                    {
                        ship.Coordinates.Add(coordinate);
                    }
                }
                
                Cells[ship.StartLocation.Row - IncValue, column - IncValue].UpdateState(CellState.Lock);
                Cells[ship.StartLocation.Row - IncValue, column].UpdateState(CellState.Lock);
                Cells[ship.StartLocation.Row - IncValue, column + IncValue].UpdateState(CellState.Lock);
                
                Cells[ship.EndLocation.Row + IncValue, column - IncValue].UpdateState(CellState.Lock);
                Cells[ship.EndLocation.Row + IncValue, column].UpdateState(CellState.Lock);
                Cells[ship.EndLocation.Row + IncValue, column + IncValue].UpdateState(CellState.Lock);
            }

            ShipsPlaced.Add(ship);
            
            Cells.EnterSafeMode();
        }

        internal int GetCellsCountWithState(CellState state)
        {
            var count = 0;
            
            for (var row = 0; row < 10; row++)
            {
                for (var column = 0; column < 10; column++)
                {
                    if (Cells[row, column].State == state)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private void CheckSameTypeShipsCount(Ship ship)
        {
            switch (ship)
            {
                case Boat _:
                    if (ShipsPlaced.Count(s => s is Boat) == 4)
                    {
                        throw new CheatDetectedException("В очко себе затолкай такой корабль");
                    }
                    break;
                case Destroyer _:
                    if (ShipsPlaced.Count(s => s is Destroyer) == 3)
                    {
                        throw new CheatDetectedException("В очко себе затолкай такой корабль");
                    }
                    break;
                case Cruiser _:
                    if (ShipsPlaced.Count(s => s is Cruiser) == 2)
                    {
                        throw new CheatDetectedException("В очко себе затолкай такой корабль");
                    }
                    break;
                case Battleship _:
                    if (ShipsPlaced.Count(s => s is Battleship) == 1)
                    {
                        throw new CheatDetectedException("В очко себе затолкай такой корабль");
                    }
                    break;
                default:
                    throw new CheatDetectedException("В очко себе затолкай такой корабль");
            }
        }
        
        internal DataTable DataTable
        {
            get
            {
                var dataTable = new DataTable("Поле");
                dataTable.Columns.Add(" ");
                foreach (var column in ColumnNamesHelper.ColumnNames)
                {
                    dataTable.Columns.Add(column);
                }

                for (var row = 0; row < 10; row++)
                {
                    var dataRow = new object[11];

                    dataRow[0] = row + 1;
                    
                    for (var column = 1; column < 11; column++)
                    {
                        switch (Cells[row, column - 1].State)
                        {
                            case CellState.Hit:
                                dataRow[column] = "*";
                                break;
                            case CellState.Kill:
                                dataRow[column] = "x";
                                break;
                            case CellState.Unit:
                                dataRow[column] = "u";
                                break;
                            default:
                                dataRow[column] = string.Empty;
                                break;
                        }
                    }
                    
                    dataTable.Rows.Add(dataRow);
                }
                
                return dataTable;
            }
        }
    }
}