namespace SeaBattle.Console
{
    using System;
    using Engine;
    using Engine.Models;
    using Practice;

    internal class Program
    {
        public static void Main(string[] args)
        {
            var rnd = new Random();

            var participant1 = new Participant
                               {
                                   Id = rnd.Next(0, 100),
                                   Strategy = Practice.GetStrategy(StrategyType.Cycle)
                               };
            
            var participant2 = new Participant
                               {
                                   Id = rnd.Next(100, 200),
                                   Strategy = Practice.GetStrategy(StrategyType.FullRandom)
                               };
            
            var engine = new Engine(participant1, participant2);
            
            var result = engine.StartGame();

            var resultFile = new GameResultSaver().SaveGameResult(result);

            Console.WriteLine("zaebok");
            Console.ReadLine();
        }
    }
}