namespace SeaBattle.Server.Core.Services
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public interface IUpdateService
    {
        Task EchoAsync(Update update);
    }
}