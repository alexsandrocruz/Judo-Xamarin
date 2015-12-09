using System;
using JudoPayDotNet.Models;
using UIKit;

namespace JudoDotNetXamariniOSSDK
{
	internal abstract class ApplePayMethods
	{
		private readonly IApplePayService _paymentService;

		public ApplePayMethods (IApplePayService paymentService)
		{
			_paymentService = paymentService;
		}

		public void ApplePayment (ApplePayViewModel viewModel, SuccessCallback success, FailureCallback failure, UINavigationController navigationController, ApplePaymentType type)
		{
			if (!JudoSDKManager.ApplePayAvailable) {
				failure (new JudoError { ApiError = new JudoPayDotNet.Errors.JudoApiErrorModel {
						ErrorMessage = "Apple Pay is not enabled on device, application entitlement or setup by user.",
						ErrorType = JudoApiError.General_Error,
						ModelErrors = null
					}
				});
			}
			try {
				_paymentService.MakeApplePayment (viewModel, success, failure, navigationController, type);
			} catch (Exception ex) {
				HandleFailure (failure, ex);
			}
		}

		private void HandleFailure (FailureCallback failure, Exception ex)
		{
			if (failure != null) {
				var judoError = new JudoError {
					Exception = ex
				};
				failure (judoError);
			}
		}
	}
}

