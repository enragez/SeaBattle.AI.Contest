namespace SeaBattle.Engine.Models
{
    public class Participant
    {
        public int Id { get; set; }
        
        public string StrategyAssemblyPath { get; set; }
        
        public byte[] StrategyAssembly { get; set; }
        
        public PlayerStrategy Strategy { get; set; }
    }
}