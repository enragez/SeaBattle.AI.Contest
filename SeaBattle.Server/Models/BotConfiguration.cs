namespace SeaBattle.Server.Models
{
    public class BotConfiguration
    {
        public string BotToken { get; set; }

        public string WebhookUrl { get; set; }
        
        public string Socks5Host { get; set; }

        public int Socks5Port { get; set; }
    }
}