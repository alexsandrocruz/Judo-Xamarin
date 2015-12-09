using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using JudoDotNetXamarinSDK;
using JudoDotNetXamarinSDK.Models;
using JudoDotNetXamarinSDK.Utils;
using JudoPayDotNet.Models;
using Result = Android.App.Result;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace Android.Xamarin.SampleApp
{
    [Activity(Label = "@string/app_name_ui", MainLauncher = true, Icon = "@drawable/ic_app_icon")]
    public class WithUIActivity : Activity
    {
        // Configure your JudoID and payment detail
		private const string ApiToken   = "[Application ApiToken]";//retrieve from JudoPortal
		private const string ApiSecret  = "[Application ApiSecret]";//retrieve from JudoPortal
		private string MY_JUDO_ID       = "[Judo ID]"; //Received when registering an account with Judo
        private string currency         = "GBP";
        private string amount           = "4.99";
        private string paymentReference = "payment101010102";
        private string consumerRef      = "consumer1010102";

        private const int ACTION_CARD_PAYMENT   = 101;
        private const int ACTION_TOKEN_PAYMENT  = 102;
        private const int ACTION_PREAUTH        = 201;
        private const int ACTION_TOKEN_PREAUTH  = 202;
        private const int ACTION_REGISTER_CARD  = 301;

        private volatile string cardToken;
        private volatile string consumerToken;
        private volatile string rcp_consumerRef;
        private volatile string lastFour;
        private volatile CardBase.CardType cardType;

        private volatile string preAuth_cardToken;
        private volatile string preAuth_consumerToken;
        private volatile string preAuth_rcp_consumerRef;
        private volatile string preAuth_lastFour;
        private volatile CardBase.CardType preAuth_cardType;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.withui);

            // setting up API token/secret 
			JudoSDKManager.Configuration.SetApiTokenAndSecret(ApiToken,ApiSecret);
            JudoSDKManager.Configuration.IsAVSEnabled = true;
            JudoSDKManager.Configuration.IsFraudMonitoringSignals = true;
            JudoSDKManager.Configuration.IsMaestroEnabled = true;

            // Get our button from the layout resource,
            // and attach an event to it
            Button payCard          = FindViewById<Button>(Resource.Id.payCard);
            Button payToken         = FindViewById<Button>(Resource.Id.payToken);
            Button payPreAuth       = FindViewById<Button>(Resource.Id.payPreAuth);
            Button payTokenPreAuth  = FindViewById<Button>(Resource.Id.payTokenPreAuth);
            Button registerCard     = FindViewById<Button>(Resource.Id.registerCard);
            Button nonUiExamples    = FindViewById<Button>(Resource.Id.nonUiExamples);

            // Assigning click delegates
            payCard.Click           += new EventHandler(payCard_Click);
            payToken.Click          += new EventHandler(payToken_Click);
            payPreAuth.Click        += new EventHandler(payPreAuth_Click);
            payTokenPreAuth.Click   += new EventHandler(payTokenPreAuth_Click);
            registerCard.Click      += new EventHandler(registerCard_Click);
            nonUiExamples.Click     += new EventHandler(nonUiExamples_Click);

            FindViewById<TextView>(Resource.Id.sdk_version_label).Text = "";
        }

        private void payCard_Click(object sender, EventArgs e)
        {
            // Optional: Supply meta data about this transaction, pass as last argument instead of null.
            Dictionary<string, string> metaData = new Dictionary<string, string>{{"test1", "test2"}};
            
            var intent = JudoSDKManager.UIMethods.Payment(this, MY_JUDO_ID, currency, amount, paymentReference, consumerRef, metaData);

            StartActivityForResult(intent, ACTION_CARD_PAYMENT);
        }

        private void payPreAuth_Click(object sender, EventArgs e)
        {
            // Optional: Supply meta data about this transaction, pass as last argument instead of null.
            Intent intent = JudoSDKManager.UIMethods.PreAuth(this, MY_JUDO_ID, currency, amount, paymentReference, consumerRef, null);

            StartActivityForResult(intent, ACTION_PREAUTH);
        }

        private void payToken_Click(object sender, EventArgs e) 
        {
            if (string.IsNullOrWhiteSpace(consumerToken) || string.IsNullOrWhiteSpace(cardToken) 
                || string.IsNullOrWhiteSpace(rcp_consumerRef) || string.IsNullOrWhiteSpace(lastFour))
            {
                Toast.MakeText(this,
                    "Can't make a token payment before making a full card payment to save card token",
                    ToastLength.Short).Show();
                return;
            }

            var consumerReference = rcp_consumerRef;
            var token = new CardToken()
            {
                CardLastFour = lastFour,
                Token = cardToken,
                ConsumerToken = consumerToken,
                CardType = cardType
            };

            // Optional: Supply meta data about this transaction, pass as last argument instead of null.
            var intent = JudoSDKManager.UIMethods.TokenPayment(this, MY_JUDO_ID, currency, amount, paymentReference, consumerReference, token, null, consumerToken);

            StartActivityForResult(intent, ACTION_TOKEN_PAYMENT);

        }

        private void payTokenPreAuth_Click(object sender, EventArgs e) 
        {
            if (string.IsNullOrWhiteSpace(preAuth_consumerToken) || string.IsNullOrWhiteSpace(preAuth_cardToken)
                || string.IsNullOrWhiteSpace(preAuth_rcp_consumerRef) || string.IsNullOrWhiteSpace(preAuth_lastFour))
            {
                Toast.MakeText(this,
                    "Can't make a Preauth token payment before making a full preauth card payment to save card token",
                    ToastLength.Short).Show();
                return;
            }

            var consumerReference = preAuth_rcp_consumerRef;
            var token = new CardToken()
            {
                CardLastFour = preAuth_lastFour,
                Token = preAuth_cardToken,
                ConsumerToken = preAuth_consumerToken,
                CardType = preAuth_cardType
            };

            // Optional: Supply meta data about this transaction, pass as last argument instead of null.
            var intent = JudoSDKManager.UIMethods.TokenPreAuth(this, MY_JUDO_ID, currency, amount, paymentReference, consumerReference, token, null, preAuth_consumerToken);

            StartActivityForResult(intent, ACTION_TOKEN_PREAUTH);
        }

        private void registerCard_Click(object sender, EventArgs e)
        {
            var intent = JudoSDKManager.UIMethods.RegisterCard(this, consumerRef);

            StartActivityForResult(intent, ACTION_REGISTER_CARD);
        }

        private void nonUiExamples_Click(object sender, EventArgs e) 
        {
            StartActivity(typeof(WithoutUIActivity));
        }

        private void UpdateCount() 
        {
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            Receipt receipt = null;
            string msg_prefix = "";

            if (resultCode == Result.Canceled) 
            {
                Toast.MakeText(this, "Payment Canceled.", ToastLength.Long).Show();
                return;
            }
            else if (resultCode == JudoSDKManager.JUDO_ERROR)
            {
                Error err =  data.GetParcelableExtra(JudoSDKManager.JUDO_ERROR_EXCEPTION) as Error;

                Toast.MakeText(this, string.Format("Error: {0} {1}", data.GetStringExtra(JudoSDKManager.JUDO_ERROR_MESSAGE),
                    err != null && err.Exception != null ? "\r\nException: " + err.Exception.Message : ""), ToastLength.Long).Show();
                return;
            }

            if(data != null)
                receipt = data.GetParcelableExtra(JudoSDKManager.JUDO_RECEIPT) as Receipt;

            if (receipt == null)
            {
                Toast.MakeText(this, string.Format("Error: {0}", data.GetStringExtra(JudoSDKManager.JUDO_ERROR_MESSAGE)), ToastLength.Long).Show();
                return;
            }

            switch (requestCode)
            {
                case ACTION_CARD_PAYMENT:
                    if (resultCode == Result.Ok && receipt.Result != "Declined")
                    {
                        PaymentReceiptModel paymentReceipt;
                        if ((paymentReceipt = receipt.FullReceipt as PaymentReceiptModel) != null)
                        {
                            cardToken = paymentReceipt.CardDetails.CardToken;
                            consumerToken = paymentReceipt.Consumer.ConsumerToken;
                            rcp_consumerRef = paymentReceipt.Consumer.YourConsumerReference;
                            lastFour = paymentReceipt.CardDetails.CardLastfour;
                            cardType = (CardBase.CardType)paymentReceipt.CardDetails.CardType;
                        }
                        msg_prefix = "Payment succeeded";
                    }
                    else
                    {
                        msg_prefix = "Payment failed";
                    }
                    break;
                case ACTION_PREAUTH:
                    if (resultCode == Result.Ok && receipt.Result != "Declined")
                    {
                        PaymentReceiptModel paymentReceipt;
                        if ((paymentReceipt = receipt.FullReceipt as PaymentReceiptModel) != null)
                        {
                            preAuth_cardToken = paymentReceipt.CardDetails.CardToken;
                            preAuth_consumerToken = paymentReceipt.Consumer.ConsumerToken;
                            preAuth_rcp_consumerRef = paymentReceipt.Consumer.YourConsumerReference;
                            preAuth_lastFour = paymentReceipt.CardDetails.CardLastfour;
                            preAuth_cardType = (CardBase.CardType)paymentReceipt.CardDetails.CardType;
                        }

                        msg_prefix = "PreAuth card payment succeeded";
                    }
                    else
                    {
                        msg_prefix = "PreAuth card payment failed";
                    }
                    break;
                case ACTION_TOKEN_PAYMENT:
                    if (resultCode == Result.Ok && receipt.Result != "Declined")
                    {
                        msg_prefix = "Token payment succeeded";
                    }
                    else
                    {
                        msg_prefix = "Token payment failed";
                    }
                    break;
                case ACTION_TOKEN_PREAUTH:
                    if (resultCode == Result.Ok && receipt.Result != "Declined")
                    {
                        msg_prefix = "PreAuth Token payment succeeded";
                    }
                    else
                    {
                        msg_prefix = "PreAuth Token payment failed";
                    }
                    break;
                case ACTION_REGISTER_CARD:
                    if (resultCode == Result.Ok && receipt.Result != "Declined")
                    {
                        PaymentReceiptModel paymentReceipt;
                        if ((paymentReceipt = receipt.FullReceipt as PaymentReceiptModel) != null)
                        {
                            cardToken = paymentReceipt.CardDetails.CardToken;
                            consumerToken = paymentReceipt.Consumer.ConsumerToken;
                            rcp_consumerRef = paymentReceipt.Consumer.YourConsumerReference;
                            lastFour = paymentReceipt.CardDetails.CardLastfour;
                            cardType = (CardBase.CardType)paymentReceipt.CardDetails.CardType;
                        }

                        msg_prefix = "Register card succeeded";
                    }
                    else
                    {
                        msg_prefix = "Register card failed";
                    }
                    break;
            }

            if (receipt != null)
            {
                Toast.MakeText(this, string.Format("{0}: id: {1},\r\nMessage: {2},\r\nresult: {3}", msg_prefix, receipt.ReceiptId, receipt.Message, receipt.Result), ToastLength.Long).Show();
            }

        }
    }
}

