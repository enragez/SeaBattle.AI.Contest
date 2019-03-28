namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Dal.Entities;
    using Engine.Models;
    using Participant = Dal.Entities.Participant;

    public interface IGameRunner
    {
        Task<(PlayedGame, GameResult)> StartGameAsync(Participant player1, Participant player2, bool ratedGame);
    }
}