using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using JudoDotNetXamarinSDK.Models;
using JudoPayDotNet.Models;

namespace JudoDotNetXamarinSDK.Activies
{
    [Activity(Label = "PreAuthTokenActivity")]
    public class PreAuthTokenActivity : PaymentTokenActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetTitle(Resource.String.title_pre_auth_token);
            FindViewById<Button>(Resource.Id.payButton).Text = Resources.GetString(Resource.String.token_preauth);
        }

        public override void MakeTokenPayment()
        {
            TokenPaymentModel payment = new TokenPaymentModel()
            {
                JudoId = judoId,
                Currency = judoCurrency,
                Amount = judoAmount,
                YourPaymentReference = judoPaymentRef,
                ConsumerToken = judoConsumer.ConsumerToken,
                YourConsumerReference = judoConsumer.YourConsumerReference,
                YourPaymentMetaData = judoMetaData.Metadata,
                CardToken = judoCardToken.Token,
                CV2 = cv2EntryView.GetCV2(),
                ClientDetails = JudoSDKManager.GetClientDetails(this),
				UserAgent = JudoSDKManager.GetSDKVersion()
            };

            ShowLoadingSpinner(true);

            JudoSDKManager.JudoClient.PreAuths.Create(payment).ContinueWith(HandleServerResponse, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}