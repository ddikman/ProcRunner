using System;
using System.Diagnostics;
using System.IO;

namespace GenerateOutputs
{
    /// <summary>Incredibly simple log</summary>
    public class SimpleLog
    {
        public const string Path = "Simple.log";

        public static bool Exists => File.Exists(Path);

        private static void Write(string invokingMethod, string msg)
        {
            var now = DateTime.Now;
            msg = $"[{now.ToShortDateString()} {now.ToLongTimeString()}] {invokingMethod} > " + msg + "\r\n";
            using (var file = File.AppendText(Path))
                file.Write(msg);
        }

        /// <summary>Write a single line message</summary>
        /// <param name="msg">Message to write</param>
        public static void Write(string msg)
        {
            Write(InvokingMethod, msg);
        }

        /// <summary>Write a single line message and a full dump of the given exception</summary>
        /// <param name="error">Error message</param>
        /// <param name="e">Exception to dump</param>
        public static void Write(string error, Exception e)
        {
            Write(InvokingMethod, error + "\r\n" + e.GetDump());
        }

        /// <summary>Gets the name and class of the invoking method</summary>
        public static string InvokingMethod
        {
            get
            {
                var method = new StackTrace().GetFrame(1).GetMethod();
                if (method.DeclaringType != null)
                    return method.DeclaringType?.Name + "." + method.Name;

                return method.Name;
            }
        }

        /// <summary>Resets the log, starting next log line in an empty log file</summary>
        public static void Reset()
        {
            if(Exists)
                File.Delete(Path);
        }
    }
}
