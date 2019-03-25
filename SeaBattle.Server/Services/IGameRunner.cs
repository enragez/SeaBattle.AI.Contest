namespace SeaBattle.Server.Services
{
    using Engine.Models;
    using Entities;
    using Participant = Entities.Participant;

    public interface IGameRunner
    {
        (PlayedGame, GameResult) StartGame(Participant player1, Participant player2, bool ratedGame);
    }
}