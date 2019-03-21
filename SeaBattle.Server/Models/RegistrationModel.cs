namespace SeaBattle.Server.Models
{
    using Telegram.Bot.Types;

    public class RegistrationModel
    {
        public long TelegramId { get; set; }
        
        public string Name { get; set; }
        
        public File StrategyFile { get; set; }
    }
}