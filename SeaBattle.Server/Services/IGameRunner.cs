namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Engine.Models;
    using Participant = Entities.Participant;

    public interface IGameRunner
    {
        Task<GameResult> StartGameAsync(Participant player1, Participant player2, bool ratedGame);
    }
}