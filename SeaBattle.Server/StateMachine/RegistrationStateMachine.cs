namespace SeaBattle.Server.StateMachine
{
    using System.Threading.Tasks;
    using Models;
    using Services;
    using Telegram.Bot.Types;

    public class RegistrationStateMachine : IRegistrationStateMachine
    {
        private readonly IBotService _botService;
        public RegistrationState State { get; private set; } = RegistrationState.Started;

        private RegistrationModel _registration;

        public RegistrationStateMachine(IBotService botService)
        {
            _botService = botService;
        }

        public async Task MoveNext(Update update)
        {
            switch (State)
            {
                case RegistrationState.Started:
                    // todo send message to ask name
                    _registration = new RegistrationModel
                                    {
                                        TelegramId = update.Message.From.Id
                                    };
                    await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                                 "Пожалуйста, укажите ваше имя");
                    State = RegistrationState.WaitingForName;
                    break;
                case RegistrationState.WaitingForName:
                    // todo save name if valid then change state to WaitingForStrategy then ask to send strategy
                    _registration.Name = update.Message.Text;
                    await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                                 "Пришлите вашу стратегию. Стратегии принимаются в формате .zip файла содержащего набор cs-файлов.");
                    State = RegistrationState.WaitingForStrategy;
                    
                    break;
                case RegistrationState.WaitingForStrategy:
                    // todo save strategy if valid then change state to Registered
                    _registration.StrategyFile = await _botService.Client.GetFileAsync(update.Message.Document.FileId);
                    await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                                 "Регистрация завершена");
                    State = RegistrationState.Registered;
                    break;
                
            }
        }

        public RegistrationModel Register()
        {
            return State != RegistrationState.Registered 
                       ? null : 
                       _registration;
        }
    }
}