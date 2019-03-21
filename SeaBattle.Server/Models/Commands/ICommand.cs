namespace SeaBattle.Server.Models.Commands
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public interface ICommand
    {
        string Name { get; }

        Task Execute(Update update);
    }
}