using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using JudoPayDotNet.Models;
using System.IO;
using System.Text;
using HomeKit;

#if __UNIFIED__
using Foundation;
using UIKit;
using CoreFoundation;
using CoreGraphics;
// Mappings Unified CoreGraphic classes to MonoTouch classes
using RectangleF = global::CoreGraphics.CGRect;
using SizeF = global::CoreGraphics.CGSize;
using PointF = global::CoreGraphics.CGPoint;
#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreGraphics;
// Mappings Unified types to MonoTouch types
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

namespace JudoDotNetXamariniOSSDK
{
	[Register("SecureWebView")]
	internal partial class SecureWebView :UIWebView
	{
		
		public SecureWebView(IntPtr p) : base(p)
		{
			this.LoadFinished+= delegate {
				this.ScrollView.SetZoomScale(2.0f,true);
			};
			this.ShouldStartLoad = (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType) => {
				
				if(request.Url.ToString().Contains("threedsecurecallback") && ReceiptID !=null)
				{
					Dictionary<string,string> queryStringDictionary = new Dictionary<string,string>();

					var TrackTraceDataArray = request.Body.ToString().Split (new char[] { '&' });

					foreach (string keyValuePair in TrackTraceDataArray)
					{
						var pairComponents = keyValuePair.Split (new char[] { '=' });
						string key =pairComponents.First();
						string value =pairComponents.Last();
						queryStringDictionary.Add(key,value);
					}

					_paymentService.CompleteDSecure (ReceiptID,queryStringDictionary["PaRes"],queryStringDictionary["MD"]).ContinueWith (reponse => {
						var result = reponse.Result;
						if (result != null && !result.HasError && result.Response.Result != "Declined") {
							var paymentreceipt = result.Response as PaymentReceiptModel;

							if (paymentreceipt != null) {
								// call success callback
								if (_successCallback != null)

									_successCallback (paymentreceipt);
								
							} else {
								throw new Exception ("JudoXamarinSDK: unable to find the receipt in response.");
							}

						} else {
							// Failure callback
							if (_failureCallback != null) {
								var judoError = new JudoError { ApiError = result != null ? result.Error : null };
								var paymentreceipt = result != null ? result.Response as PaymentReceiptModel : null;

								if (paymentreceipt != null) {
									// send receipt even we got card declined

									_failureCallback (judoError, paymentreceipt);

								} else {

									_failureCallback (judoError);

								}
							}
						}
					});

				}

				return true;
		};
		}





		private IPaymentService _paymentService;
		public string ReceiptID;
		public SuccessCallback _successCallback {  get; set; }
		public FailureCallback _failureCallback {  get; set; }


		public void SetupWebView(IPaymentService paymentService,SuccessCallback successCallback,FailureCallback failureCallback)
		{
			_paymentService = paymentService;
			_successCallback = successCallback;
			_failureCallback = failureCallback;
		}

		public override void LoadRequest (NSUrlRequest r)
		{
			base.LoadRequest (r);
		}


	

	}
}

