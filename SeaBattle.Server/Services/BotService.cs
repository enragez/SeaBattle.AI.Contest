namespace SeaBattle.Server.Services
{
    using System.Configuration;
    using System.Threading.Tasks;
    using Telegram.Bot;

    public class BotService : IBotService
    {
        private readonly string _botWebhookUrl;
        
        public BotService()
        {
            var botToken = ConfigurationManager.AppSettings["BotToken"];
            _botWebhookUrl = ConfigurationManager.AppSettings["WebhookUrl"];
            Client = new TelegramBotClient(botToken);
        }

        public TelegramBotClient Client { get; }
        
        public async Task SetWebhook()
        {
            await Client.DeleteWebhookAsync();
            
            await Client.SetWebhookAsync(_botWebhookUrl);
        }
    }
}