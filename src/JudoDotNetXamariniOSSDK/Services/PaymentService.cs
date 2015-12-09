using System;

using JudoPayDotNet;
using JudoPayDotNet.Models;
using System.Threading.Tasks;

namespace JudoDotNetXamariniOSSDK
{
	internal class PaymentService : IPaymentService
	{
		private	JudoPayApi _judoAPI;

		public PaymentService (JudoPayApi judoAPI)
		{
			_judoAPI = judoAPI;
		}

		public async Task<IResult<ITransactionResult>> MakePayment (PaymentViewModel paymentViewModel)
		{
            try
            {
                CardPaymentModel payment = new CardPaymentModel
                {
                    JudoId = JudoConfiguration.Instance.JudoId,
                    YourPaymentReference = paymentViewModel.PaymentReference,
                    YourConsumerReference = paymentViewModel.ConsumerReference,
				    Amount = paymentViewModel.Amount,
				    CardNumber = paymentViewModel.Card.CardNumber,
				    CV2 = paymentViewModel.Card.CV2,
				    ExpiryDate = paymentViewModel.Card.ExpireDate,
				    CardAddress = new CardAddressModel() { PostCode = paymentViewModel.Card.PostCode, CountryCode = (int)paymentViewModel.Card.CountryCode },
				    StartDate = paymentViewModel.Card.StartDate,
                    IssueNumber = paymentViewModel.Card.IssueNumber,
                    YourPaymentMetaData = paymentViewModel.YourPaymentMetaData,
                    ClientDetails = JudoSDKManager.GetClientDetails(),
                    Currency = paymentViewModel.Currency,
					UserAgent = JudoSDKManager.GetSDKVersion()
                };

                Task<IResult<ITransactionResult>> task =  _judoAPI.Payments.Create(payment);

				return await task;
			}
			catch(Exception e){
				Console.WriteLine(e.InnerException.ToString());
				return null;
			}

		}
			
		public async Task<IResult<ITransactionResult>> PreAuthoriseCard (PaymentViewModel authorisation)
		{
            try
            {
                CardPaymentModel payment = new CardPaymentModel
                {
				    JudoId = JudoConfiguration.Instance.JudoId,
                    YourPaymentReference = authorisation.PaymentReference,
                    YourConsumerReference = authorisation.ConsumerReference,
				    Amount = authorisation.Amount,
				    CardNumber = authorisation.Card.CardNumber,
				    CV2 = authorisation.Card.CV2,
				    ExpiryDate = authorisation.Card.ExpireDate,
                    CardAddress = new CardAddressModel() { PostCode = authorisation.Card.PostCode, CountryCode = (int)authorisation.Card.CountryCode },
                    StartDate = authorisation.Card.StartDate,
                    IssueNumber = authorisation.Card.IssueNumber,
                    YourPaymentMetaData = authorisation.YourPaymentMetaData,
                    ClientDetails = JudoSDKManager.GetClientDetails(),
					UserAgent = JudoSDKManager.GetSDKVersion(),
                    Currency = authorisation.Currency
                };

				Task<IResult<ITransactionResult>> task =  _judoAPI.PreAuths.Create(payment);
				return await task;
			}
			catch(Exception e){
				Console.WriteLine(e.InnerException.ToString());
				return null;
			}

		}

		public async Task<IResult<ITransactionResult>> MakeTokenPayment (TokenPaymentViewModel tokenPayment)
		{
            try
            {
                TokenPaymentModel payment = new TokenPaymentModel
                {
				    JudoId = JudoConfiguration.Instance.JudoId,
                    YourPaymentReference = tokenPayment.PaymentReference,
                    YourConsumerReference = tokenPayment.ConsumerReference,
				    Amount = tokenPayment.Amount,
				    CardToken = tokenPayment.Token,
				    CV2 = tokenPayment.CV2,
                    ConsumerToken = tokenPayment.ConsumerToken,
                    YourPaymentMetaData = tokenPayment.YourPaymentMetaData,
                    ClientDetails = JudoSDKManager.GetClientDetails(),
					UserAgent = JudoSDKManager.GetSDKVersion()
			    };
				Task<IResult<ITransactionResult>> task =  _judoAPI.Payments.Create(payment);
				return await task;
			}
			catch(Exception e){
				Console.WriteLine(e.InnerException.ToString());
				return null;
			}

		}

		public async Task<IResult<ITransactionResult>> MakeTokenPreAuthorisation (TokenPaymentViewModel tokenPayment)
		{
			TokenPaymentModel payment = new TokenPaymentModel {
				JudoId = JudoConfiguration.Instance.JudoId,
                YourPaymentReference = tokenPayment.PaymentReference,
                YourConsumerReference = tokenPayment.ConsumerReference,
				Amount = tokenPayment.Amount,
				CardToken = tokenPayment.Token,
				CV2 = tokenPayment.CV2,
                ConsumerToken = tokenPayment.ConsumerToken,
                YourPaymentMetaData = tokenPayment.YourPaymentMetaData,
                ClientDetails = JudoSDKManager.GetClientDetails(),
				UserAgent = JudoSDKManager.GetSDKVersion()
			};
			try
			{
				Task<IResult<ITransactionResult>> task =  _judoAPI.PreAuths.Create(payment);
				return await task;
			}
			catch(Exception e){
				Console.WriteLine(e.InnerException.ToString());
				return null;
			}
		}

        public async Task<IResult<ITransactionResult>> RegisterCard(PaymentViewModel payment)
        {
            var registerCard = new RegisterCardModel()
            {
                CardAddress = new CardAddressModel()
                {
                    PostCode = payment.Card.PostCode
                },
                CardNumber = payment.Card.CardNumber,
                CV2 = payment.Card.CV2,
                ExpiryDate = payment.Card.ExpireDate,
                StartDate = payment.Card.StartDate,
                IssueNumber = payment.Card.IssueNumber,
                YourConsumerReference = payment.ConsumerReference
            };
            try
            {
                Task<IResult<ITransactionResult>> task = _judoAPI.RegisterCards.Create(registerCard);
                return await task;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.ToString());
                return null;
            }
        }

		public async Task<IResult<ITransactionResult>> CompleteDSecure (string receiptID,string paRes, string md)
		{
			try
			{
				ThreeDResultModel model = new ThreeDResultModel();
				model.PaRes = paRes;
				Task<IResult<PaymentReceiptModel>> task = _judoAPI.ThreeDs.Complete3DSecure(receiptID,model);
				return await task;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.InnerException.ToString());
				return null;
			}
		}
	}
}
