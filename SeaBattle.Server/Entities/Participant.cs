namespace SeaBattle.Server.Entities
{
    using System.Collections.Generic;

    public class Participant
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public int TelegramId { get; set; }
        
        public byte[] Strategy { get; set; }
        
        public Statistic Statistic { get; set; }
        
        public List<StrategySource> StrategySources { get; set; }
    }
}