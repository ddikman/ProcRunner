using System;

namespace GenerateOutputs
{
    public static class SampleExtension
    {
        private static readonly Random Random = new Random();

        public static char Sample(this string str)
        {
            return str[Random.Next(0, str.Length - 1)];
        }
    }
}