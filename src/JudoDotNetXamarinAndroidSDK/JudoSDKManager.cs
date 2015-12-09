using System;
using System.Collections.Generic;
using JudoPayDotNet.Models;
using Newtonsoft.Json.Linq;
using System.Drawing;

using System.Diagnostics;
using JudoDotNetXamarinAndroidSDK;
using JudoDotNetXamarin;
using Android.App;
using Android.Content;
using JudoPayDotNet.Errors;
using JudoDotNetXamarinAndroidSDK.Utils;
using Newtonsoft.Json;


namespace JudoDotNetXamarinAndroidSDK
{

    public class JudoSDKManager
    {

        public static readonly Android.App.Result JUDO_SUCCESS = Android.App.Result.Ok;
        public static readonly Android.App.Result JUDO_CANCELLED = Android.App.Result.Canceled;
        public static readonly Android.App.Result JUDO_ERROR = Android.App.Result.FirstUser;


        public static string JUDO_PAYMENT_REF = "JudoPay-yourPaymentReference";
        public static string JUDO_AMOUNT = "JudoPay-amount";
        public static string JUDO_ID = "JudoPay-judoId";
        public static string JUDO_CURRENCY = "JudoPay-currency";
        public static string JUDO_META_DATA = "JudoPay-yourPaymentMetaData";
        public static string REQUEST_CODE = "requestCode";

        public static string JUDO_RECEIPT = "JudoPay-receipt";

        public static string JUDO_CARD_DETAILS = "JudoPay-CardToken";
        public static string JUDO_CONSUMER = "JudoPay-consumer";

        public static string JUDO_ERROR_MESSAGE = "ERROR_MESSAGE";
        public static string JUDO_ERROR_EXCEPTION = "ERROR_EXCEPTION";

        private static string REGULAR_CARD_FORMAT_HINT = "0000 0000 0000 0000";
        private static string AMEX_CARD_FORMAT_HINT = "0000 000000 00000";
        private static string REGULAR_EXPIRY_AND_VALIDATION_FORMAT_HINT = "MM/YY CV2";
        private static string AMEX_EXPIRY_AND_VALIDATION_FORMAT_HINT = "MM/YY CIDV";
        private static string REGULAR_EXPIRY_AND_VALIDATION_ERROR_MESSAGE = "Invalid CV2";
        private static string AMEX_EXPIRY_AND_VALIDATION_ERROR_MESSAGE = "Invalid CIDV";
            
        private static readonly Lazy<JudoSDKManager> _singleton = new Lazy<JudoSDKManager> (() => new JudoSDKManager ());

        public static JudoSDKManager Instance {
            get { return _singleton.Value; }
        }

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

        /// <summary>
        /// Enable/Disable risk signal to pass fruad monitoring device data
        /// default is true
        /// </summary>
        public static bool RiskSignals{ get; set; }

        /// <summary>
        /// SSLPinningEnabled
        /// </summary>
        public static bool SSLPinningEnabled { get; set; }

        public bool AllowRooted { get; set; }

        private bool isRooted;
        private RootCheck _rootCheck;

        public JudoSDKManager ()
        {

            _rootCheck = new RootCheck ();
            isRooted = _rootCheck.IsRooted ();
        }

        private static JudoAndroidSDKAPI _judoSdkApi;

        private static bool _uiMode { get; set; }

        /// <summary>
        /// Enable UI Mode
        /// By default this property is set to True
        /// </summary>
        public static bool UIMode {
            get { return _uiMode; }
            set {
                if (value) {
                    _judoSdkApi = new UIMethods ();

                } else {
                    _judoSdkApi = new NonUIMethods ();


                  
                }
                _uiMode = value;
            }
        }

        public void Payment (PaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure, Activity context)
        {
            EvaluateRootCheck (failure);
            var innerModel = payment.Clone ();         
            _judoSdkApi.Payment (innerModel, success, failure, context);

        }

        public void PreAuth (PaymentViewModel preAuthorisation, JudoSuccessCallback success, JudoFailureCallback failure, Activity context)
        {
            EvaluateRootCheck (failure);
            var innerModel = preAuthorisation.Clone ();
            _judoSdkApi.PreAuth (innerModel, success, failure, context);
          
        }

        public void TokenPayment (TokenPaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure, Activity context)
        {
            EvaluateRootCheck (failure);
            var innerModel = payment.Clone ();         
            _judoSdkApi.TokenPayment (innerModel, success, failure, context);
        }



        public void TokenPreAuth (TokenPaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure, Activity context)
        {
            EvaluateRootCheck (failure);
            var innerModel = payment.Clone ();         
            _judoSdkApi.TokenPreAuth (innerModel, success, failure, context);
        }

        public void RegisterCard (PaymentViewModel registerCard, JudoSuccessCallback success, JudoFailureCallback failure, Activity context)
        {
            if (registerCard.Amount == 0) {
                registerCard.Amount = 1.01m;
            }
            EvaluateRootCheck (failure);
            var innerModel = registerCard.Clone ();         
            _judoSdkApi.RegisterCard (innerModel, success, failure, context); 
        }

        internal static string DEBUG_TAG = "com.judopay.android";

        internal static string CardHintFormat (CardType cardType)
        {
            switch (cardType) {
            case CardType.AMEX:
                return AMEX_CARD_FORMAT_HINT;
            default:
                return REGULAR_CARD_FORMAT_HINT;
            }
        }

        internal static int GetCardResourceId (Context context, CardType cardType, bool showFront)
        {
            if (showFront) {
                switch (cardType) {
                case CardType.VISA:
                    return Resource.Drawable.ic_card_visa;
                case CardType.MASTERCARD:
                    return Resource.Drawable.ic_card_mastercard;
                case CardType.AMEX:
                    return Resource.Drawable.ic_card_amex;
                case CardType.MAESTRO:
                    return Resource.Drawable.ic_card_maestro;
                case CardType.UNKNOWN:
                default:
                    return Resource.Drawable.ic_card_unknown;
                }
            } else {
                switch (cardType) {
                case CardType.AMEX:
                    return Resource.Drawable.ic_card_cv2_amex;
                default:
                    return Resource.Drawable.ic_card_cv2;
                }
            }
        }

        internal static string GetCardHintFormat (CardType cardType)
        {
            switch (cardType) {
            case CardType.AMEX:
                return AMEX_CARD_FORMAT_HINT;
            default:
                return REGULAR_CARD_FORMAT_HINT;
            }
        }

        internal static string GetExpiryAndValidationHintFormat (CardType cardType)
        {
            switch (cardType) {
            case CardType.AMEX:
                return AMEX_EXPIRY_AND_VALIDATION_FORMAT_HINT;
            default:
                return REGULAR_EXPIRY_AND_VALIDATION_FORMAT_HINT;
            }
        }

        internal static string GetExpiryAndValidationErrorMessage (CardType cardType)
        {
            switch (cardType) {
            case CardType.AMEX:
                return AMEX_EXPIRY_AND_VALIDATION_ERROR_MESSAGE;
            default:
                return REGULAR_EXPIRY_AND_VALIDATION_ERROR_MESSAGE;
            }
        }

        internal static Intent CreateErrorIntent (string message, Exception exception, JudoApiErrorModel apiErrorModel)
        {
            Intent intent = new Intent ();
            intent.PutExtra (JUDO_ERROR_MESSAGE, message);
            intent.PutExtra (JUDO_ERROR_EXCEPTION, JsonConvert.SerializeObject (new JudoError (exception, apiErrorModel)));

            return intent;
        }

        void EvaluateRootCheck (JudoFailureCallback failure)
        {
            if (!AllowRooted && isRooted) {
                failure (new JudoError () {
                    Exception = new Exception ("Users Device is rooted and app is configured to block calls from rooted Device"),
                    ApiError = null
                });
            }
        }
            
    }


}