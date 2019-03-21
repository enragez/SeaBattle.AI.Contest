namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public interface IRegisterService
    {
        Task MoveMext(Update update);

        bool UserRegistering(int userId);
    }
}