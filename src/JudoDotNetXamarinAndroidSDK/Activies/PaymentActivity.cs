using System;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using JudoDotNetXamarinAndroidSDK.Models;
using JudoDotNetXamarinAndroidSDK.Ui;
using JudoDotNetXamarinAndroidSDK.Utils;
using JudoDotNetXamarinSDK;
using JudoPayDotNet.Models;

namespace JudoDotNetXamarinAndroidSDK.Activies
{
    public class PaymentActivity : BaseActivity
    {
        protected string judoPaymentRef;
        protected decimal judoAmount;
        protected string judoId;
        protected string judoCurrency;
        protected MetaData judoMetaData;
        protected CardEntryView cardEntryView;
        protected Models.Consumer judoConsumer;
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

            judoPaymentRef = Intent.GetStringExtra(JudoSDKManagerA.JUDO_PAYMENT_REF);
            judoConsumer = Intent.GetParcelableExtra(JudoSDKManagerA.JUDO_CONSUMER).JavaCast<Models.Consumer>();

            judoAmount = decimal.Parse(Intent.GetStringExtra(JudoSDKManagerA.JUDO_AMOUNT));
            judoId = Intent.GetStringExtra(JudoSDKManagerA.JUDO_ID);
            judoCurrency = Intent.GetStringExtra(JudoSDKManagerA.JUDO_CURRENCY);

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

            judoMetaData = Intent.Extras.GetParcelable(JudoSDKManagerA.JUDO_META_DATA).JavaCast<MetaData>();

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

                if (ValidationHelper.IsStartDateRequiredForCardNumber(cardNumber) && JudoSDKManagerA.Configuration.IsMaestroEnabled)
                {
                    startDateEntryView.Visibility = ViewStates.Visible;
                    startDateEntryView.RequestFocus();
                    avsEntryView.InhibitFocusOnFirstShowOfCountrySpinner();
                }

                if (JudoSDKManagerA.Configuration.IsAVSEnabled && avsEntryView != null)
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
            SetResult(JudoSDKManagerA.JUDO_CANCELLED);
            base.OnBackPressed();
        }

        public virtual void MakeCardPayment()
        {
            var cardNumber = cardEntryView.GetCardNumber();
            var expiryDate = cardEntryView.GetCardExpiry();
            var cv2 = cardEntryView.GetCardCV2();

            CardAddressModel cardAddress = new CardAddressModel();

            if (JudoSDKManagerA.Configuration.IsAVSEnabled)
            {
                var country = avsEntryView.GetCountry();
                cardAddress.PostCode = avsEntryView.GetPostCode();
            }

            string startDate = null;
            string issueNumber = null;

            if (JudoSDKManagerA.Configuration.IsMaestroEnabled)
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
                ClientDetails = JudoSDKManagerA.GetClientDetails(this),
				UserAgent = JudoSDKManagerA.GetSDKVersion()
					
            };

            ShowLoadingSpinner(true);


            var judoPay = JudoSDKManagerA.JudoClient;

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