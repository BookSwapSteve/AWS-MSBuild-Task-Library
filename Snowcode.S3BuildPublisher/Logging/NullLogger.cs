using System;
using System.Diagnostics;
using Microsoft.Build.Framework;

namespace Snowcode.S3BuildPublisher.Logging
{
    public class NullLogger : ITaskLogger
    {
        public void LogMessage(MessageImportance messageImportance, string message, params object[] messageArgs)
        { }

        public void LogMessage(string message, params object[] messageArgs)
        { }

        public void LogErrorFromException(Exception exception)
        {
            Debug.WriteLine(exception.ToString());
        }
    }
}
