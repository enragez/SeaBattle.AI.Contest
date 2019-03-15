namespace SeaWarsEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;

    public class Engine
    {
        private Player _player1;

        private Player _player2;

        private readonly Dictionary<int, string> _dllPaths = new Dictionary<int, string>();

        private readonly Dictionary<int, Fields> _fields = new Dictionary<int, Fields>();

        private Player _currentTurnPlayer;

        private Player _enemyPlayer;

        private Field _startPlayer1Field;

        private Field _startPlayer2Field;

        public Engine(string strategy1DllPath, string strategy2DllPath)
        {
            _dllPaths.Add(1, strategy1DllPath);
            _dllPaths.Add(2, strategy2DllPath);

            _fields.Add(1, new Fields());
            _fields.Add(2, new Fields());
        }

        public GameResult StartGame()
        {
            _player1 = CreatePlayer(1);
            _player2 = CreatePlayer(2);
            
            _player1.GamePhaseChanged(GamePhase.Planning);
            _player2.GamePhaseChanged(GamePhase.Planning);
            
            _player1.Strategy.PrepareField();
            _player2.Strategy.PrepareField();

            if (!FieldValidator.IsFieldValid(_player1.Field))
            {
                // 1 proebal
            }

            if (!FieldValidator.IsFieldValid(_player2.Field))
            {
                // 2 proebal
            }

            _startPlayer1Field = new Field(_player1.Field.ShipsPlaced);
            _startPlayer2Field = new Field(_player2.Field.ShipsPlaced);

            _player1.GamePhaseChanged(GamePhase.Battle);
            _player2.GamePhaseChanged(GamePhase.Battle);
            
            var coinWinnerId = CoinFlip();

            _currentTurnPlayer = GetPlayer(coinWinnerId);
            _enemyPlayer = GetPlayer(SwapId(coinWinnerId));

            var currentStrategy = _currentTurnPlayer.Strategy;

            var turnCoordinates = currentStrategy.DoTurn(new TurnResult(new Coordinate(-1, -1), 
                                                                        _currentTurnPlayer.Id));

            var turn = new Turn(turnCoordinates);

            while (true)
            {
                var turnResult = new TurnResult(turnCoordinates, _currentTurnPlayer.Id);
                
                if (!turn.IsValid())
                {
                    var currentTurnPlayerId = _currentTurnPlayer.Id;

                    turnResult.NewCellState = CellState.Empty;
                    _currentTurnPlayer.TurnsHistory.Enqueue(turnResult);

                    _currentTurnPlayer = GetPlayer(SwapId(currentTurnPlayerId));
                    _enemyPlayer = GetPlayer(currentTurnPlayerId);

                    turnCoordinates = _currentTurnPlayer.Strategy.DoTurn(turnResult);
                    continue;
                }

                if (TurnHitsEnemy(_enemyPlayer, turn))
                {
                    PerformHit(_currentTurnPlayer, _enemyPlayer, turn);

                    if (TurnKillsEnemyShip(_enemyPlayer, turn))
                    {
                        PerformKill(_currentTurnPlayer, _enemyPlayer, turn);
                        turnResult.NewCellState = CellState.Kill;
                        _currentTurnPlayer.TurnsHistory.Enqueue(turnResult);

                        if (IsPlayerLost(_enemyPlayer.Id))
                        {
                            return new GameResult
                                   {
                                       WinnerId = _currentTurnPlayer.Id,
                                       Player1StartField = _startPlayer1Field,
                                       Player2StartField = _startPlayer2Field,
                                       Player1TurnsHistory = _player1.TurnsHistory,
                                       Player2TurnsHistory = _player2.TurnsHistory
                                   };
                       }
                    }
                    else
                    {
                        turnResult.NewCellState = CellState.Hit;
                        _currentTurnPlayer.TurnsHistory.Enqueue(turnResult);
                    }
                }
                else
                {
                    PerformMiss(_currentTurnPlayer, _enemyPlayer, turn);
                    turnResult.NewCellState = CellState.Miss;
                    _currentTurnPlayer.TurnsHistory.Enqueue(turnResult);

                    var currentTurnPlayerId = _currentTurnPlayer.Id;

                    _currentTurnPlayer = GetPlayer(SwapId(currentTurnPlayerId));
                    _enemyPlayer = GetPlayer(currentTurnPlayerId);
                }

                turnCoordinates = _currentTurnPlayer.Strategy.DoTurn(turnResult);
                turn = new Turn(turnCoordinates);
            }
        }

        private Player GetPlayer(int playerId)
        {
            return playerId == 1 ? _player1 : _player2;
        }

        private bool TurnHitsEnemy(Player enemy, Turn turn)
        {
            return enemy.Field.Cells[turn.Coordinate.Row, turn.Coordinate.Column].State == CellState.Unit;
        }

        private void PerformHit(Player player, Player enemy, Turn turn)
        {
            enemy.Field.Cells[turn.Coordinate.Row, turn.Coordinate.Column].UpdateState(CellState.Hit);
            player.EnemyField.Cells[turn.Coordinate.Row, turn.Coordinate.Column].UpdateState(CellState.Hit);
        }

        private bool TurnKillsEnemyShip(Player enemy, Turn turn)
        {
            foreach (var ship in enemy.Field.ShipsPlaced)
            {
                Ship attackedShip = null;

                foreach (var shipCoordinate in ship.Coordinates)
                {
                    if (shipCoordinate.Row == turn.Coordinate.Row &&
                        shipCoordinate.Column == turn.Coordinate.Column)
                    {
                        attackedShip = ship;
                        break;
                    }
                }

                if (attackedShip != null)
                {
                    return attackedShip.Coordinates.All(coordinate =>
                            enemy.Field.Cells[coordinate.Row, coordinate.Column].State == CellState.Hit);
                }
            }

            return false;
        }

        private void PerformKill(Player player, Player enemy, Turn turn)
        {
            foreach (var ship in enemy.Field.ShipsPlaced)
            {
                Ship attackedShip = null;

                foreach (var shipCoordinate in ship.Coordinates)
                {
                    if (shipCoordinate.Row == turn.Coordinate.Row &&
                        shipCoordinate.Column == turn.Coordinate.Column)
                    {
                        attackedShip = ship;
                        break;
                    }
                }

                if (attackedShip != null)
                {
                    foreach (var attackedShipCoordinate in attackedShip.Coordinates)
                    {
                        enemy.Field.Cells[attackedShipCoordinate.Row, attackedShipCoordinate.Column].UpdateState(CellState.Kill);
                        player.EnemyField.Cells[attackedShipCoordinate.Row, attackedShipCoordinate.Column].UpdateState(CellState.Kill);
                    }
                }
            }
        }

        private void PerformMiss(Player player, Player enemy, Turn turn)
        {
            enemy.Field.Cells[turn.Coordinate.Row, turn.Coordinate.Column].UpdateState(CellState.Miss);
            player.EnemyField.Cells[turn.Coordinate.Row, turn.Coordinate.Column].UpdateState(CellState.Miss);
        }

        private bool IsPlayerLost(int playerId)
        {
            return GetPlayer(playerId).Field.GetCellsCountWithState(CellState.Kill) == 20;
        }
        
        private int CoinFlip()
        {
            return new Random().Next(1, 2);
        }

        private int SwapId(int currentId)
        {
            return currentId == 1
                       ? 2
                       : 1;
        }

        private Player CreatePlayer(int playerId)
        {
            var dllPath = _dllPaths[playerId];

            var playerField = _fields[playerId].Field;

            var enemyField = _fields[SwapId(playerId)].FieldForEnemy;

            var strategyWrapper = CreateStrategyWrapper(dllPath);
            strategyWrapper.Setup(playerField, enemyField, playerId);

            return new Player
                   {
                       Id = playerId,
                       Field = playerField,
                       EnemyField = enemyField,
                       Strategy = strategyWrapper,
                       TurnsHistory = new Queue<TurnResult>()
                   };
        }

        private StrategyWrapper CreateStrategyWrapper(string dllPath)
        {
            return new StrategyWrapper(dllPath);
        }
    }
}