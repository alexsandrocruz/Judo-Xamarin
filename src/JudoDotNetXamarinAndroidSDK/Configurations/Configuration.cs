using JudoDotNetXamarin;
using JudoPayDotNet.Enums;

namespace JudoDotNetXamarinSDK.Configurations
{
    internal class Configuration : IConfiguration
    {
        private static volatile bool avsEnabled = false;
        public bool IsAVSEnabled { get { return avsEnabled; } set { avsEnabled = value; } }

        private static volatile bool maestroEnabled = false;
        public bool IsMaestroEnabled { get { return maestroEnabled; } set { maestroEnabled = value; } }

        private static volatile bool fraudMonitoringSignals = false;
        public bool IsFraudMonitoringSignals { get { return fraudMonitoringSignals; } set { fraudMonitoringSignals = value; } }

        private static volatile bool isSSLPinningEnabled = false;
        public bool IsSSLPinningEnabled { get { return isSSLPinningEnabled; } set { isSSLPinningEnabled = value; } }

        /// <summary>
        /// Sets the configuration to access judo servers
        /// </summary>
        /// <param name="apiToken">The apiToken of the merchant</param>
        /// <param name="apiSecret">The apiSecret of the merchant</param> 
        /// <param name="environment">The environment to use</param>
        public void SetApiTokenAndSecret(string apiToken, string apiSecret, JudoEnvironment environment)
        {
            var judoSDKManager = JudoSDKManager.Instance;
            judoSDKManager.SetEnvironment(environment);
            judoSDKManager.SetJudoClient(JudoPaymentsFactory.Create(judoSDKManager.Environment, apiToken, apiSecret));
        }
    }
}