namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Telegram.Bot;
    using Telegram.Bot.Types;

    public interface IBotService
    {
        TelegramBotClient Client { get; }

        Task SetWebhook();
        
        long ChannelId { get; }

        Task<Message> SendTextMessageAsync(long chatId, string text);
    }
}