namespace PracticeStrategies
{
    using System;
    using System.IO;
    using System.Linq;

    public class Practice
    {
        public static string GetStrategy(StrategyType strategyType)
        {
            switch (strategyType)
            {
                case StrategyType.Cycle:
                    return GetStrategyPath("CycleStrategy.dll");
                case StrategyType.FullRandom:
                    return GetStrategyPath("FullRandomStrategy.dll");
                default:
                    return null;
            }
        }

        private static string GetStrategyPath(string resourceName)
        {
            var currentAssembly = typeof(Practice).Assembly;

            var stream = currentAssembly.GetManifestResourceStream(currentAssembly.GetManifestResourceNames().First(name => name.EndsWith(resourceName)));

            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{resourceName}");
            
            using (var fs = File.Create(tempPath))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fs);
            }

            return tempPath;
        }
    }
}