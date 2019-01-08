using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace serial_monitor.Utils
{
    public class Debugger
    {
        public enum LogLevel
        {
            TRACE = 0,
            DEBUG = 1,
            INFO = 2,
            WARN = 3,
            ERROR = 4,
            FATAL = 5
        }

        public delegate void LogEventHandler(string log);
        public static event LogEventHandler LogRaised;

        public static Queue<string> LogQueue { get; set; } = new Queue<string>(1024);

        public static void Log(string message, LogLevel level = LogLevel.DEBUG)
        {
            string caller = new StackFrame(1).GetMethod().Name;
            string log = "[" + level.ToString() + "]" + " at " + caller + ": " + message;

            WriteToConsole(log);

            LogQueue.Enqueue(log);
            LogRaised?.Invoke(log);
        }

        [Conditional("DEBUG")]
        private static void WriteToConsole(string message)
        {
            Console.WriteLine(message);
        }
    }
}
