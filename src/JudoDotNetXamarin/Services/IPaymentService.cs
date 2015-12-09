using System.Threading.Tasks;
using JudoDotNetXamarin;
using JudoPayDotNet.Models;
using System.Runtime.CompilerServices;


[assembly: InternalsVisibleTo("JudoDotNetXamariniOSSDK")]
[assembly: InternalsVisibleTo("JudoDotNetXamarinAndroidSDK")]
namespace JudoDotNetXamarin
{
	internal interface IPaymentService
	{
		Task<IResult<ITransactionResult>> MakePayment (PaymentViewModel payment,IClientService clientService);
		Task<IResult<ITransactionResult>> PreAuthoriseCard (PaymentViewModel payment,IClientService clientService);
		Task<IResult<ITransactionResult>> MakeTokenPayment (TokenPaymentViewModel payment,IClientService clientService);
		Task<IResult<ITransactionResult>> MakeTokenPreAuthorisation (TokenPaymentViewModel payment,IClientService clientService);
		Task<IResult<ITransactionResult>> RegisterCard(PaymentViewModel payment,IClientService clientService);
		Task<IResult<ITransactionResult>> CompleteDSecure (string receiptID,string PaRes, string MD);
	}
}

