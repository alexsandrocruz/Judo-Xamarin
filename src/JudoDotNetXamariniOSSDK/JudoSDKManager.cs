using System;
using System.Collections.Generic;
using JudoDotNetXamariniOSSDK.Clients;
using JudoDotNetXamariniOSSDK.Utils;
using JudoPayDotNet.Models;
using Newtonsoft.Json.Linq;
using System.Drawing;
using PassKit;
using System.Diagnostics;


#if __UNIFIED__
using Foundation;
using UIKit;
using CoreFoundation;
using CoreAnimation;
using CoreGraphics;
using ObjCRuntime;
// Mappings Unified CoreGraphic classes to MonoTouch classes
using RectangleF = global::CoreGraphics.CGRect;
using SizeF = global::CoreGraphics.CGSize;
using PointF = global::CoreGraphics.CGPoint;
#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreGraphics;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreAnimation;
// Mappings Unified types to MonoTouch types
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

namespace JudoDotNetXamariniOSSDK
{
    /// <summary>
    /// Callback for success transaction, this should be known blocking call
    /// </summary>
    public delegate void SuccessCallback(PaymentReceiptModel receipt);

    /// <summary>
    /// Callback for fail transaction, this should be known blocking call
    /// </summary>
    public delegate void FailureCallback(JudoError error, PaymentReceiptModel receipt = null);
	public class JudoSDKManager
	{
		
		
		internal static readonly UIFont FIXED_WIDTH_FONT_SIZE_20 = UIFont.FromName ("Courier", 17.0f);

        /// <summary>
        /// Enable 3D security process
        /// </summary>
		public static bool ThreeDSecureEnabled{ get; set; }

        /// <summary>
        /// Enable/Disable AVS check
        /// </summary>
		public static bool AVSEnabled { get; set; }

        /// <summary>
        /// Enable/Disable Amex card support
        /// </summary>
		public static bool AmExAccepted { get; set; }

        /// <summary>
        /// Enable/Disable Mestro card support
        /// </summary>
		public static bool MaestroAccepted { get; set; }

		public static bool ApplePayAvailable{get{
				NSString[] paymentNetworks = new NSString[] {
					new NSString(@"Amex"),
					new NSString(@"MasterCard"),
					new NSString(@"Visa")
				};

				if (PKPaymentAuthorizationViewController.CanMakePayments && PKPaymentAuthorizationViewController.CanMakePaymentsUsingNetworks (paymentNetworks)) {
					return true;
				} else {

					return false;
				}		
			}}

        /// <summary>
        /// Enable/Disable risk signal to pass fruad monitoring device data
        /// default is true
        /// </summary>
		public static bool RiskSignals{ get; set; }
		
        /// <summary>
        /// SSLPinningEnabled
        /// </summary>
        public static bool SSLPinningEnabled { get; set; }

        // get the top most view 
		private static UIView appView = UIApplication.SharedApplication.Windows [0].RootViewController.View;

		private static readonly Lazy<JudoSDKManager> _singleton = new Lazy<JudoSDKManager> (() => new JudoSDKManager ());

        public static JudoSDKManager Instance
        {
            get { return _singleton.Value; }
        }

        static JudoSDKManager()
        {
            // setting up UI mode by default
            UIMode = true;
            RiskSignals = true;
        }


        internal static JObject GetClientDetails()
        {
            if(RiskSignals)
                return JObject.FromObject(ClientDetailsProvider.GetClientDetails());

            return null;
        }

		public static string GetSDKVersion ()
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
			string version = fvi.FileVersion;

			return "Xamarin-iOS-" + version;
		}

		internal static void SetUserAgent ()
		{
			
		}

		public static bool ShouldCheckUserAgent ()
		{
			return false;
		}

        private static IJudoSDKApi _judoSdkApi;
		private static readonly ServiceFactory ServiceFactory = new ServiceFactory ();
		private static readonly IPaymentService PaymentService = ServiceFactory.GetPaymentService ();
		private static readonly IApplePayService ApplePaymentService = ServiceFactory.GetApplePaymentService ();
		private static LoadingOverlay _loadPop;

		private static bool _uiMode { get; set; }

        /// <summary>
        /// Enable UI Mode
        /// By default this property is set to True
        /// </summary>
		public static bool UIMode {
			get { return _uiMode; }
			set {
			    if (value)
					_judoSdkApi = new UIMethods(ApplePaymentService,new ViewLocator(PaymentService));
			    else
					_judoSdkApi = new NonUIMethods(ApplePaymentService,PaymentService);

			    _uiMode = value;
			}
		}

        /// <summary>
        /// shows loading screen while processing payment
        /// </summary>
        /// <param name="view"></param>
		internal static void ShowLoading (UIView view)
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				_loadPop = new LoadingOverlay (new RectangleF ((view.Frame.Width / 2) - 75f, (view.Frame.Height / 2) - 75f, 150f, 150f), true);
			} else {
                view = UIApplication.SharedApplication.Windows[0].RootViewController.View;
                _loadPop = new LoadingOverlay();
				_loadPop.Frame = view.Frame;
			}
			view.Add (_loadPop);
		}

        /// <summary>
        /// hides loading screen while processing payment
        /// </summary>
		internal static void HideLoading ()
		{
			if (_loadPop != null)
            {
				_loadPop.Hide (appView);
                _loadPop.Dispose();
            }
		}

        /// <summary>
        /// Process a card payment
        /// </summary>
        /// <param name="payment">PaymentViewModel to pass Amount and card detail</param>
        /// <param name="success">Callback for success transaction</param>
        /// <param name="failure">Callback for fail transaction</param>
        /// <param name="navigationController">Navigation controller from UI this can be Null for non-UI Mode API</param>
		public static void Payment (PaymentViewModel payment, SuccessCallback success, FailureCallback failure, UINavigationController navigationController)
		{
			var innerModel = payment.Clone ();
			if (UIMode && navigationController == null) {
				var error = new JudoError { Exception = new Exception ("Navigation controller cannot be null with UIMode enabled.") };
				failure (error);
			} else {
				
				_judoSdkApi.Payment (innerModel, success, failure, navigationController);
			}
		}

        /// <summary>
        /// Process a pre-authorisation payment
        /// </summary>
        /// <param name="preAuthorisation">PaymentViewModel to pass Amount and card detail</param>
        /// <param name="success">Callback for success transaction</param>
        /// <param name="failure">Callback for fail transaction</param>
        /// <param name="navigationController">Navigation controller from UI this can be Null for non-UI Mode API</param>
		public static void PreAuth (PaymentViewModel preAuthorisation, SuccessCallback success, FailureCallback failure, UINavigationController navigationController)
		{
			var innerModel = preAuthorisation.Clone ();
			if (UIMode && navigationController == null) {
				var error = new JudoError { Exception = new Exception ("Navigation controller cannot be null with UIMode enabled.") };
				failure (error);
			} else {
				_judoSdkApi.PreAuth (innerModel, success, failure, navigationController);
			}
		}

        /// <summary>
        /// Process a card payment
        /// </summary>
        /// <param name="payment">TokenPaymentViewModel to pass Amount and Token detail</param>
        /// <param name="success">Callback for success transaction</param>
        /// <param name="failure">Callback for fail transaction</param>
        /// <param name="navigationController">Navigation controller from UI this can be Null for non-UI Mode API</param>
		public static void TokenPayment (TokenPaymentViewModel payment, SuccessCallback success, FailureCallback failure, UINavigationController navigationController)
		{
			var innerModel = payment.Clone ();
			if (UIMode && navigationController == null) {
				var error = new JudoError { Exception = new Exception ("Navigation controller cannot be null with UIMode enabled.") };
				failure (error);
			} else {
				_judoSdkApi.TokenPayment (innerModel, success, failure, navigationController);
			}
		}

        /// <summary>
        /// Process a token authorisation payment
        /// </summary>
        /// <param name="payment">TokenPaymentViewModel to pass Amount and Token detail</param>
        /// <param name="success">Callback for success transaction</param>
        /// <param name="failure">Callback for fail transaction</param>
        /// <param name="navigationController">Navigation controller from UI this can be Null for non-UI Mode API</param>
		public static void TokenPreAuth (TokenPaymentViewModel payment, SuccessCallback success, FailureCallback failure, UINavigationController navigationController)
		{
			var innerModel = payment.Clone ();
			if (UIMode && navigationController == null) {
				var error = new JudoError { Exception = new Exception ("Navigation controller cannot be null with UIMode enabled.") };
				failure (error);
			} else {
				_judoSdkApi.TokenPreAuth (innerModel, success, failure, navigationController);
			}
		}

        /// <summary>
        /// Process a RegisterCard
        /// </summary>
        /// <param name="registerCard">TokenPaymentViewModel to pass Amount and Token detail</param>
        /// <param name="success">Callback for success transaction</param>
        /// <param name="failure">Callback for fail transaction</param>
        /// <param name="navigationController">Navigation controller from UI this can be Null for non-UI Mode API</param>
		public static void RegisterCard (PaymentViewModel registerCard, SuccessCallback success, FailureCallback failure, UINavigationController navigationController)
		{
			var innerModel = registerCard.Clone ();
			if (UIMode && navigationController == null) {
				var error = new JudoError { Exception = new Exception ("Navigation controller cannot be null with UIMode enabled.") };
				failure (error);
			} else {
                _judoSdkApi.RegisterCard(innerModel, success, failure, navigationController);
			}
		}

		public static void MakeApplePayment (ApplePayViewModel payment, SuccessCallback success, FailureCallback failure, UINavigationController navigationController)
		{
			_judoSdkApi.ApplePayment(payment,success,failure,navigationController,ApplePaymentType.Payment);
		}

		public static void MakeApplePreAuth (ApplePayViewModel payment, SuccessCallback success, FailureCallback failure, UINavigationController navigationController)
		{

			_judoSdkApi.ApplePayment(payment,success,failure,navigationController,ApplePaymentType.PreAuth);

		}

		internal static void SummonThreeDSecure (PaymentRequiresThreeDSecureModel threedDSecureReceipt, SecureWebView secureWebView)
		{
			secureWebView.ReceiptID =	threedDSecureReceipt.ReceiptId;

			NSCharacterSet allowedCharecterSet = NSCharacterSet.FromString (@":/=,!$&'()*+;[]@#?").InvertedSet;
			NSString paReq = new NSString (threedDSecureReceipt.PaReq);
			var encodedPaReq = paReq.CreateStringByAddingPercentEncoding (allowedCharecterSet);

			NSString termUrl = new NSString ("judo1234567890://threedsecurecallback");
			var encodedTermUrl = termUrl.CreateStringByAddingPercentEncoding (allowedCharecterSet);


			NSUrl url = new NSUrl (threedDSecureReceipt.AcsUrl);

			NSMutableUrlRequest req = new NSMutableUrlRequest (url);

			NSString postString = new NSString ("MD=" + threedDSecureReceipt.Md + "&PaReq=" + encodedPaReq + "&TermUrl=" + encodedTermUrl + "");
			NSData postData = postString.Encode (NSStringEncoding.UTF8);

			req.HttpMethod = "POST";
			req.Body = postData;

			try {
				DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
					secureWebView.LoadRequest (req);

					JudoSDKManager.HideLoading ();
					secureWebView.Hidden = false;
				});
			} catch (Exception e) {
				if (secureWebView._failureCallback != null) {
					var judoError = new JudoError { Exception = e };
					secureWebView._failureCallback (judoError);
				}
			}
		}
    }
}

