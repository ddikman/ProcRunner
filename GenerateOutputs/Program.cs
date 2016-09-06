using System;
using System.Linq;

namespace GenerateOutputs
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // args = new string[] {"waitforcancel", "20000"};
                SimpleLog.Write("Running with arguments: " + string.Join(" ", args));
                var mode = (GenerateMode) Enum.Parse(typeof (GenerateMode), args[0], true);
                var generatorArgs = args.Skip(1).ToArray();
                switch (mode)
                {
                    case GenerateMode.BufferOverflow:
                        BufferOverflow.Generate(generatorArgs);
                        break;
                    case GenerateMode.Output:
                        Output.Generate(generatorArgs);
                        break;
                    case GenerateMode.Error:
                        Error.Generate(generatorArgs);
                        break;
                    case GenerateMode.Timeout:
                        Timeout.Generate(generatorArgs);
                        break;
                    case GenerateMode.WaitForCancel:
                        WaitForCancel.Run(generatorArgs);
                        break;
                    default:
                        throw new Exception("Unknown mode: " + mode);
                }
            }
            catch(Exception e)
            {
                SimpleLog.Write("Unhandled failure.", e);
                throw;
            }
        }
    }
}
