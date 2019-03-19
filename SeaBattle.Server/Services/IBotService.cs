namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Telegram.Bot;
    
    public interface IBotService
    {
        TelegramBotClient Client { get; }

        Task SetWebhook();
    }
}