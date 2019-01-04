using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

        [Conditional("DEBUG")]
        public static void Log(string message, LogLevel level = LogLevel.DEBUG)
        {
            string caller = new StackFrame(1).GetMethod().Name;

            Console.WriteLine("[" + level.ToString() + "]" + " at " + caller + ": " + message);
        }
    }
}
