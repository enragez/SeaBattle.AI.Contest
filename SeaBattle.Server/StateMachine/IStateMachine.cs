namespace SeaBattle.Server.StateMachine
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public interface IStateMachine<out TState>
    {
        TState State { get; }

        Task MoveNext(Update update);

        Task Finish(Update update);
    }
}