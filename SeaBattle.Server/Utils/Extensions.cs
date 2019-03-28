namespace SeaBattle.Server.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Extensions
    {
        private static readonly Random Random = new Random();
        
        public static IEnumerable<T> RandomPermutation<T>(this IEnumerable<T> sequence)
        {
            var retArray = sequence.ToArray();

            for (var i = 0; i < retArray.Length - 1; i += 1)
            {
                var swapIndex = Random.Next(i, retArray.Length);
                if (swapIndex == i)
                {
                    continue;
                }

                var temp = retArray[i];
                retArray[i] = retArray[swapIndex];
                retArray[swapIndex] = temp;
            }

            return retArray;
        }
    }
}