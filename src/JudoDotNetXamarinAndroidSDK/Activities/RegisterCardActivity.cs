using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using JudoDotNetXamarinAndroidSDK.Ui;
using JudoDotNetXamarinAndroidSDK.Utils;
using JudoPayDotNet.Models;
using JudoDotNetXamarin;
using System;
using Newtonsoft.Json;

namespace JudoDotNetXamarinAndroidSDK.Activities
{
    [Activity (Label = "RegisterCardActivity")]
    public class RegisterCardActivity : BaseActivity
    {
        private Bundle judoMetaData;
        private CardEntryView cardEntryView;

        private Consumer judoConsumer;

        protected string judoPaymentRef;
        protected decimal judoAmount;
        protected string judoId;
        protected string judoCurrency;
       

        private EditText addressLine1;
        private EditText addressLine2;
        private EditText addressLine3;
        private EditText addressTown;
        private EditText addressPostCode;
        private AVSEntryView avsEntryView;
        private StartDateIssueNumberEntryView startDateEntryView;

        private HelpButton cv2ExpiryHelpInfoButton;

        private ClientService clientService;
        IPaymentService _paymentService;
        ServiceFactory factory;

        Button payButton;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            SetContentView (Resource.Layout.register_card);


            SetResources ();

            UnbundleIntent ();

            WireUpButtons ();

            SetUpDelegates ();

           

            clientService = new ClientService ();
            factory = new ServiceFactory ();
            _paymentService = factory.GetPaymentService (); 

            if (bundle != null) {
                RestoreState (bundle);
            }
        }

        void SetUpDelegates ()
        {
            cardEntryView.OnCreditCardEntered += cardNumber => {
                cv2ExpiryHelpInfoButton.Visibility = ViewStates.Visible;
            };

            cardEntryView.OnExpireAndCV2Entered += (expiryDate, cv2) => {
                string cardNumber = null;
                try {
                    cardNumber = cardEntryView.GetCardNumber ();
                } catch (InvalidDataException e) {
                    Log.Error (JudoSDKManager.DEBUG_TAG, e.StackTrace, e);
                }

                bool startDateFocus = false;
                if (ValidationHelper.IsStartDateRequiredForCardNumber (cardNumber) && JudoSDKManager.MaestroAccepted) {
                    startDateEntryView.Visibility = ViewStates.Visible;
                    startDateEntryView.RequestFocus ();
                    startDateFocus = true;
                    avsEntryView.InhibitFocusOnFirstShowOfCountrySpinner ();
                }

                if (JudoSDKManager.AVSEnabled && avsEntryView != null) {
                    avsEntryView.Visibility = ViewStates.Visible;

                    if (!startDateFocus) {
                        avsEntryView.FocusPostCode ();
                    }
                }
                payButton.Enabled = true;
            };

            cardEntryView.OnReturnToCreditCardNumberEntry += () => {
                cv2ExpiryHelpInfoButton.Visibility = ViewStates.Gone;
            };

            cardEntryView.NoLongerComplete += () => {
                payButton.Enabled = false;
            };
        }

        void WireUpButtons ()
        {
            payButton = FindViewById<Button> (Resource.Id.payButton);

            payButton.SetText (Resource.String.register_card);

            payButton.Click += (sender, args) => TransactClickHandler (RegisterCard);

            payButton.Enabled = false;
        }

        void UnbundleIntent ()
        {
            judoPaymentRef = Intent.GetStringExtra (JudoSDKManager.JUDO_PAYMENT_REF);
            judoConsumer = JsonConvert.DeserializeObject<Consumer> (Intent.GetStringExtra (JudoSDKManager.JUDO_CONSUMER));
            judoAmount = decimal.Parse (Intent.GetStringExtra (JudoSDKManager.JUDO_AMOUNT));
            judoId = Intent.GetStringExtra (JudoSDKManager.JUDO_ID);
            judoCurrency = Intent.GetStringExtra (JudoSDKManager.JUDO_CURRENCY);

            if (judoPaymentRef == null) {
                throw new ArgumentException ("JUDO_PAYMENT_REF must be supplied");
            }
            if (judoConsumer == null) {
                throw new ArgumentException ("JUDO_CONSUMER must be supplied");
            }
            if (judoAmount == null) {
                throw new ArgumentException ("JUDO_AMOUNT must be supplied");
            } 
            if (judoId == null) {
                throw new ArgumentException ("JUDO_ID must be supplied");
            }
            if (judoCurrency == null) {
                throw new ArgumentException ("JUDO_CURRENCY must be supplied");
            }


            judoMetaData = Intent.GetBundleExtra (JudoSDKManager.JUDO_META_DATA);
        }

        void SetResources ()
        {
            SetTitle (Resource.String.title_register_card);
            cardEntryView = FindViewById<CardEntryView> (Resource.Id.cardEntryView);
            TextView hintTextView = FindViewById<TextView> (Resource.Id.hintTextView);
            cardEntryView.HintTextView = hintTextView;
            avsEntryView = FindViewById<AVSEntryView> (Resource.Id.avsEntryView);
            startDateEntryView = FindViewById<StartDateIssueNumberEntryView> (Resource.Id.startDateEntryView);

            cv2ExpiryHelpInfoButton = FindViewById<HelpButton> (Resource.Id.infoButtonID);

            addressLine1 = FindViewById<EditText> (Resource.Id.addressLine1);
            addressLine2 = FindViewById<EditText> (Resource.Id.addressLine2);
            addressLine3 = FindViewById<EditText> (Resource.Id.addressLine3);
            addressTown = FindViewById<EditText> (Resource.Id.addressTown);
            addressPostCode = FindViewById<EditText> (Resource.Id.addressPostCode);

            SetHelpText (Resource.String.help_info, Resource.String.help_card_text);
            SetHelpText (Resource.String.help_postcode_title, Resource.String.help_postcode_text, Resource.Id.postCodeHelpButton);
        }

        public override void OnBackPressed ()
        {
            SetResult (JudoSDKManager.JUDO_CANCELLED);
            base.OnBackPressed ();
        }

        public void RegisterCard ()
        {
            ShowLoadingSpinner (true);
            PaymentViewModel cardPayment = new PaymentViewModel ();
            CardViewModel card;
            UiUtils.GatherCardDetails (cardEntryView, avsEntryView, startDateEntryView, out card);
            cardPayment.Card = card;
            cardPayment.Currency = judoCurrency;
            cardPayment.JudoID = judoId;
            cardPayment.Amount = judoAmount;
            cardPayment.PaymentReference = judoPaymentRef;
            cardPayment.ConsumerReference = judoConsumer.YourConsumerReference;

            _paymentService.RegisterCard (cardPayment, clientService).ContinueWith (HandleServerResponse, TaskScheduler.FromCurrentSynchronizationContext ());

        }

        protected override void ShowLoadingSpinner (bool show)
        {
            RunOnUiThread (() => {
                ((InputMethodManager)GetSystemService (Context.InputMethodService)).HideSoftInputFromWindow (
                    FindViewById (Resource.Id.payButton).WindowToken, 0);
                FindViewById (Resource.Id.loadingLayout).Visibility = show ? ViewStates.Visible : ViewStates.Gone;

            });
        }

        protected override void OnSaveInstanceState (Bundle outState)
        {

            var cardNumber = cardEntryView.GetCardNumber (false);
            var expiryDate = cardEntryView.GetCardExpiry (false);
            var cv2 = cardEntryView.GetCardCV2 (false);
            var stage = cardEntryView.CurrentStage;
            outState.PutString ("CARDNUMBER", cardNumber);
            outState.PutString ("EXPIRYDATE", expiryDate);
            outState.PutString ("CV2", cv2);
            outState.PutInt ("STAGE", (int)stage);

            if (JudoSDKManager.AVSEnabled) {
                var country = avsEntryView.GetCountry ();
                var PostCode = avsEntryView.GetPostCode ();
                outState.PutInt ("COUNTRY", (Int32)country);
                outState.PutString ("POSTCODE", PostCode);
            }

            if (JudoSDKManager.MaestroAccepted) {
                string startDate = null;
                string issueNumber = null;
                issueNumber = startDateEntryView.GetIssueNumber ();
                startDate = startDateEntryView.GetStartDate ();
                outState.PutString ("ISSUENUMBER", issueNumber);
                outState.PutString ("STARTDATE", startDate);
            }

            // always call the base implementation!
            base.OnSaveInstanceState (outState);    
        }

        void RestoreState (Bundle bundle)
        {

            var cardNumber = bundle.GetString ("CARDNUMBER", "");
            var expiry = bundle.GetString ("EXPIRYDATE", "");
            var cv2 = bundle.GetString ("CV2", "");
            var stage = bundle.GetInt ("STAGE", (int)Stage.STAGE_CC_NO);
            cardEntryView.RestoreState (cardNumber, expiry, cv2, (Stage)stage);

            if (JudoSDKManager.AVSEnabled) {
                var country = bundle.GetInt ("COUNTRY", 0);
                var PostCode = bundle.GetString ("POSTCODE", "");
                avsEntryView.RestoreState (country, PostCode);
            }

            if (JudoSDKManager.MaestroAccepted) {
                string startDate = bundle.GetString ("STARTDATE", "");
                string issueNumber = bundle.GetString ("ISSUENUMBER", "");
                startDateEntryView.RestoreState (startDate, issueNumber);


            }

        }
    }
}