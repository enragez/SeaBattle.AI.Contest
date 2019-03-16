namespace ConsoleViewer
{
    using System;
    using PracticeStrategies;
    using SeaWarsEngine;
    using SeaWarsEngine.Models;

    internal class Program
    {
        public static void Main(string[] args)
        {
            var rnd = new Random();

            var participant1 = new Participant
                               {
                                   Id = rnd.Next(0, 100),
                                   StrategyAssemblyPath = Practice.GetStrategy(StrategyType.Cycle)
                               };
            
            var participant2 = new Participant
                               {
                                   Id = rnd.Next(100, 200),
                                   StrategyAssemblyPath = Practice.GetStrategy(StrategyType.FullRandom)
                               };

            var gameId = rnd.Next(0, 100);
            
            var engine = new SeaWarsEngine.Engine(gameId, participant1, participant2);
            
            var result = engine.StartGame();

            var resultFile = new GameResultSaver().SaveGameResult(result);

            Console.WriteLine("zaebok");
            Console.ReadLine();
        }
    }
}