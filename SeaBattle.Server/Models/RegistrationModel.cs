namespace SeaBattle.Server.Models
{
    using System.IO;

    public class RegistrationModel
    {
        public int TelegramId { get; set; }
        
        public string Name { get; set; }
        
        public byte[] StrategyAssembly { get; set; }
    }
}