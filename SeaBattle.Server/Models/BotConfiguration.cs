namespace SeaBattle.Server.Models
{
    public class BotConfiguration
    {
        public long ChannelId { get; set; }
        
        public string BotToken { get; set; }

        public string WebhookUrl { get; set; }
    }
}