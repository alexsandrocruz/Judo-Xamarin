using JudoDotNetXamarin;
using JudoDotNetXamariniOSSDK.Services;

namespace JudoDotNetXamariniOSSDK.Factories
{
    internal class AppleServiceFactory
    {
        public IApplePayService GetApplePaymentService ()
        {
            var judoApi = JudoPaymentsFactory.Create (JudoConfiguration.Instance.Environment, JudoConfiguration.Instance.ApiToken, JudoConfiguration.Instance.ApiSecret);
            return  new ApplePayService (judoApi);
        }
    }

}

