using JudoPayDotNet.Enums;

namespace JudoDotNetXamarinSDK.Configurations
{
    /// <summary>
    /// JudoPay Sdk configurations
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Address verification system is used.
        /// </summary>
        bool IsAVSEnabled { get;  set; }

        /// <summary>
        /// Maestro cards are supported.
        /// </summary>
        bool IsMaestroEnabled { get; set; }

        /// <summary>
        /// Client details are picked up to monitor fraud signals
        /// </summary>
        bool IsFraudMonitoringSignals { get; set; }

        /// <summary>
        /// Allows certificate pinning.
        /// </summary>
        bool IsSSLPinningEnabled { get; set; }

        /// <summary>
        /// Sets the API token, secret and environment to use while talking with JudoPay servers.
        /// </summary>
        /// <param name="apiToken">The API token.</param>
        /// <param name="apiSecret">The API secret.</param>
        /// <param name="environment">The environment.</param>
        void SetApiTokenAndSecret(string apiToken, string apiSecret, JudoEnvironment environment = JudoEnvironment.Sandbox);
    }
}