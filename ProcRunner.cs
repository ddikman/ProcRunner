using System.Diagnostics;
using System.Text;

namespace ProcRunner
{
    public class ProcRunner
    {
        private readonly string _executablePath;
        private readonly string _arguments;
        private ProcessStartInfo _processInfo;
        private Process _process;
        private readonly StringBuilder _output = new StringBuilder();
        private readonly StringBuilder _error = new StringBuilder();

        public static int DefaultTimeoutMs = 1000*60; // 1 minute

        public ProcRunner(string executablePath, string arguments)
        {
            _executablePath = executablePath;
            _arguments = arguments;

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
    }
}
