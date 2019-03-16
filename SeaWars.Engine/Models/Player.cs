namespace SeaWars.Engine.Models
{
    using System.Collections.Generic;

    internal class Player
    {
        internal StrategyWrapper Strategy { get; set; }
        
        internal Field Field { get; set; }
        
        internal Field EnemyField { get; set; }

        internal int Id { get; set; }
        
        internal Queue<TurnResult> TurnsHistory { get; set; }

        internal void GamePhaseChanged(GamePhase gamePhase)
        {
            Field.GamePhaseChanged(gamePhase);
            EnemyField.GamePhaseChanged(gamePhase);
        }
    }
}