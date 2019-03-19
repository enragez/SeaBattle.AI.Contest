namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public interface IUpdateService
    {
        Task EchoAsync(Update update);
    }
}