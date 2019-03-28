namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Models;
    using Telegram.Bot;

    public class BotService : IBotService
    {
        private readonly BotConfiguration _config;

        private bool _webHookSetted;
        
        public BotService(IOptions<BotConfiguration> config)
        {
            _config = config.Value;
            Client = new TelegramBotClient(_config.BotToken);
        }

        public TelegramBotClient Client { get; }
        
        public async Task SetWebhook()
        {
            if (_webHookSetted)
            {
                return;
            }
            
            await Client.SetWebhookAsync(_config.WebhookUrl);
            _webHookSetted = true;
        }

        public long ChannelId => _config.ChannelId;
    }
}