using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using JudoDotNetXamarin;
using JudoDotNetXamarinSDK.Models;
using JudoDotNetXamarinSDK.Ui;
using JudoDotNetXamarinSDK.Utils;
using JudoPayDotNet.Models;
using Consumer = JudoDotNetXamarinSDK.Models.Consumer;

namespace JudoDotNetXamarinSDK.Activies
{
    public class PaymentActivity : BaseActivity
    {
        protected string judoPaymentRef;
        protected decimal judoAmount;
        protected string judoId;
        protected string judoCurrency;
        protected MetaData judoMetaData;
        protected CardEntryView cardEntryView;
        protected Consumer judoConsumer;
        protected AVSEntryView avsEntryView;
        protected HelpButton cv2ExpiryHelpInfoButton;
        protected StartDateIssueNumberEntryView startDateEntryView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.payment);
            Title = GetString(Resource.String.title_payment);
            cardEntryView = FindViewById<CardEntryView>(Resource.Id.cardEntryView);
            TextView hintTextView = FindViewById<TextView>(Resource.Id.hintTextView);
            cardEntryView.HintTextView = hintTextView;
            avsEntryView = FindViewById<AVSEntryView>(Resource.Id.avsEntryView);
            startDateEntryView = FindViewById<StartDateIssueNumberEntryView>(Resource.Id.startDateEntryView);

            cv2ExpiryHelpInfoButton = FindViewById<HelpButton>(Resource.Id.infoButtonID);
            cv2ExpiryHelpInfoButton.Visibility = ViewStates.Gone;

            SetHelpText(Resource.String.help_info, Resource.String.help_card_text);
            SetHelpText(Resource.String.help_postcode_title, Resource.String.help_postcode_text,
                Resource.Id.postCodeHelpButton);

            judoPaymentRef = Intent.GetStringExtra(JudoSDKManager.JUDO_PAYMENT_REF);
            judoConsumer = Intent.GetParcelableExtra(JudoSDKManager.JUDO_CONSUMER).JavaCast<Consumer>();

            judoAmount = decimal.Parse(Intent.GetStringExtra(JudoSDKManager.JUDO_AMOUNT));
            judoId = Intent.GetStringExtra(JudoSDKManager.JUDO_ID);
            judoCurrency = Intent.GetStringExtra(JudoSDKManager.JUDO_CURRENCY);

            if (judoPaymentRef == null)
            {
                throw new ArgumentException("JUDO_PAYMENT_REF must be supplied");
            }
            if (judoConsumer == null)
            {
                throw new ArgumentException("JUDO_CONSUMER must be supplied");
            }
            if (judoAmount == null)
            {
                throw new ArgumentException("JUDO_AMOUNT must be supplied");
            } 
            if (judoId == null)
            {
                throw new ArgumentException("JUDO_ID must be supplied");
            }
            if (judoCurrency == null)
            {
                throw new ArgumentException("JUDO_CURRENCY must be supplied");
            }

            judoMetaData = Intent.Extras.GetParcelable(JudoSDKManager.JUDO_META_DATA).JavaCast<MetaData>();

            var payButton = FindViewById<Button>(Resource.Id.payButton);

            payButton.Text = Resources.GetString(Resource.String.payment);

            payButton.Click += (sender, args) => TransactClickHandler(MakeCardPayment);

            cardEntryView.OnCreditCardEntered += cardNumber =>
            {
                cv2ExpiryHelpInfoButton.Visibility = ViewStates.Visible;
            };

            cardEntryView.OnExpireAndCV2Entered += (expiryDate, cv2) =>
            {
                string cardNumber = null;

                try
                {
                    cardNumber = cardEntryView.GetCardNumber();
                }
                catch (Exception e)
                {
                    Console.Error.Write(e.StackTrace);
                }

                if (ValidationHelper.IsStartDateRequiredForCardNumber(cardNumber) && JudoSDKManager.Configuration.IsMaestroEnabled)
                {
                    startDateEntryView.Visibility = ViewStates.Visible;
                    startDateEntryView.RequestFocus();
                    avsEntryView.InhibitFocusOnFirstShowOfCountrySpinner();
                }

                if (JudoSDKManager.Configuration.IsAVSEnabled && avsEntryView != null)
                {
                    avsEntryView.Visibility = ViewStates.Visible;
                }
            };

            cardEntryView.OnReturnToCreditCardNumberEntry += () =>
            {
                cv2ExpiryHelpInfoButton.Visibility = ViewStates.Gone;
            };
        }

        public override void OnBackPressed()
        {
            SetResult(JudoSDKManager.JUDO_CANCELLED);
            base.OnBackPressed();
        }

        public virtual void MakeCardPayment()
        {
            var cardNumber = cardEntryView.GetCardNumber();
            var expiryDate = cardEntryView.GetCardExpiry();
            var cv2 = cardEntryView.GetCardCV2();

            CardAddressModel cardAddress = new CardAddressModel();

            if (JudoSDKManager.Configuration.IsAVSEnabled)
            {
                var country = avsEntryView.GetCountry();
                cardAddress.PostCode = avsEntryView.GetPostCode();
            }

            string startDate = null;
            string issueNumber = null;

            if (JudoSDKManager.Configuration.IsMaestroEnabled)
            {
                issueNumber = startDateEntryView.GetIssueNumber();
                startDate = startDateEntryView.GetStartDate();
            }

            var cardPayment = new CardPaymentModel()
            {
                JudoId = judoId,
                Currency = judoCurrency,
                Amount = judoAmount,
                YourPaymentReference = judoPaymentRef,
                YourConsumerReference = judoConsumer.YourConsumerReference,
                YourPaymentMetaData = judoMetaData.Metadata,
                CardNumber = cardNumber,
                CardAddress = cardAddress,
                StartDate = startDate,
                ExpiryDate = expiryDate,
                CV2 = cv2,
                ClientDetails = JudoSDKManager.GetClientDetails(this),
				UserAgent = JudoSDKManager.GetSDKVersion()
					
            };

            ShowLoadingSpinner(true);


            var judoPay = JudoSDKManager.JudoClient;

            judoPay.Payments.Create(cardPayment).ContinueWith(HandleServerResponse, TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void ShowLoadingSpinner(bool show)
        {
            RunOnUiThread(() =>
            {
                ((InputMethodManager) GetSystemService(Context.InputMethodService)).HideSoftInputFromWindow(
                    FindViewById(Resource.Id.payButton).WindowToken, 0);
                FindViewById(Resource.Id.loadingLayout).Visibility = show ? ViewStates.Visible : ViewStates.Gone;

            });
        }
    }
}