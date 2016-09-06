using System;
using System.Linq;

namespace GenerateOutputs
{
    /// <summary>Generates a very large single output</summary>
    internal class BufferOverflow
    {
        public static void Generate(string[] generatorArgs)
        {
            // when testing I found that the process.Output.ReadToEnd() returned 0 if given an output of the below size
            var characters = new string(Enumerable.Range('a', 'z' - 'a').Select(i => (char) i).ToArray());
            characters = characters + characters.ToUpper();

            var length = int.Parse(generatorArgs[0]);
            var buffer = new string(Enumerable.Range(0, length).Select(_ => characters.Sample()).ToArray());
            Console.Out.Write(buffer);
        }
    }
}