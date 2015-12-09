using System;
using JudoPayDotNet.Models;
using System.Threading.Tasks;
using UIKit;
using PassKit;
using Foundation;

namespace JudoDotNetXamariniOSSDK
{
	internal interface IApplePayService
	{
		void MakeApplePayment (ApplePayViewModel payment,SuccessCallback success, FailureCallback failure,UINavigationController controller, ApplePaymentType type);
		Task<IResult<ITransactionResult>> HandlePKPayment (PKPayment payment,string customerRef,NSDecimalNumber amount, ApplePaymentType type,FailureCallback failure);
	}
}

