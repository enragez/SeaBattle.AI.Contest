namespace SeaBattle.Server.Entities
{
    public class Participant
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public int TelegramId { get; set; }
        
        public byte[] Strategy { get; set; }
        
        public Statistic Statistic { get; set; }
    }
}