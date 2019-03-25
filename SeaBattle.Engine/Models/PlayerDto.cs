namespace SeaBattle.Engine.Models
{
    using Strategy;

    public class PlayerDto
    {
        public int Id { get; set; }
        
        public byte[] StrategyAssembly { get; set; }
        
        public PlayerStrategy Strategy { get; set; }
    }
}