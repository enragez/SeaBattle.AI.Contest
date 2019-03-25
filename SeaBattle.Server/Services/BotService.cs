namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Models;
    using Telegram.Bot;

    public class BotService : IBotService
    {
        private readonly BotConfiguration _config;

        public BotService(IOptions<BotConfiguration> config)
        {
            _config = config.Value;
            // use proxy if configured in appsettings.*.json
            Client = new TelegramBotClient(_config.BotToken);
        }

        public TelegramBotClient Client { get; }
        
        public async Task SetWebhook()
        {
            await Client.SetWebhookAsync(_config.WebhookUrl);
        }
    }
}