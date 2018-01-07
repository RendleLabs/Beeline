using System;
using System.Threading;

namespace Beeline.AspNetCoreBenchmarks.Data
{
    public class DefaultRandom : IRandom
    {
        private static int _nextSeed = 0;
        // Random isn't thread safe
        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _nextSeed)));

        public int Next(int minValue, int maxValue)
        {
            return Random.Value.Next(minValue, maxValue);
        }
    }
}