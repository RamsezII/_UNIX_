using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    internal static class LOG_CATCHER
    {
        public struct LogMsg
        {
            public string msg;
            public string stackTrace;
            public LogType type;
        }

        public static readonly Queue<LogMsg> logqueue = new();

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void TheSeed()
        {
            logqueue.Clear();
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
        }

        //----------------------------------------------------------------------------------------------------------

        static void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            bool sublog = message.StartsWith("-s");
            if (sublog)
                message = message[2..];

            if (TERMINAL_base.logfocus.TryGetValue(out TERMINAL_base terminal))
            {
                SIGT sig = sublog ? SIGT.SUBLOG : type switch
                {
                    LogType.Warning => SIGT.WARN,
                    LogType.Error => SIGT.ERR,
                    LogType.Exception => SIGT.EXCP,
                    _ => SIGT.LOG
                };
                terminal.OnSignal(new(null, sig, message, data: type == LogType.Exception ? stackTrace : null));
            }

            lock (logqueue)
            {
                while (logqueue.Count > 50)
                    logqueue.Dequeue();
                logqueue.Enqueue(new LogMsg { msg = message, stackTrace = stackTrace, type = type });
            }
        }
    }
}