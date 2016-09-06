using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcRunner
{
    public class ProcRunner : IDisposable
    {
        private readonly ProcessStartInfo _processInfo;
        private Process _process;
        private readonly StringBuilder _output = new StringBuilder();
        private readonly StringBuilder _error = new StringBuilder();

        public static int DefaultTimeoutMs = 1000*60; // 1 minute

        public ProcRunner(string executablePath, string arguments)
        {
            _processInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = executablePath,
                Arguments = arguments,
                RedirectStandardError = true,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
        }

        public bool HasExited { get; private set; }
        public bool Failed => ExitCode != 0;
        public bool Timeout { get; private set; }
        public int ExitCode { get; private set; }

        public bool WaitForExit()
        {
            return WaitForExit(DefaultTimeoutMs);
        }

        public bool WaitForExit(int timeoutMs)
        {
            if (!_process.WaitForExit(timeoutMs))
                Timeout = true;
            else
                ExitCode = _process.ExitCode;

            HasExited = true;
            return !Failed && !Timeout;
        }

        public void Start()
        {
            _process = new Process
            {
                StartInfo = _processInfo
            };

            _process.OutputDataReceived += OnOutput;
            _process.ErrorDataReceived += OnError;

            _process.Start();

            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
        }

        private void OnOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;
            if (_output.Length > 0) _output.AppendLine();
            _output.Append(e.Data);
        }

        private void OnError(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;
            if (_error.Length > 0) _error.AppendLine();
            _error.Append(e.Data);
        }

        public string Output => _output.ToString();
        public string Error => _error.ToString();

        public void KillGracefully()
        {
            if (HasExited)
                return;

            StopProgram(_process);
            if (!_process.HasExited)
                _process.Kill();
        }

        #region Stop a Process
        // See information at http://stanislavs.org/stopping-command-line-applications-programatically-with-ctrl-c-events-from-net/

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();

        private const int CtrlCEvent = 0;

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

        // Delegate type to be used as the Handler Routine for SCCH
        delegate Boolean ConsoleCtrlDelegate(uint ctrlType);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handlerRoutine, bool add);

        private void StopProgram(Process proc)
        {
            //This does not require the console window to be visible.
            if (!AttachConsole((uint) proc.Id))
                return;

            // Disable Ctrl-C handling for our program
            SetConsoleCtrlHandler(null, true);
            GenerateConsoleCtrlEvent(CtrlCEvent, 0);

            // Must wait here. If we don't and re-enable Ctrl-C
            // handling below too fast, we might terminate ourselves.
            WaitForExit(2000);

            FreeConsole();

            //Re-enable Ctrl-C handling or any subsequently started
            //programs will inherit the disabled state.
            SetConsoleCtrlHandler(null, false);
        }

        #endregion

        public void Dispose()
        {
            if(_process != null && !HasExited)
                StopProgram(_process);
        }
    }
}
