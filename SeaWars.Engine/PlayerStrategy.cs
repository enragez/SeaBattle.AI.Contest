namespace SeaWars.Engine
{
    using Models;

    public abstract class PlayerStrategy
    {
        protected Field MyField { get; private set; }
        
        protected Field EnemyField { get; private set; }
        
        protected int MyId { get; private set; }

        internal void Setup(Field myField, Field enemyField, int playerId)
        {
            MyField = myField;
            EnemyField = enemyField;
            MyId = playerId;
        }
        
        public abstract void PrepareField();

        public abstract Coordinate DoTurn(TurnResult turn);
    }
}