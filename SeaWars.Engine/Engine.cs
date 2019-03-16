namespace SeaWars.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Models.Ships;

    public class Engine
    {
        private Player _player1;

        private Player _player2;

        private readonly Dictionary<int, Fields> _fields = new Dictionary<int, Fields>();

        private Player _currentTurnPlayer;

        private Player _enemyPlayer;

        private Field _startPlayer1Field;

        private Field _startPlayer2Field;
        
        private readonly Participant[] _participants = new Participant[2];
        
        private readonly Dictionary<int, int> _ids = new Dictionary<int, int>();
        
        private readonly Queue<TurnResult> _turnsHistory = new Queue<TurnResult>();

        private readonly DateTime _gameStartTime;

        private readonly int _gameId;
        
        public Engine(int gameId, Participant participant1, Participant participant2)
        {
            _gameId = gameId;
            
            _fields.Add(0, new Fields());
            _fields.Add(1, new Fields());

            _participants[0] = participant1;
            _ids.Add(participant1.Id, 0);
            
            _participants[1] = participant2;
            _ids.Add(participant2.Id, 1);

            _gameStartTime = DateTime.Now;
        }

        public GameResult StartGame()
        {
            _player1 = CreatePlayer(_participants[0]);
            _player2 = CreatePlayer(_participants[1]);
            
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

            var turnCounter = -1;
            
            var turnCoordinates = currentStrategy.DoTurn(new TurnResult(new Coordinate(-1, -1), 
                                                                        _currentTurnPlayer.Id,
                                                                        _participants[_currentTurnPlayer.Id].Id,
                                                                        turnCounter));
            turnCounter++;
            var turn = new Turn(turnCoordinates);

            while (true)
            {
                var turnResult = new TurnResult(turnCoordinates, _currentTurnPlayer.Id, _participants[_currentTurnPlayer.Id].Id, turnCounter);
                
                if (!turn.IsValid())
                {
                    var currentTurnPlayerId = _currentTurnPlayer.Id;

                    turnResult.NewCellState = CellState.Empty;
                    _currentTurnPlayer.TurnsHistory.Enqueue(turnResult);
                    _turnsHistory.Enqueue(turnResult);

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
                        _turnsHistory.Enqueue(turnResult);

                        if (IsPlayerLost(_enemyPlayer.Id))
                        {
                            return new GameResult
                                   {
                                       Id = _gameId,
                                       StartTime = _gameStartTime,
                                       EndTime = DateTime.Now,
                                       Participant1 = _participants[0],
                                       Participant2 = _participants[1],
                                       Winner = _participants[_currentTurnPlayer.Id],
                                       TurnsHistory = _turnsHistory,
                                       Participant1StartField = _startPlayer1Field,
                                       Participant2StartField = _startPlayer2Field
                                   };
                       }
                    }
                    else
                    {
                        turnResult.NewCellState = CellState.Hit;
                        _currentTurnPlayer.TurnsHistory.Enqueue(turnResult);
                        _turnsHistory.Enqueue(turnResult);
                    }
                }
                else
                {
                    PerformMiss(_currentTurnPlayer, _enemyPlayer, turn);
                    turnResult.NewCellState = CellState.Miss;
                    _currentTurnPlayer.TurnsHistory.Enqueue(turnResult);
                    _turnsHistory.Enqueue(turnResult);

                    var currentTurnPlayerId = _currentTurnPlayer.Id;

                    _currentTurnPlayer = GetPlayer(SwapId(currentTurnPlayerId));
                    _enemyPlayer = GetPlayer(currentTurnPlayerId);
                }

                turnCoordinates = _currentTurnPlayer.Strategy.DoTurn(turnResult);
                turnCounter++;
                turn = new Turn(turnCoordinates);
            }
        }

        private Player GetPlayer(int playerId)
        {
            return playerId == 0 ? _player1 : _player2;
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
            return new Random().Next(0, 1);
        }

        private int SwapId(int currentId)
        {
            return currentId == 0
                       ? 1
                       : 0;
        }

        private Player CreatePlayer(Participant participant)
        {
            var participantIndex = _ids[participant.Id];
            
            var dllPath = participant.StrategyAssemblyPath;
            var playerField = _fields[participantIndex].Field;

            var enemyField = _fields[SwapId(participantIndex)].FieldForEnemy;

            var strategyWrapper = !string.IsNullOrEmpty(dllPath)
                                      ? CreateStrategyWrapper(dllPath)
                                      : CreateStrategyWrapper(participant.Strategy);
            
            strategyWrapper.Setup(playerField, enemyField, participantIndex);

            return new Player
                   {
                       Id = participantIndex,
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

        private StrategyWrapper CreateStrategyWrapper(PlayerStrategy strategy)
        {
            return new StrategyWrapper(strategy);
        }
    }
}