using System;
using System.IO;

namespace GenerateOutputs
{
    internal class Output
    {
        public static void Generate(string[] generatorArgs)
        {
            if (generatorArgs[0] == "-f")
            {
                var contents = File.ReadAllText(generatorArgs[1]);
                Console.Out.Write(contents);
            }
            else
                Console.Out.Write(generatorArgs[0]);
        }
    }
}