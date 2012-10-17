using System;

namespace Simple.Logging
{
    public interface ILog
    {
        void Log(LogLevel level, string message);

        void Error(string message, Exception exception);

        void Fatal(string message, Exception exception);

    }
}