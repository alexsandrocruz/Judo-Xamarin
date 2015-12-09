using System;
using JudoPayDotNet.Models;
using System.Threading.Tasks;
using PassKit;
using UIKit;
using System.Net.Cache;
using JudoPayDotNet;
using System.Runtime.Remoting.Channels;
using Foundation;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using JudoPayDotNet.Errors;

namespace JudoDotNetXamariniOSSDK
{
	internal class ApplePayService :IApplePayService
	{
		private	JudoPayApi _judoAPI;

		public ApplePayService (JudoPayApi judoAPI)
		{
			_judoAPI = judoAPI;
		}

		public  void MakeApplePayment (ApplePayViewModel payment, SuccessCallback success, FailureCallback failure, UINavigationController controller, ApplePaymentType type)
		{
			try {
				PKPaymentRequest request = new PKPaymentRequest ();

				request.CurrencyCode = payment.CurrencyCode;

				request.CountryCode = payment.CountryCode;

				request.MerchantCapabilities = (PKMerchantCapability)payment.MerchantCapabilities;


				request.SupportedNetworks = payment.SupportedNetworks;


				request.PaymentSummaryItems = payment.Basket;

				request.MerchantIdentifier = payment.MerchantIdentifier;// @"merchant.com.judo.Xamarin"; // do it with configuration/overwrite

				var pkDelegate = new JudoPKPaymentAuthorizationViewControllerDelegate (this, request,payment.ConsumerRef.ToString(), type, success, failure);



				PKPaymentAuthorizationViewController pkController = new PKPaymentAuthorizationViewController (request){ Delegate = pkDelegate };
				controller.PresentViewController (pkController, true, null);

			} catch (Exception e) {
				Console.WriteLine (e.InnerException.ToString ());

				var judoError = new JudoError() {Exception = e };
				failure (judoError);
			}
		}



		public async Task<IResult<ITransactionResult>> HandlePKPayment (PKPayment payment,string customerRef, NSDecimalNumber amount, ApplePaymentType type,FailureCallback failure)
		{
			try {
				CardPaymentModel paymentmodel = new CardPaymentModel {
					JudoId = JudoConfiguration.Instance.JudoId,
					ClientDetails = JudoSDKManager.GetClientDetails (),
					UserAgent = JudoSDKManager.GetSDKVersion()
				};


				var test = payment.Token.PaymentData.ToString (NSStringEncoding.UTF8);
				JObject jo = JObject.Parse (test.ToString ());
				PKPaymentModel pkModel = new PKPaymentModel () {
					JudoId = JudoConfiguration.Instance.JudoId,
					YourPaymentReference = "paymentRef12343",
					YourConsumerReference = customerRef,
					Amount = amount.ToDecimal (),
					ClientDetails = JudoSDKManager.GetClientDetails (),
					UserAgent = JudoSDKManager.GetSDKVersion(),
					PkPayment = new PKPaymentInnerModel () {
						Token = new PKPaymentTokenModel () {
							PaymentData = jo,
							PaymentInstrumentName = payment.Token.PaymentInstrumentName,
							PaymentNetwork = payment.Token.PaymentNetwork
						}
					}
				};
				Task<IResult<ITransactionResult>> task = null;
				if (type == ApplePaymentType.Payment) {

					task = _judoAPI.Payments.Create (pkModel);
				} else if (type == ApplePaymentType.PreAuth) {
					task = _judoAPI.PreAuths.Create (pkModel);
				}
				if (task == null) {
					var judoError = new JudoError() {Exception = new Exception("Judo server did not return response. Please contact customer support") };
					failure (judoError);
				}
				return await task;
			} catch (Exception e) {
				Console.WriteLine (e.InnerException.ToString ());
				var judoError = new JudoError() {Exception = e };
				failure (judoError);
				return null;
			}
		}
			
	}
}

