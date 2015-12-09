using System.Threading.Tasks;
using Foundation;
using JudoDotNetXamarin;
using JudoDotNetXamariniOSSDK.ViewModels;
using JudoPayDotNet.Models;
using PassKit;
using UIKit;

namespace JudoDotNetXamariniOSSDK.Services
{
    internal interface IApplePayService
    {
        void MakeApplePayment (ApplePayViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure, UINavigationController controller, ApplePaymentType type);

        Task<IResult<ITransactionResult>> HandlePKPayment (PKPayment payment, string customerRef, NSDecimalNumber amount, ApplePaymentType type, JudoFailureCallback failure);
    }
}

