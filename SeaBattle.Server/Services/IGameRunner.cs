namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Engine.Models;
    using Entities;
    using Participant = Entities.Participant;

    public interface IGameRunner
    {
        Task<(PlayedGame, GameResult)> StartGameAsync(Participant player1, Participant player2, bool ratedGame);
    }
}