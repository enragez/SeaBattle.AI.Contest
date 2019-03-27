namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public interface IBotUpdateHandler
    {
        Task Handle(Update update);
    }
}