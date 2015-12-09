using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Widget;
using JudoPayDotNet.Models;
using JudoDotNetXamarin;

namespace JudoDotNetXamarinAndroidSDK.Activities
{
    [Activity (Label = "PreAuthTokenActivity")]
    public class PreAuthTokenActivity : PaymentTokenActivity
    {
        private ClientService clientService;
        IPaymentService _paymentService;
        ServiceFactory factory;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            SetTitle (Resource.String.title_pre_auth_token);
            FindViewById<Button> (Resource.Id.payButton).Text = Resources.GetString (Resource.String.token_preauth);
            clientService = new ClientService ();
            factory = new ServiceFactory ();
            _paymentService = factory.GetPaymentService (); 
        }

        public override void MakeTokenPayment ()
        {
            TokenPaymentViewModel payment = new TokenPaymentViewModel () {
                Currency = judoCurrency,
                Amount = judoAmount,
                ConsumerToken = judoConsumer.ConsumerToken,
                CardType = judoCardToken.CardType,
                JudoID = judoId,
                Token = judoCardToken.Token,
                ConsumerReference = judoConsumer.YourConsumerReference,
                PaymentReference = judoPaymentRef,
                CV2 = cv2EntryView.GetCV2 ()

            };

            ShowLoadingSpinner (true);

            _paymentService.MakeTokenPreAuthorisation (payment, new ClientService ()).ContinueWith (HandleServerResponse, TaskScheduler.FromCurrentSynchronizationContext ());
        }
    }
}