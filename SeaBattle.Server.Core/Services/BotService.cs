namespace SeaBattle.Server.Core.Services
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using MihaZupan;
    using Models;
    using Telegram.Bot;

    public class BotService : IBotService
    {
        private readonly BotConfiguration _config;

        public BotService(IOptions<BotConfiguration> config)
        {
            _config = config.Value;
            // use proxy if configured in appsettings.*.json
            Client = string.IsNullOrEmpty(_config.Socks5Host)
                         ? new TelegramBotClient(_config.BotToken)
                         : new TelegramBotClient(
                                                 _config.BotToken,
                                                 new HttpToSocks5Proxy(_config.Socks5Host, _config.Socks5Port));

           
        }

        public TelegramBotClient Client { get; }
        
        public async Task SetWebhook()
        {
            await Client.SetWebhookAsync(_config.WebhookUrl);
        }
    }
}