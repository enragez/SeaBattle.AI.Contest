namespace ConsoleViewer
{
    using System;
    using PracticeStrategies;

    internal class Program
    {
        public static void Main(string[] args)
        {
            var str1 = Practice.GetStrategy(StrategyType.Cycle);

            var str2 = Practice.GetStrategy(StrategyType.FullRandom);
            
            var engine = new SeaWarsEngine.Engine(str1, str2);
            
            engine.StartGame();

            Console.WriteLine("zaebok");
            Console.ReadLine();
        }
    }
}