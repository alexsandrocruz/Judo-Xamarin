using Newtonsoft.Json.Linq;

namespace JudoDotNetXamarin
{
    public class BasePaymentViewModel
    {
        public JObject ClientDetails { get; set; }

        /// <summary>
        /// your JudoID, can be used to ovverride the value set within JudoConfiguration on a transactional bases
        /// JudoConfiguration MUST still be set as failover
        /// </summary>
        public string  JudoID { get; set; }
    }
}

