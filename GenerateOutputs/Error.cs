using System;
using System.IO;

namespace GenerateOutputs
{
    internal class Error
    {
        public static void Generate(string[] generatorArgs)
        {
            var exitCode = int.Parse(generatorArgs[0]);
            if (generatorArgs[1] == "-f")
            {
                var contents = File.ReadAllText(generatorArgs[2]);
                Console.Error.Write(contents);
            }
            else
                Console.Error.Write(generatorArgs[1]);

            Environment.ExitCode = exitCode;
        }
    }
}