namespace SeaBattle.Server.StateMachine
{
    using System.Threading.Tasks;
    using Models;
    using Telegram.Bot.Types;

    public interface IRegistrationStateMachine
    {
        RegistrationState State { get; }

        Task MoveNext(Update update);

        RegistrationModel Register();
    }
}