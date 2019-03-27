namespace SeaBattle.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enums;
    using Exceptions;
    using Models;
    using Models.Field;
    using Models.Serializable;
    using Models.Ships;
    using Strategy;
    using Utils;

    public class Engine
    {
        private readonly Random _random = new Random();
        
        private Player _player1;

        private Player _player2;

        private readonly Dictionary<int, Fields> _fields = new Dictionary<int, Fields>();

        private Player _currentTurnPlayer;

        private Player _enemyPlayer;

        private Field _startPlayer1Field;

        private Field _startPlayer2Field;
        
        private readonly PlayerDto[] _playerDtos = new PlayerDto[2];
        
        private readonly Queue<ExtendedTurnResult> _turnsHistory = new Queue<ExtendedTurnResult>();

        private readonly DateTime _gameStartTime;
        
        public Engine(PlayerDto playerDto1, PlayerDto playerDto2)
        {
            _fields.Add(0, new Fields());
            _fields.Add(1, new Fields());

            _playerDtos[0] = playerDto1;
            
            _playerDtos[1] = playerDto2;

            _gameStartTime = DateTime.Now;
        }

        public GameResult StartGame()
        {
            try
            {
                _player1 = CreatePlayer(_playerDtos[0], 0);
            }
            catch (CheatDetectedException)
            {
                return GetGameResult(_playerDtos[1]);
            }
            
            try
            {
                _player2 = CreatePlayer(_playerDtos[1], 1);
            }
            catch (CheatDetectedException)
            {
                return GetGameResult(_playerDtos[0]);
            }
            
            _player1.GamePhaseChanged(GamePhase.Planning);
            _player2.GamePhaseChanged(GamePhase.Planning);

            try
            {
                _player1.Strategy.PrepareField();
            }
            catch (CheatDetectedException)
            {
                return GetGameResult(_playerDtos[1]);
            }
            
            try
            {
                _player2.Strategy.PrepareField();
            }
            catch (CheatDetectedException)
            {
                return GetGameResult(_playerDtos[0]);
            }
            
            if (!FieldValidator.IsFieldValid(_player1.Field))
            {
                return GetGameResult(_playerDtos[1]);
            }

            if (!FieldValidator.IsFieldValid(_player2.Field))
            {
                return GetGameResult(_playerDtos[0]);
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
                                                                        _currentTurnPlayer.IngameId,
                                                                        _playerDtos[_currentTurnPlayer.IngameId].Id,
                                                                        turnCounter));
            turnCounter++;
            var turn = new Turn(turnCoordinates);

            while (true)
            {
                var turnResult = new TurnResult(turnCoordinates, 
                                                _currentTurnPlayer.IngameId, 
                                                _playerDtos[_currentTurnPlayer.IngameId].Id, 
                                                turnCounter);
                
                if (!turn.IsValid())
                {
                    turnResult.NewCellState = CellState.Empty;

                    AddTurnToHistory(turnResult, turnCounter,
                                     extendedTurnRes =>
                                     {
                                         extendedTurnRes.ChangedCells.Add(turnResult.Coordinate, CellState.Empty);
                                     });
                    
                    if (IsBothPlayersReachedTurnsLimit())
                    {
                        return FinishGameByTurnsLimit();
                    }

                    PassTurnToAnotherPlayer();

                    turnCoordinates = _currentTurnPlayer.Strategy.DoTurn(turnResult);
                    continue;
                }

                if (TurnHitsEnemy(_enemyPlayer, turn))
                {
                    PerformHit(_currentTurnPlayer, _enemyPlayer, turn);

                    if (TurnKillsEnemyShip(_enemyPlayer, turn))
                    {
                        PerformKill(_currentTurnPlayer, _enemyPlayer, turn, out var killedShipCoordinates);
                        
                        turnResult.NewCellState = CellState.Kill;

                        AddTurnToHistory(turnResult, turnCounter,
                                         extendedTurnRes =>
                                         {
                                             foreach (var coordinate in killedShipCoordinates)
                                             {
                                                 extendedTurnRes.ChangedCells.Add(coordinate,
                                                                                  CellState.Kill);
                                             }
                                         });
                        
                        if (IsBothPlayersReachedTurnsLimit())
                        {
                            return FinishGameByTurnsLimit();
                        }

                        if (IsPlayerLost(_enemyPlayer.IngameId))
                        {
                            return GetGameResult(_playerDtos[_currentTurnPlayer.IngameId]);
                        }
                    }
                    else
                    {
                        turnResult.NewCellState = CellState.Hit;

                        AddTurnToHistory(turnResult, turnCounter,
                                         extendedTurnRes =>
                                         {
                                             extendedTurnRes.ChangedCells.Add(turnResult.Coordinate, CellState.Hit);
                                         });
                        
                        if (IsBothPlayersReachedTurnsLimit())
                        {
                            return FinishGameByTurnsLimit();
                        }
                    }
                }
                else
                {
                    PerformMiss(_currentTurnPlayer, _enemyPlayer, turn);
                    turnResult.NewCellState = CellState.Miss;

                    AddTurnToHistory(turnResult, turnCounter,
                                     extendedTurnRes =>
                                     {
                                         extendedTurnRes.ChangedCells.Add(turnResult.Coordinate, CellState.Miss);
                                     });
                    
                    if (IsBothPlayersReachedTurnsLimit())
                    {
                        return FinishGameByTurnsLimit();
                    }

                    PassTurnToAnotherPlayer();
                }

                turnCoordinates = _currentTurnPlayer.Strategy.DoTurn(turnResult);
                turnCounter++;
                turn = new Turn(turnCoordinates);
            }
        }

        private void AddTurnToHistory(TurnResult turnResult, int turnCounter, Action<ExtendedTurnResult> changedCellsFiller)
        {
            _currentTurnPlayer.TurnsHistory.Enqueue(turnResult);
                    
            var extendedTurnRes = new ExtendedTurnResult
                                  {
                                      Id = turnCounter,
                                      PlayerId = _currentTurnPlayer.IngameId,
                                      ParticipantId = _playerDtos[_currentTurnPlayer.IngameId].Id
                                  };

            changedCellsFiller(extendedTurnRes);
            _turnsHistory.Enqueue(extendedTurnRes);
        }

        private void PassTurnToAnotherPlayer()
        {
            var currentTurnPlayerId = _currentTurnPlayer.IngameId;

            _currentTurnPlayer = GetPlayer(SwapId(currentTurnPlayerId));
            _enemyPlayer = GetPlayer(currentTurnPlayerId);
        }
        
        private Player GetPlayer(int playerId)
        {
            return playerId == 0 ? _player1 : _player2;
        }

        private bool TurnHitsEnemy(Player enemy, Turn turn)
        {
            return enemy.Field[turn.Coordinate.Row, turn.Coordinate.Column].State == CellState.Unit ||
                   enemy.Field[turn.Coordinate.Row, turn.Coordinate.Column].State == CellState.Hit ||
                   enemy.Field[turn.Coordinate.Row, turn.Coordinate.Column].State == CellState.Kill;
        }

        private void PerformHit(Player player, Player enemy, Turn turn)
        {
            enemy.Field[turn.Coordinate.Row, turn.Coordinate.Column].UpdateState(CellState.Hit);
            player.EnemyField[turn.Coordinate.Row, turn.Coordinate.Column].UpdateState(CellState.Hit);
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
                            enemy.Field[coordinate.Row, coordinate.Column].State == CellState.Hit);
                }
            }

            return false;
        }

        private void PerformKill(Player player, Player enemy, Turn turn, out IEnumerable<Coordinate> killedShipCoordinates)
        {
            killedShipCoordinates = null;
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
                    killedShipCoordinates = attackedShip.Coordinates;
                    
                    foreach (var attackedShipCoordinate in attackedShip.Coordinates)
                    {
                        enemy.Field[attackedShipCoordinate.Row, attackedShipCoordinate.Column].UpdateState(CellState.Kill);
                        player.EnemyField[attackedShipCoordinate.Row, attackedShipCoordinate.Column].UpdateState(CellState.Kill);
                    }
                }
            }
        }

        private void PerformMiss(Player player, Player enemy, Turn turn)
        {
            enemy.Field[turn.Coordinate.Row, turn.Coordinate.Column].UpdateState(CellState.Miss);
            player.EnemyField[turn.Coordinate.Row, turn.Coordinate.Column].UpdateState(CellState.Miss);
        }

        private bool IsPlayerLost(int playerId)
        {
            return GetPlayer(playerId).Field.GetCellsCountWithState(CellState.Kill) == 20;
        }

        private bool IsBothPlayersReachedTurnsLimit()
        {
            var player1TurnsCount = _turnsHistory.Count(t => t.PlayerId == _player1.IngameId);
            var player2TurnsCount = _turnsHistory.Count(t => t.PlayerId == _player2.IngameId);

            return player1TurnsCount >= 100 && player2TurnsCount >= 100;
        }

        private GameResult FinishGameByTurnsLimit()
        {
            var player1HitsCount = _fields[1].Field.GetCellsCountWithState(CellState.Hit) +
                                   _fields[1].Field.GetCellsCountWithState(CellState.Kill);
            
            var player2HitsCount = _fields[0].Field.GetCellsCountWithState(CellState.Hit) +
                                   _fields[0].Field.GetCellsCountWithState(CellState.Kill);

            PlayerDto winner;
            
            if (player1HitsCount > player2HitsCount)
            {
                winner = _playerDtos[0];
            }
            else if (player2HitsCount > player1HitsCount)
            {
                winner = _playerDtos[1];
            }
            else
            {
                winner = _playerDtos[CoinFlip()];
            }
            
            return GetGameResult(winner);
        }

        private GameResult GetGameResult(PlayerDto winner)
        {
            return new GameResult
                   {
                       StartTime = _gameStartTime,
                       EndTime = DateTime.Now,
                       Participant1 = new ParticipantModel { PlayerDto = _playerDtos[0], IngamePlayerId = _player1.IngameId },
                       Participant2 = new ParticipantModel { PlayerDto = _playerDtos[1], IngamePlayerId = _player2.IngameId },
                       Winner = winner,
                       TurnsHistory = _turnsHistory,
                       Participant1StartField = _startPlayer1Field,
                       Participant2StartField = _startPlayer2Field
                   };
        }
        
        private int CoinFlip()
        {
            return _random.Next(0, 1);
        }

        private int SwapId(int currentId)
        {
            return currentId == 0
                       ? 1
                       : 0;
        }

        private Player CreatePlayer(PlayerDto playerDto, int participantIndex)
        {
            var playerField = _fields[participantIndex].Field;

            var enemyField = _fields[SwapId(participantIndex)].FieldForEnemy;

            var strategyWrapper = playerDto.StrategyAssembly != null 
                                      ? CreateStrategyWrapper(playerDto.StrategyAssembly)
                                      : CreateStrategyWrapper(playerDto.Strategy);
            
            strategyWrapper.Setup(playerField, enemyField, participantIndex);

            return new Player
                   {
                       IngameId = participantIndex,
                       Field = playerField,
                       EnemyField = enemyField,
                       Strategy = strategyWrapper,
                       TurnsHistory = new Queue<TurnResult>()
                   };
        }

        private StrategyWrapper CreateStrategyWrapper(byte[] assembly)
        {
            return new StrategyWrapper(assembly);
        }

        private StrategyWrapper CreateStrategyWrapper(PlayerStrategy strategy)
        {
            return new StrategyWrapper(strategy);
        }
    }
}