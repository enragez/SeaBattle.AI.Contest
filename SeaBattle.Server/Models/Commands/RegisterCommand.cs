namespace SeaBattle.Server.Models.Commands
{
    using System.Threading.Tasks;
    using Services;
    using Telegram.Bot.Types;

    public class RegisterCommand : ICommand
    {
        private readonly IRegisterService _registerService;
        public string Name => "register";

        public RegisterCommand(IRegisterService registerService)
        {
            _registerService = registerService;
        }
        
        public async Task Execute(Update update)
        {
            // todo: if(registered) send update command tips else -> movenext
            
            await _registerService.MoveMext(update);
        }
    }
}