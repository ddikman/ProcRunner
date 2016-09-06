using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace GenerateOutputs
{
    internal class WaitForCancel
    {
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);

        // A delegate type to be used as the handler routine 
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(int ctrlType);

        public static void Run(string[] generatorArgs)
        {
            var waitMs = int.Parse(generatorArgs[0]);

            var cancelled = false;

            // Doesn't work when running in debugger
            SetConsoleCtrlHandler(ctrlTypes =>
            {
                cancelled = true;
                return true;
            }, true);

            var stopwatch = Stopwatch.StartNew();
            while (!cancelled)
            {
                if(stopwatch.ElapsedMilliseconds > waitMs)
                    throw new Exception($"Wasn't cancelled within {waitMs}ms.");

                Thread.Sleep(100);
            }

            Console.Out.Write("Was cancelled");
            SimpleLog.Write("Was cancelled");
            Environment.ExitCode = 2;
        }
    }
}