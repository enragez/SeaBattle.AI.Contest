namespace SeaBattle.Practice
{
    using Engine;
    using Strategies;

    public class Practice
    {
        public static PlayerStrategy GetStrategy(StrategyType strategyType)
        {
            switch (strategyType)
            {
                case StrategyType.FullRandom: 
                    return new FullRandom();
                case StrategyType.Cycle:
                    return new Cycle();
                default:
                    return null;
            }
        }
    }
}