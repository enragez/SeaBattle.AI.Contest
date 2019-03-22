namespace SeaBattle.Server.Models.Commands
{
    using System.Threading.Tasks;
    using Services;
    using StateMachine.UpdateStrategy;
    using Telegram.Bot.Types;

    public class UpdateStrategyCommand : ICommand
    {
        private readonly IServiceWithState<UpdateStrategyState> _updateService;
        
        public string Name => "updatestrategy";

        public UpdateStrategyCommand(IServiceWithState<UpdateStrategyState> updateService)
        {
            _updateService = updateService;
        }
        
        public async Task Execute(Update update)
        {
            await _updateService.MoveMext(update);
        }
    }
}