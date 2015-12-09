using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using JudoPayDotNet.Models;

namespace JudoDotNetXamarinSDK.Clients
{
    internal class NonUIMethods : INonUIMethods
    {
        public Task<IResult<ITransactionResult>> Payment(Context context, string judoId, string currency, decimal amount, string paymentReference,
            string consumerReference, IDictionary<string, string> metaData, 
			string cardNumber,string postCode, string startDate, string expiryDate, string cv2)
        {
            var cardPayment = new CardPaymentModel()
            {
                JudoId = judoId,
                Currency = currency,
                Amount = amount,
                YourPaymentReference = paymentReference,
                YourConsumerReference = consumerReference,
                YourPaymentMetaData = metaData,
                CardNumber = cardNumber,
                CardAddress = new CardAddressModel { PostCode = postCode },
                StartDate = startDate,
                ExpiryDate = expiryDate,
                CV2 = cv2,
                ClientDetails = JudoSDKManager.GetClientDetails(context),
				UserAgent = JudoSDKManager.GetSDKVersion()
            };

            return JudoSDKManager.JudoClient.Payments.Create(cardPayment);
        }

        public Task<IResult<ITransactionResult>> TokenPayment(Context context, string judoId, string currency, decimal amount, string paymentReference,
            string consumerToken, string consumerReference, IDictionary<string, string> metaData,
            string cardToken, string cv2)
        {
            TokenPaymentModel payment = new TokenPaymentModel()
            {
                JudoId = judoId,
                Currency = currency,
                Amount = amount,
                YourPaymentReference = paymentReference,
                ConsumerToken = consumerToken,
                YourConsumerReference = consumerReference,
                YourPaymentMetaData = metaData,
                CardToken = cardToken,
                CV2 = cv2,
                ClientDetails = JudoSDKManager.GetClientDetails(context),
				UserAgent = JudoSDKManager.GetSDKVersion()
            };

            return JudoSDKManager.JudoClient.Payments.Create(payment);
        }

        public Task<IResult<ITransactionResult>> PreAuth(Context context, string judoId, string currency, decimal amount, string paymentReference,
            string consumerReference, IDictionary<string, string> metaData, string cardNumber,
            string postCode, string startDate, string expiryDate, string cv2)
        {
            var cardPayment = new CardPaymentModel()
            {
                JudoId = judoId,
                Currency = currency,
                Amount = amount,
                YourPaymentReference = paymentReference,
                YourConsumerReference = consumerReference,
                YourPaymentMetaData = metaData,
                CardNumber = cardNumber,
                CardAddress = new CardAddressModel { PostCode = postCode },
                StartDate = startDate,
                ExpiryDate = expiryDate,
                CV2 = cv2,
                ClientDetails = JudoSDKManager.GetClientDetails(context),
				UserAgent = JudoSDKManager.GetSDKVersion()
            };

            return JudoSDKManager.JudoClient.PreAuths.Create(cardPayment);
        }

        public Task<IResult<ITransactionResult>> TokenPreAuth(Context context, string judoId, string currency, decimal amount, string paymentReference,
            string consumerToken, string consumerReference, IDictionary<string, string> metaData,
            string cardToken, string cv2)
        {
            TokenPaymentModel payment = new TokenPaymentModel()
            {
                JudoId = judoId,
                Currency = currency,
                Amount = amount,
                YourPaymentReference = paymentReference,
                ConsumerToken = consumerToken,
                YourConsumerReference = consumerReference,
                YourPaymentMetaData = metaData,
                CardToken = cardToken,
                CV2 = cv2,
                ClientDetails = JudoSDKManager.GetClientDetails(context),
				UserAgent = JudoSDKManager.GetSDKVersion()
            };

            return JudoSDKManager.JudoClient.PreAuths.Create(payment);
        }

        public Task<IResult<ITransactionResult>> RegisterCard(string cardNumber, string cv2, string expiryDate, string consumerReference, string postCode)
        {
            var registerCard = new RegisterCardModel()
            {
                CardAddress = new CardAddressModel()
                {
                    PostCode = postCode
                },
                CardNumber = cardNumber,
                CV2 = cv2,
                ExpiryDate = expiryDate,
                YourConsumerReference = consumerReference
            };

            return JudoSDKManager.JudoClient.RegisterCards.Create(registerCard);
        }
    }
}