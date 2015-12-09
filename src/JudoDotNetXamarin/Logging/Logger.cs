using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JudoPayDotNet.Logging;
using Serilog;
using Serilog.Events;

namespace JudoDotNetXamarin.Logging
{
    class Logger : ILog
    {
        private bool _isDebugEnabled;
        private bool _isErrorEnabled;
        private bool _isFatalEnabled;
        private bool _isInfoEnabled;
        private bool _isWarnEnabled;
        private ILogger logger;

        public Logger(ILogger logger)
        {
            this.logger = logger;
        } 

        public void Debug(object message)
        {
            logger.Debug("{message}", message);
        }

        public void Debug(object message, Exception exception)
        {
            logger.Debug(exception, "{message}", message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            Debug(String.Format(format, args));
        }

        public void DebugFormat(string format, Exception exception, params object[] args)
        {
            Debug(String.Format(format, args), exception);
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Debug(String.Format(formatProvider, format, args));
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            Debug(String.Format(formatProvider, format, args), exception);
        }

        public void Info(object message)
        {
            logger.Information("{message}", message);
        }

        public void Info(object message, Exception exception)
        {
            logger.Information(exception, "{message}", message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Info(String.Format(format, args));
        }

        public void InfoFormat(string format, Exception exception, params object[] args)
        {
            Info(String.Format(format, args), exception);
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Info(String.Format(formatProvider, format, args));
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            Info(String.Format(formatProvider, format, args), exception);
        }

        public void Warn(object message)
        {
            logger.Warning("{message}", message);
        }

        public void Warn(object message, Exception exception)
        {
            logger.Warning(exception, "{message}", message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            Warn(String.Format(format, args));
        }

        public void WarnFormat(string format, Exception exception, params object[] args)
        {
            Warn(String.Format(format, args), exception);
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Warn(String.Format(formatProvider, format, args));
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            Warn(String.Format(formatProvider, format, args), exception);
        }

        public void Error(object message)
        {
            logger.Warning("{message}", message);
        }

        public void Error(object message, Exception exception)
        {
            logger.Warning(exception, "{message}", message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Error(String.Format(format, args));
        }

        public void ErrorFormat(string format, Exception exception, params object[] args)
        {
            Error(String.Format(format, args), exception);
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Error(String.Format(formatProvider, format, args));
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            Error(String.Format(formatProvider, format, args), exception);
        }

        public void Fatal(object message)
        {
            logger.Warning("{message}", message);
        }

        public void Fatal(object message, Exception exception)
        {
            logger.Warning(exception, "{message}", message);
        }

        public void FatalFormat(string format, params object[] args)
        {
            Error(String.Format(format, args));
        }

        public void FatalFormat(string format, Exception exception, params object[] args)
        {
            Error(String.Format(format, args), exception);
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Error(String.Format(formatProvider, format, args));
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            Error(String.Format(formatProvider, format, args), exception);
        }

        public bool IsDebugEnabled
        {
            get { return logger.IsEnabled(LogEventLevel.Debug); }
        }

        public bool IsErrorEnabled
        {
            get { return logger.IsEnabled(LogEventLevel.Error); }
        }

        public bool IsFatalEnabled
        {
            get { return logger.IsEnabled(LogEventLevel.Fatal); }
        }

        public bool IsInfoEnabled
        {
            get { return logger.IsEnabled(LogEventLevel.Information); }
        }

        public bool IsWarnEnabled
        {
            get { return logger.IsEnabled(LogEventLevel.Warning); }
        }
    }
}
