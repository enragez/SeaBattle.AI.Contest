namespace SeaBattle.Engine.Strategy
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using Exceptions;
    using Models;
    using Models.Field;
    using Models.Serializable;

    internal class StrategyWrapper
    {
        private readonly PlayerStrategy _strategy;
        
        internal StrategyWrapper(byte[] assemblyBytes)
        {
            var assembly = Assembly.Load(assemblyBytes);

            var module = assembly.Modules.FirstOrDefault();

            if (module.GetCustomAttribute<UnverifiableCodeAttribute>() != null)
            {
                throw new CheatDetectedException("unsafe себе в очко затолкай");
            }
            
            var type = assembly.GetTypes().FirstOrDefault(t => !t.IsAbstract && typeof(PlayerStrategy).IsAssignableFrom(t));

            var strategy = Activator.CreateInstance(type);

            _strategy = (PlayerStrategy) strategy;
        }

        internal StrategyWrapper(PlayerStrategy strategy)
        {
            _strategy = strategy;
        }

        internal void Setup(Field playerField, Field enemyField, int playerId)
        {
            _strategy.Setup(playerField, enemyField, playerId);
        }

        internal void PrepareField()
        {
            _strategy.PrepareField();
        }

        internal Coordinate DoTurn(TurnResult turn)
        {
            return _strategy.DoTurn(turn);
        }
    }
}