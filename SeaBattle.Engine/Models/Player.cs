namespace SeaBattle.Engine.Models
{
    using System.Collections.Generic;
    using Enums;
    using Serializable;
    using Strategy;

    internal class Player
    {
        internal StrategyWrapper Strategy { get; set; }
        
        internal Field.Field Field { get; set; }
        
        internal Field.Field EnemyField { get; set; }

        internal int IngameId { get; set; }
        
        internal Queue<TurnResult> TurnsHistory { get; set; }

        internal void GamePhaseChanged(GamePhase gamePhase)
        {
            Field.GamePhaseChanged(gamePhase);
            EnemyField.GamePhaseChanged(gamePhase);
        }
    }
}