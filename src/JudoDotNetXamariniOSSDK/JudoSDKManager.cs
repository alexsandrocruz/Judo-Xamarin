using System;
using System.Collections.Generic;
using System.Diagnostics;
using JudoDotNetXamarin;
using JudoDotNetXamariniOSSDK;
using JudoDotNetXamariniOSSDK.Clients;
using JudoDotNetXamariniOSSDK.Factories;
using JudoDotNetXamariniOSSDK.Services;
using JudoDotNetXamariniOSSDK.ViewModels;
using JudoPayDotNet.Models;
using Newtonsoft.Json.Linq;
using UIKit;
using JudoShieldXamarin;

namespace JudoDotNetXamariniOSSDK
{

    public class JudoSDKManager : IJudoSDKManager
    {
        internal static readonly UIFont FIXED_WIDTH_FONT_SIZE_20 = UIFont.FromName ("Courier", 17.0f);

        /// <summary>
        /// Enable 3D security process
        /// </summary>
        public bool ThreeDSecureEnabled{ get; set; }

        /// <summary>
        /// Enable/Disable AVS check
        /// </summary>
        public bool AVSEnabled { get; set; }

        /// <summary>
        /// Enable/Disable Amex card support
        /// </summary>
        public bool AmExAccepted { get; set; }

        /// <summary>
        /// Enable/Disable Mestro card support
        /// </summary>
        public bool MaestroAccepted { get; set; }


        /// <summary>
        /// Enable/Disable risk signal to pass fruad monitoring device data
        /// default is true
        /// </summary>
        public bool RiskSignals{ get; set; }

        /// <summary>
        /// SSLPinningEnabled
        /// </summary>
        public bool SSLPinningEnabled { get; set; }

        public bool AllowRooted { get; set; }


       

        private static readonly Lazy<JudoSDKManager> _singleton = new Lazy<JudoSDKManager> (() => new JudoSDKManager ());

        public static JudoSDKManager Instance {
            get { return _singleton.Value; }
        }

        static JudoSDKManager ()
        {
            // setting up UI mode by default
            UIMode = true;
            Instance.RiskSignals = true;
        }

        internal static void SetUserAgent ()
        {
			
        }

        public static bool ShouldCheckUserAgent ()
        {
            return false;
        }

        private static readonly ServiceFactory serviceFactory = new ServiceFactory ();
        private static readonly AppleServiceFactory appleServiceFactory = new AppleServiceFactory ();
        private static readonly IPaymentService PaymentService = serviceFactory.GetPaymentService ();
        private static readonly IApplePayService ApplePaymentService = appleServiceFactory.GetApplePaymentService ();
        private  IApplePayMethods _applePayMethods = new ApplePayMethods (ApplePaymentService);
        private static IJudoSDKApi _judoSdkApi;

        private static bool _uiMode { get; set; }

        /// <summary>
        /// Enable UI Mode
        /// By default this property is set to True
        /// </summary>
        public static bool UIMode {
            get { return _uiMode; }
            set {
                if (value)
                    _judoSdkApi = new UIMethods (new ViewLocator (PaymentService));
                else
                    _judoSdkApi = new NonUIMethods (PaymentService);

                _uiMode = value;
            }
        }

        void RootCheck (JudoFailureCallback failure)
        {
            if (!AllowRooted && JudoShield.IsiOSRooted ()) {
                failure (new JudoError () {
                    Exception = new Exception ("Users Device is rooted and app is configured to block calls from rooted Device"),
                    ApiError = null
                });
            }
        }

        /// <summary>
        /// Process a card payment
        /// </summary>
        /// <param name="payment">PaymentViewModel to pass Amount and card detail</param>
        /// <param name="success">Callback for success transaction</param>
        /// <param name="failure">Callback for fail transaction</param>
        /// <param name="navigationController">Navigation controller from UI this can be Null for non-UI Mode API</param>
        public void Payment (PaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure)
        {
            RootCheck (failure);

            var innerModel = payment.Clone ();

            _judoSdkApi.Payment (innerModel, success, failure);

        }

        /// <summary>
        /// Process a pre-authorisation payment
        /// </summary>
        /// <param name="preAuthorisation">PaymentViewModel to pass Amount and card detail</param>
        /// <param name="success">Callback for success transaction</param>
        /// <param name="failure">Callback for fail transaction</param>
        /// <param name="navigationController">Navigation controller from UI this can be Null for non-UI Mode API</param>
        public  void PreAuth (PaymentViewModel preAuthorisation, JudoSuccessCallback success, JudoFailureCallback failure)
        {
            RootCheck (failure);

            var innerModel = preAuthorisation.Clone ();
            _judoSdkApi.PreAuth (innerModel, success, failure);
        }

        /// <summary>
        /// Process a card payment
        /// </summary>
        /// <param name="payment">TokenPaymentViewModel to pass Amount and Token detail</param>
        /// <param name="success">Callback for success transaction</param>
        /// <param name="failure">Callback for fail transaction</param>
        /// <param name="navigationController">Navigation controller from UI this can be Null for non-UI Mode API</param>
        public void TokenPayment (TokenPaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure)
        {
            RootCheck (failure);

            var innerModel = payment.Clone ();
            _judoSdkApi.TokenPayment (innerModel, success, failure);

        }

        /// <summary>
        /// Process a token authorisation payment
        /// </summary>
        /// <param name="payment">TokenPaymentViewModel to pass Amount and Token detail</param>
        /// <param name="success">Callback for success transaction</param>
        /// <param name="failure">Callback for fail transaction</param>
        /// <param name="navigationController">Navigation controller from UI this can be Null for non-UI Mode API</param>
        public void TokenPreAuth (TokenPaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure)
        {
            RootCheck (failure);

            var innerModel = payment.Clone ();
            _judoSdkApi.TokenPreAuth (innerModel, success, failure);

        }

        /// <summary>
        /// Process a RegisterCard
        /// </summary>
        /// <param name="registerCard">TokenPaymentViewModel to pass Amount and Token detail</param>
        /// <param name="success">Callback for success transaction</param>
        /// <param name="failure">Callback for fail transaction</param>
        /// <param name="navigationController">Navigation controller from UI this can be Null for non-UI Mode API</param>
        public void RegisterCard (PaymentViewModel registerCard, JudoSuccessCallback success, JudoFailureCallback failure)
        {
            if (registerCard.Amount == 0) {
                registerCard.Amount = 1.01m;
            }

            RootCheck (failure);

            var innerModel = registerCard.Clone ();

            _judoSdkApi.RegisterCard (innerModel, success, failure);

        }

        public void MakeApplePayment (ApplePayViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure)
        {
            RootCheck (failure);

            _applePayMethods.ApplePayment (payment, success, failure, ApplePaymentType.Payment);
		
        }

        public void MakeApplePreAuth (ApplePayViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure)
        {
            RootCheck (failure);

            _applePayMethods.ApplePayment (payment, success, failure, ApplePaymentType.PreAuth);

        }

	
        UIViewController GetCurrentViewController ()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null) {
                vc = vc.PresentedViewController;
            }
            return vc;
        }
    }
}

