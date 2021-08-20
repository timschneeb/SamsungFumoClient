using System;
using System.Runtime.CompilerServices;

namespace SamsungFumoClient.Utils
{
    public class LogEventArgs : EventArgs
    {
        public Log.Severity Severity;
        public string Message;

        public LogEventArgs(Log.Severity severity, string message)
        {
            Severity = severity;
            Message = message;
        }
    }

    
    public static class Log
    {
        public enum Severity
        {
            VRB,
            DBG,
            INF,
            WRN,
            ERR
        }
        
        public static EventHandler<LogEventArgs>? OnLogEvent;
        
        public static void V(string str)
        {
            WriteLine(Severity.VRB, str);
        }

        public static void D(string str)
        {
            WriteLine(Severity.DBG, str);
        }

        public static void I(string str)
        {
            WriteLine(Severity.INF, str);
        }

        public static void W(string str)
        {
            WriteLine(Severity.WRN, str);
        }

        public static void E(string str)
        {
            WriteLine(Severity.ERR, str);
        }

        private static void WriteLine(Severity sev, string msg)
        {
            if (OnLogEvent == null)
            {
                Console.WriteLine($"[{sev}] {msg}");
            }
            else
            {
                OnLogEvent.Invoke(null, new LogEventArgs(sev, msg));
            }
        }
    }
}