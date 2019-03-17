namespace SeaBattle.Practice.Strategies
{
    using Engine;
    using Engine.Models;
    using Engine.Models.Ships;

    public class Cycle: PlayerStrategy
    {
        private int _previousTurnRow;

        private int _previousTurnColumn;

        public override void PrepareField()
        {
            MyField.SetShip(new Battleship(new Coordinate(0, 6), new Coordinate(0, 9)));
            
            MyField.SetShip(new Cruiser(new Coordinate(0, 3), new Coordinate(2, 3)));
            MyField.SetShip(new Cruiser(new Coordinate(4, 1), new Coordinate(6, 1)));
            
            MyField.SetShip(new Destroyer(new Coordinate(0, 0), new Coordinate(0, 1)));
            MyField.SetShip(new Destroyer(new Coordinate(2, 9), new Coordinate(3, 9)));
            MyField.SetShip(new Destroyer(new Coordinate(8, 8), new Coordinate(9, 8)));
            
            MyField.SetShip(new Boat(new Coordinate(9, 0)));
            MyField.SetShip(new Boat(new Coordinate(8, 3)));
            MyField.SetShip(new Boat(new Coordinate(8, 5)));
            MyField.SetShip(new Boat(new Coordinate(5, 6)));
        }

        public override Coordinate DoTurn(TurnResult turnResult)
        {
            var turn = new Coordinate(_previousTurnRow, _previousTurnColumn);
            if (_previousTurnRow < 9)
            {
                _previousTurnRow++;
                return turn;
            }

            if (_previousTurnRow == 9)
            {
                _previousTurnColumn++;
                _previousTurnRow = 0;
            }
            
            return turn;
        }
    }
}