using System;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using JudoDotNetXamarinAndroidSDK.Ui;
using JudoDotNetXamarinAndroidSDK.Utils;
using JudoPayDotNet.Models;
using JudoDotNetXamarin;
using Android.App;
using Newtonsoft.Json;

namespace JudoDotNetXamarinAndroidSDK.Activities
{
    [Activity]
    public class PaymentActivity : BaseActivity
    {
        protected string judoPaymentRef;
        protected decimal judoAmount;
        protected string judoId;
        protected string judoCurrency;
        protected CardEntryView cardEntryView;
        protected Consumer judoConsumer;
        protected AVSEntryView avsEntryView;
        protected HelpButton cv2ExpiryHelpInfoButton;
        protected StartDateIssueNumberEntryView startDateEntryView;
        IPaymentService _paymentService;
        ServiceFactory factory;

        Button payButton;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            SetResources ();

            UnbundleIntent ();

            WireUpButtons ();

            SetUpDelegates ();

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
                } catch (Exception e) {
                    Console.Error.Write (e.StackTrace);
                }
                if (ValidationHelper.IsStartDateRequiredForCardNumber (cardNumber) && JudoSDKManager.MaestroAccepted) {
                    startDateEntryView.Visibility = ViewStates.Visible;
                    startDateEntryView.RequestFocus ();
                    avsEntryView.InhibitFocusOnFirstShowOfCountrySpinner ();
                }
                if (JudoSDKManager.AVSEnabled && avsEntryView != null) {
                    avsEntryView.Visibility = ViewStates.Visible;
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

            payButton.Text = Resources.GetString (Resource.String.payment);

            payButton.Click += (sender, args) => TransactClickHandler (MakeCardPayment);
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
        }

        void SetResources ()
        {
            SetContentView (Resource.Layout.payment);
            Title = GetString (Resource.String.title_payment);
            cardEntryView = FindViewById<CardEntryView> (Resource.Id.cardEntryView);
            TextView hintTextView = FindViewById<TextView> (Resource.Id.hintTextView);
            cardEntryView.HintTextView = hintTextView;
            avsEntryView = FindViewById<AVSEntryView> (Resource.Id.avsEntryView);
            startDateEntryView = FindViewById<StartDateIssueNumberEntryView> (Resource.Id.startDateEntryView);
            cv2ExpiryHelpInfoButton = FindViewById<HelpButton> (Resource.Id.infoButtonID);
            cv2ExpiryHelpInfoButton.Visibility = ViewStates.Gone;
            SetHelpText (Resource.String.help_info, Resource.String.help_card_text);
            SetHelpText (Resource.String.help_postcode_title, Resource.String.help_postcode_text, Resource.Id.postCodeHelpButton);
        }


        public override void OnBackPressed ()
        {
            SetResult (JudoSDKManager.JUDO_CANCELLED);
            base.OnBackPressed ();
        }

        public virtual void MakeCardPayment ()
        {
            PaymentViewModel cardPayment = new PaymentViewModel ();
            cardPayment.JudoID = judoId;
            CardViewModel card;
            UiUtils.GatherCardDetails (cardEntryView, avsEntryView, startDateEntryView, out card);
            cardPayment.Card = card;
            cardPayment.Currency = judoCurrency;
            cardPayment.Amount = judoAmount;
            cardPayment.PaymentReference = judoPaymentRef;
            cardPayment.ConsumerReference = judoConsumer.YourConsumerReference;

            ShowLoadingSpinner (true);

            _paymentService.MakePayment (cardPayment, new ClientService ()).ContinueWith (HandleServerResponse, TaskScheduler.FromCurrentSynchronizationContext ());

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
              
                //var country = avsEntryView.GetCountry ();
                //var PostCode = avsEntryView.GetPostCode ();
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