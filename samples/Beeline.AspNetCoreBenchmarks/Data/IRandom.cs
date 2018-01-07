namespace Beeline.AspNetCoreBenchmarks.Data
{
    public interface IRandom
    {
        int Next(int minValue, int maxValue);
    }
}