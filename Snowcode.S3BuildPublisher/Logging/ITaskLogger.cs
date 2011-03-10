using System;
using Microsoft.Build.Framework;

namespace Snowcode.S3BuildPublisher.Logging
{
    public interface ITaskLogger
    {
        void LogMessage(MessageImportance messageImportance, string message, params object[] messageArgs);
        void LogMessage(string message, params object[] messageArgs);
        void LogErrorFromException(Exception exception);
    }
}
