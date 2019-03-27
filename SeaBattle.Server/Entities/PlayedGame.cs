namespace SeaBattle.Server.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class PlayedGame
    {
        public int Id { get; set; }
        
        public bool Rated { get; set; }
        
        [Column(TypeName = "jsonb")]
        public string Result { get; set; }
    }
}