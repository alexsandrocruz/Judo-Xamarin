using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JudoDotNetXamarin;
using JudoPayDotNet;
using JudoPayDotNet.Authentication;
using JudoPayDotNet.Http;
using JudoPayDotNet.Enums;

namespace JudoDotNetXamarin
{
    public static class JudoPaymentsFactory
    {
        private static readonly string LIVE_URL = "https://GW1.judopay.com/";
        private static readonly string SANDBOX_URL = "https://GW1.judopay-sandbox.com/";


        private static readonly string API_VERSION = "4.1.0.0";
        private const string Apiversionheader = "api-version";

        private static JudoPayApi Create (Credentials credentials, JudoEnvironment environment)
        {
            string baseUrl = null;
            switch (environment) {
            case JudoEnvironment.Live:
                baseUrl = LIVE_URL;
                break;
            case JudoEnvironment.Sandbox:
                baseUrl = SANDBOX_URL;
                break;
            }

            var httpClient = new HttpClientWrapper (new AuthorizationHandler (credentials,
                                 XamarinLoggerFactory.Create (typeof(AuthorizationHandler))),
                                 new VersioningHandler (Apiversionheader, API_VERSION));
            var connection = new Connection (httpClient,
                                 XamarinLoggerFactory.Create,
                                 baseUrl);
            var client = new Client (connection);

            return new JudoPayApi (XamarinLoggerFactory.Create, client);
        }

        public static JudoPayApi Create (JudoEnvironment environment, string token, string secret)
        {
            return Create (new Credentials (token, secret), environment);
        }

        public static JudoPayApi Create (JudoEnvironment environment, string oauthAccessToken)
        {
            return Create (new Credentials (oauthAccessToken), environment);
        }
    }
}
