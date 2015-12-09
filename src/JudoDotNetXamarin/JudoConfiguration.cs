using System;
using JudoPayDotNet.Models;
using JudoPayDotNet.Enums;

namespace JudoDotNetXamarin
{
    public class JudoConfiguration
    {
        private static readonly Lazy<JudoConfiguration> _singleton = new Lazy<JudoConfiguration> (() => new JudoConfiguration ());

        public static JudoConfiguration Instance {
            get { return _singleton.Value; }
        }

        /// <summary>
        /// Api Token must be set before calling API
        /// </summary>
        public  string ApiToken { get; set; }

        /// <summary>
        /// Api Secret must be set before calling API
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// JudoID must be set before calling API
        /// </summary>
        public string JudoId { get; set; }

        /// <summary>
        /// Environment SANDBOX or LIVE
        /// </summary>
        public JudoEnvironment Environment { get; set; }

        public void Validate ()
        {
            if (String.IsNullOrWhiteSpace (JudoId) || String.IsNullOrWhiteSpace (ApiToken) || String.IsNullOrWhiteSpace (ApiSecret)) {
              
                Exception e = new Exception ("SDK configuration variables invalid");
                   
                //e.InnerException = "SDK configuration variables invalid";

                   
                throw e;
             
            }
        }
    }
}

