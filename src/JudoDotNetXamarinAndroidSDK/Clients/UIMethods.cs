using System.Collections.Generic;
using Android.Content;
using JudoDotNetXamarinSDK.Activies;
using JudoDotNetXamarinSDK.Models;

namespace JudoDotNetXamarinSDK.Clients
{
    internal class UIMethods : IUIMethods
    {
        public Intent Payment(Context context, string judoId, string currency, string amount,
            string paymentReference, string consumerReference, Dictionary<string, string> metaData)
        {
            Intent intent = new Intent(context, typeof(PaymentActivity));
            intent.PutExtra(JudoSDKManager.JUDO_PAYMENT_REF, paymentReference);
            intent.PutExtra(JudoSDKManager.JUDO_CONSUMER, new Consumer(consumerReference));
            intent.PutExtra(JudoSDKManager.JUDO_AMOUNT, amount);
            intent.PutExtra(JudoSDKManager.JUDO_ID, judoId);
            intent.PutExtra(JudoSDKManager.JUDO_CURRENCY, currency);


            intent.PutExtra(JudoSDKManager.JUDO_META_DATA, new MetaData(metaData));

            return intent;
        }

        public Intent PreAuth(Context context, string judoId, string currency, string amount,
            string paymentReference, string consumerReference, Dictionary<string, string> metaData)
        {
            Intent intent = new Intent(context, typeof(PreAuthActivity));
            intent.PutExtra(JudoSDKManager.JUDO_PAYMENT_REF, paymentReference);
            intent.PutExtra(JudoSDKManager.JUDO_CONSUMER, new Consumer(consumerReference));
            intent.PutExtra(JudoSDKManager.JUDO_AMOUNT, amount);
            intent.PutExtra(JudoSDKManager.JUDO_ID, judoId);
            intent.PutExtra(JudoSDKManager.JUDO_CURRENCY, currency);


            intent.PutExtra(JudoSDKManager.JUDO_META_DATA, new MetaData(metaData));

            return intent;
        }

        public Intent TokenPayment(Context context, string judoId, string currency, string amount,
            string paymentReference, string consumerReference, CardToken cardToken, Dictionary<string, string> metaData, string consumerToken = null)
        {
            Intent intent = new Intent(context, typeof(PaymentTokenActivity));
            intent.PutExtra(JudoSDKManager.JUDO_PAYMENT_REF, paymentReference);
            intent.PutExtra(JudoSDKManager.JUDO_CONSUMER, new Consumer(consumerReference, consumerToken));
            intent.PutExtra(JudoSDKManager.JUDO_AMOUNT, amount);
            intent.PutExtra(JudoSDKManager.JUDO_ID, judoId);
            intent.PutExtra(JudoSDKManager.JUDO_CURRENCY, currency);
            intent.PutExtra(JudoSDKManager.JUDO_CARD_DETAILS, cardToken);


            intent.PutExtra(JudoSDKManager.JUDO_META_DATA, new MetaData(metaData));

            return intent;
        }

        public Intent TokenPreAuth(Context context, string judoId, string currency, string amount,
            string paymentReference, string consumerReference, CardToken cardToken, Dictionary<string, string> metaData, string consumerToken = null)
        {
            Intent intent = new Intent(context, typeof(PreAuthTokenActivity));
            intent.PutExtra(JudoSDKManager.JUDO_PAYMENT_REF, paymentReference);
            intent.PutExtra(JudoSDKManager.JUDO_CONSUMER, new Consumer(consumerReference, consumerToken));
            intent.PutExtra(JudoSDKManager.JUDO_AMOUNT, amount);
            intent.PutExtra(JudoSDKManager.JUDO_ID, judoId);
            intent.PutExtra(JudoSDKManager.JUDO_CURRENCY, currency);
            intent.PutExtra(JudoSDKManager.JUDO_CARD_DETAILS, cardToken);


            intent.PutExtra(JudoSDKManager.JUDO_META_DATA, new MetaData(metaData));

            return intent;
        }

        public Intent RegisterCard(Context context, string consumerReference)
        {
            Intent intent = new Intent(context, typeof(RegisterCardActivity));
            intent.PutExtra(JudoSDKManager.JUDO_CONSUMER, new Consumer(consumerReference));

            return intent;
        }
    }
}