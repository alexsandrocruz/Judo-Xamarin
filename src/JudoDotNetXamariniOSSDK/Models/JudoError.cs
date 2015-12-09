using System;
using JudoPayDotNet.Errors;

namespace JudoDotNetXamariniOSSDK
{
    public class JudoError
    {
        public Exception Exception { get; set; }
        public JudoApiErrorModel ApiError { get; set; } 
    }
}