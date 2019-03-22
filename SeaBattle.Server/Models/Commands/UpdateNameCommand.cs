namespace SeaBattle.Server.Models.Commands
{
    using System.Threading.Tasks;
    using Services;
    using StateMachine.UpdateName;
    using Telegram.Bot.Types;

    public class UpdateNameCommand : ICommand
    {
        private readonly IServiceWithState<UpdateNameState> _updateService;
        
        public string Name => "updatename";

        public UpdateNameCommand(IServiceWithState<UpdateNameState> updateService)
        {
            _updateService = updateService;
        }
        
        public async Task Execute(Update update)
        {
            await _updateService.MoveMext(update);
        }
    }
}