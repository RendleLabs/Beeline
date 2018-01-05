using System;
using BenchmarkDotNet.Running;

namespace Beeline.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmarks>();
            Console.WriteLine(summary.Table);
        }
    }
}
