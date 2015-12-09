using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JudoPayDotNet.Logging;
using Serilog;


namespace JudoDotNetXamarin.Logging
{
    public static class XamarinLoggerFactory
    {
        public static ILog Create(Type askingType)
        {
            //TODO: What to do with logs on a mobile app
            return new Logger(new LoggerConfiguration().CreateLogger());
        }
    }
}
