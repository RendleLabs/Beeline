using System;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace Beeline.Benchmarks
{
    [MemoryDiagnoser]
    [CoreJob]
    public class Benchmarks
    {
        private static readonly MemoryStream BeelineStream = new MemoryStream();
        private static Random _beelineRandom;
        private static readonly MemoryStream EfStream = new MemoryStream();
        private static Random _efRandom;
        private static readonly MemoryStream DapperStream = new MemoryStream();
        private static Random _dapperRandom;

        [GlobalSetup]
        public void DatabaseSetup()
        {
            Database.Setup();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _beelineRandom = new Random(42);
            _efRandom = new Random(42);
            _dapperRandom = new Random(42);
        }
        
        [Benchmark]
        public void Beeline()
        {
            BeelineStream.Position = 0;
            int from = _beelineRandom.Next(1, 900);
            BeelineImplementation.RunList(BeelineStream, from, from + 100).GetAwaiter().GetResult();
        }
        
        [Benchmark]
        public void Dapper()
        {
            DapperStream.Position = 0;
            int from = _dapperRandom.Next(1, 900);
            DapperImplementation.RunList(DapperStream, from, from + 100).GetAwaiter().GetResult();
        }
        
        [Benchmark(Baseline = true)]
        public void EntityFramework()
        {
            EfStream.Position = 0;
            int from = _efRandom.Next(1, 900);
            EfImplementation.RunList(EfStream, from, from + 100).GetAwaiter().GetResult();
        }
    }
}