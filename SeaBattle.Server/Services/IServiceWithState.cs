namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public interface IServiceWithState
    {
        Task MoveMext(Update update);

        bool IsActive(int userId);
    } 
    
    public interface IServiceWithState<TState> : IServiceWithState
    {
    }
}