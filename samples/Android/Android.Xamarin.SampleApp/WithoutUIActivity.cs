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
using System.Threading.Tasks;
using Android.Views.InputMethods;

namespace Android.Xamarin.SampleApp
{
    [Activity(Label = "@string/app_name")]
    public class WithoutUIActivity : Activity
    {
        // Configure your JudoID and payment detail
		private const string ApiToken   = "[Application ApiToken]";//retrieve from JudoPortal
		private const string ApiSecret  = "[Application ApiSecret]";//retrieve from JudoPortal
		private string MY_JUDO_ID       = "[Judo ID]"; //Received when registering an account with Judo
        private string currency         = "GBP";
        private decimal amount          = 4.99M;
        private string paymentReference = "payment101010102";
        private string consumerRef      = "consumer1010102";
        private string cardNumber       = "4976000000003436";
        private string addressPostCode  = "TR14 8PA";
        private string startDate        = "";
        private string expiryDate       = "12/15";
        private string cv2              = "452";
        private volatile string ResponseText = "";

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

        private TextView MsgText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "withoutui" layout resource
            SetContentView(Resource.Layout.withoutui);

            // setting up API token/secret 
            JudoSDKManager.Configuration.SetApiTokenAndSecret(ApiToken, ApiSecret);
            JudoSDKManager.Configuration.IsAVSEnabled = true;
            JudoSDKManager.Configuration.IsFraudMonitoringSignals = true;
            JudoSDKManager.Configuration.IsMaestroEnabled = true;

            // Get our button from the layout resource,
            // and attach an event to it
            Button payCard          = FindViewById<Button>(Resource.Id.payCard);
            Button threeDsecure     = FindViewById<Button>(Resource.Id.threeDsecure);
            Button payToken         = FindViewById<Button>(Resource.Id.payToken);
            Button payPreAuth       = FindViewById<Button>(Resource.Id.payPreAuth);
            Button payTokenPreAuth  = FindViewById<Button>(Resource.Id.payTokenPreAuth);
            Button registerCard     = FindViewById<Button>(Resource.Id.registerCard);
            Button getTransactions  = FindViewById<Button>(Resource.Id.getTransactions);

            // Assigning click delegates
            payCard.Click           += new EventHandler(payCard_Click);
            threeDsecure.Click      += new EventHandler(threeDsecure_Click);
            payToken.Click          += new EventHandler(payToken_Click);
            payPreAuth.Click        += new EventHandler(payPreAuth_Click);
            payTokenPreAuth.Click   += new EventHandler(payTokenPreAuth_Click);
            registerCard.Click      += new EventHandler(registerCard_Click);
            getTransactions.Click   += new EventHandler(getTransactions_Click);

            MsgText = FindViewById<TextView>(Resource.Id.MsgText);
            MsgText.Text = "";
        }

        private void ShowLoadingSpinner(bool show, int id)
        {
            RunOnUiThread(() =>
            {
                ((InputMethodManager)GetSystemService(Context.InputMethodService)).HideSoftInputFromWindow(
                    FindViewById(id).WindowToken, 0);
                FindViewById(Resource.Id.loadingLayout).Visibility = show ? ViewStates.Visible : ViewStates.Gone;

            });
        }

        private async void dealWithTask(Task<IResult<ITransactionResult>> task, int id, int requestCode)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                if (task.IsFaulted || result == null) 
                {
                    ResponseText = task.Exception.ToString();
                }
                else if (result.HasError)
                {
                    dealWithResult(requestCode, Result.FirstUser, result);
                }
                else 
                {
                    dealWithResult(requestCode, Result.Ok, result);
                }
            }
            catch (Exception ex)
            {
                ResponseText = ex.Message;
            }
            finally
            {
                RunOnUiThread(() => MsgText.Text = ResponseText);
                ShowLoadingSpinner(false, id);
            }
        }

#region Button_Click_Events

        private void payCard_Click(object sender, EventArgs e)
        {
            MsgText.Text = "";
            // Optional: Supply meta data about this transaction, pass as last argument instead of null.
            Dictionary<string, string> metaData = new Dictionary<string, string>{{"test1", "test2"}};

            ShowLoadingSpinner(true, Resource.Id.payCard);
            var paymentTask = JudoSDKManager.NonUIMethods.Payment(this, MY_JUDO_ID, currency, amount, paymentReference,
                                                                consumerRef, metaData, cardNumber, addressPostCode, startDate, expiryDate, cv2);
            dealWithTask(paymentTask, Resource.Id.payCard, ACTION_CARD_PAYMENT);
        }

        private void threeDsecure_Click(object sender, EventArgs e)
        {
            MsgText.Text = "";
        }

        private void payToken_Click(object sender, EventArgs e)
        {
            MsgText.Text = "";

            if (string.IsNullOrWhiteSpace(consumerToken) || string.IsNullOrWhiteSpace(cardToken)
                || string.IsNullOrWhiteSpace(rcp_consumerRef) || string.IsNullOrWhiteSpace(lastFour))
            {
                Toast.MakeText(this,
                    "Can't make a token payment before making a full card payment to save card token",
                    ToastLength.Short).Show();
                return;
            }

            // Optional: Supply meta data about this transaction, pass as last argument instead of null.
            Dictionary<string, string> metaData = new Dictionary<string, string> { { "test1", "test2" } };

            ShowLoadingSpinner(true, Resource.Id.payCard);
            var paymentTask = JudoSDKManager.NonUIMethods.TokenPayment(this, MY_JUDO_ID, currency, amount, paymentReference,
                                                                consumerToken, consumerRef, metaData, cardToken, cv2);
            dealWithTask(paymentTask, Resource.Id.payToken, ACTION_TOKEN_PAYMENT);
        }

        private void payPreAuth_Click(object sender, EventArgs e)
        {
            MsgText.Text = "";

            ShowLoadingSpinner(true, Resource.Id.payCard);
            var paymentTask = JudoSDKManager.NonUIMethods.PreAuth(this, MY_JUDO_ID, currency, amount, paymentReference,
                                                                consumerRef, null, cardNumber, addressPostCode, startDate, expiryDate, cv2);
            dealWithTask(paymentTask, Resource.Id.payPreAuth, ACTION_PREAUTH);
        }

        private void payTokenPreAuth_Click(object sender, EventArgs e)
        {
            MsgText.Text = "";

            if (string.IsNullOrWhiteSpace(preAuth_consumerToken) || string.IsNullOrWhiteSpace(preAuth_cardToken)
                || string.IsNullOrWhiteSpace(preAuth_rcp_consumerRef) || string.IsNullOrWhiteSpace(preAuth_lastFour))
            {
                Toast.MakeText(this,
                    "Can't make a Preauth token payment before making a full preauth card payment to save card token",
                    ToastLength.Short).Show();
                return;
            }

            ShowLoadingSpinner(true, Resource.Id.payCard);
            var paymentTask = JudoSDKManager.NonUIMethods.TokenPreAuth(this, MY_JUDO_ID, currency, amount, paymentReference,
                                                                preAuth_consumerToken, consumerRef, null, preAuth_cardToken, cv2);
            dealWithTask(paymentTask, Resource.Id.payTokenPreAuth, ACTION_TOKEN_PREAUTH);
        }

        private void registerCard_Click(object sender, EventArgs e)
        {
            MsgText.Text = "";

            ShowLoadingSpinner(true, Resource.Id.payCard);
            var registerCardTask = JudoSDKManager.NonUIMethods.RegisterCard(cardNumber, cv2, expiryDate, consumerRef, addressPostCode);
            dealWithTask(registerCardTask, Resource.Id.registerCard, ACTION_REGISTER_CARD);
        }

        private void getTransactions_Click(object sender, EventArgs e)
        {
            MsgText.Text = "";
        }

#endregion //Button_Click_Events

        private void dealWithResult(int requestCode, Result resultCode, IResult<ITransactionResult> data)
        {
            Receipt receipt = null;
            string msg_prefix = "";

            if (resultCode == Result.Canceled)
            {
                ResponseText = "Payment Canceled.";
                return;
            }
            else if (resultCode == JudoSDKManager.JUDO_ERROR)
            {
                ResponseText = string.Format("Error: {0}", data.Error.ErrorMessage);
                return;
            }

            if (data != null)
            {
                receipt = new Receipt(data.Response);
            }

            if (receipt == null)
            {
                ResponseText = string.Format("Error: {0}", data.Error.ErrorMessage);
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
                ResponseText = string.Format("{0}: id: {1},\r\nMessage: {2},\r\nresult: {3}", msg_prefix, receipt.ReceiptId, receipt.Message, receipt.Result);
            }
        }

    }
}