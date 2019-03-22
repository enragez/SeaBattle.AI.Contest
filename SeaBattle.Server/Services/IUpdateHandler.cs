namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public interface IUpdateHandler
    {
        Task Handle(Update update);
    }
}