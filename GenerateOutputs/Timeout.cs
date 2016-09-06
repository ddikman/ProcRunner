namespace GenerateOutputs
{
    internal class Timeout
    {
        public static void Generate(string[] generatorArgs)
        {
            var waitMs = int.Parse(generatorArgs[0]);
            System.Threading.Thread.Sleep(waitMs);
        }
    }
}