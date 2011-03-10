using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Snowcode.S3BuildPublisher.Logging
{
    class MsBuildTaskLogger : ITaskLogger
    {
        private readonly TaskLoggingHelper _log;

        public MsBuildTaskLogger(TaskLoggingHelper log)
        {
            if (log == null) throw new ArgumentNullException("log");

            _log = log;
        }

        public void LogMessage(MessageImportance messageImportance, string message, params object[] messageArgs)
        {
            _log.LogMessage(messageImportance, message, messageArgs);
        }

        public void LogMessage(string message, params object[] messageArgs)
        {
            _log.LogMessage(message, messageArgs);
        }

        public void LogErrorFromException(Exception exception)
        {
            _log.LogErrorFromException(exception);
        }
    }
}
