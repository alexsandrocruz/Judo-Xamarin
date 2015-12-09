using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Java.Lang;
using JudoDotNetXamarinSDK.Models;
using JudoDotNetXamarinSDK.Ui;
using JudoDotNetXamarinSDK.Utils;
using JudoPayDotNet.Models;
using Consumer = JudoDotNetXamarinSDK.Models.Consumer;
using Exception = System.Exception;

namespace JudoDotNetXamarinSDK.Activies
{
    [Activity(Label = "RegisterCardActivity")]
    public class RegisterCardActivity : BaseActivity
    {
        private Bundle judoMetaData;
        private CardEntryView cardEntryView;
        private Consumer judoConsumer;
        private EditText addressLine1;
        private EditText addressLine2;
        private EditText addressLine3;
        private EditText addressTown;
        private EditText addressPostCode;
        private AVSEntryView aVsEntryView;
        private StartDateIssueNumberEntryView startDateEntryView;

        private HelpButton cv2ExpiryHelpInfoButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.register_card);
            SetTitle(Resource.String.title_register_card);

            cardEntryView = FindViewById<CardEntryView>(Resource.Id.cardEntryView);
            TextView hintTextView = FindViewById<TextView>(Resource.Id.hintTextView);
            cardEntryView.HintTextView = hintTextView;
            aVsEntryView = FindViewById<AVSEntryView>(Resource.Id.avsEntryView);
            startDateEntryView = FindViewById<StartDateIssueNumberEntryView>(Resource.Id.startDateEntryView);

            cv2ExpiryHelpInfoButton = FindViewById<HelpButton>(Resource.Id.infoButtonID);

            addressLine1 = FindViewById<EditText>(Resource.Id.addressLine1);
            addressLine2 = FindViewById<EditText>(Resource.Id.addressLine2);
            addressLine3 = FindViewById<EditText>(Resource.Id.addressLine3);
            addressTown = FindViewById<EditText>(Resource.Id.addressTown);
            addressPostCode = FindViewById<EditText>(Resource.Id.addressPostCode);
            
            SetHelpText(Resource.String.help_info, Resource.String.help_card_text);
            SetHelpText(Resource.String.help_postcode_title, Resource.String.help_postcode_text, Resource.Id.postCodeHelpButton);

            judoConsumer = Intent.GetParcelableExtra(JudoSDKManager.JUDO_CONSUMER).JavaCast<Consumer>();

            if (judoConsumer == null)
            {
                throw new IllegalArgumentException("JUDO_CONSUMER must be supplied");
            }

            judoMetaData = Intent.GetBundleExtra(JudoSDKManager.JUDO_META_DATA);

            var payButton = FindViewById<Button>(Resource.Id.payButton);

            payButton.SetText(Resource.String.register_card);

            payButton.Click += (sender, args) => TransactClickHandler(RegisterCard);

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
                catch (InvalidDataException e)
                {
                    Log.Error(JudoSDKManager.DEBUG_TAG, e.StackTrace, e);
                }

                bool startDateFocus = false;
                if (ValidationHelper.IsStartDateRequiredForCardNumber(cardNumber) && JudoSDKManager.Configuration.IsMaestroEnabled)
                {
                    startDateEntryView.Visibility = ViewStates.Visible;
                    startDateEntryView.RequestFocus();
                    startDateFocus = true;
                    aVsEntryView.InhibitFocusOnFirstShowOfCountrySpinner();
                }

                if (JudoSDKManager.Configuration.IsAVSEnabled && aVsEntryView != null)
                {
                    aVsEntryView.Visibility = ViewStates.Visible;

                    if (!startDateFocus)
                    {
                        aVsEntryView.FocusPostCode();
                    }
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

        public void RegisterCard()
        {
            var registerCard = new RegisterCardModel()
            {
                CardAddress = new CardAddressModel()
                {
                    Line1 = addressLine1.Text,
                    Line2 = addressLine2.Text,
                    Line3 = addressLine3.Text,
                    Town = addressTown.Text,
                    PostCode = addressPostCode.Text
                },
                CardNumber = cardEntryView.GetCardNumber(),
                CV2 = cardEntryView.GetCardCV2(),
                ExpiryDate = cardEntryView.GetCardExpiry(),
                YourConsumerReference = judoConsumer.YourConsumerReference
            };
            
            ShowLoadingSpinner(true);

            JudoSDKManager.JudoClient.RegisterCards.Create(registerCard).ContinueWith(HandleServerResponse, TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void ShowLoadingSpinner(bool show)
        {
            RunOnUiThread(() =>
            {
                ((InputMethodManager)GetSystemService(Context.InputMethodService)).HideSoftInputFromWindow(
                    FindViewById(Resource.Id.payButton).WindowToken, 0);
                FindViewById(Resource.Id.loadingLayout).Visibility = show ? ViewStates.Visible : ViewStates.Gone;

            });
        }
    }
}