namespace SeaBattle.Server.Models
{
    using System.IO;

    public class RegistrationModel
    {
        public int TelegramId { get; set; }
        
        public string Name { get; set; }
        
        public MemoryStream StrategyStream { get; set; }
    }
}