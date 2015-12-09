using JudoDotNetXamarin;
using JudoDotNetXamariniOSSDK.ViewModels;

namespace JudoDotNetXamariniOSSDK.Clients
{
    public interface IApplePayMethods
    {
        void ApplePayment (ApplePayViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure, ApplePaymentType type);
    }
}

