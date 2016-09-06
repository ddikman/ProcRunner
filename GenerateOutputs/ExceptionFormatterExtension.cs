using System;
using System.Collections.Generic;
using System.Text;

namespace GenerateOutputs
{
    public static class ExceptionFormatterExtension
    {
        public static string GetDump(this Exception e)
        {
            var exceptions = new Stack<Exception>();
            while (e != null)
            {
                exceptions.Push(e);
                e = e.InnerException;
            }

            StringBuilder sb = new StringBuilder();
            while (exceptions.Count > 0)
            {
                e = exceptions.Pop();
                sb.AppendLine(e.GetType().Name + ": " + e.Message);
                sb.AppendLine(e.StackTrace);
                sb.AppendLine();
            }

            return sb.ToString().Trim();
        }
    }
}