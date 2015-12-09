using System.Threading.Tasks;
using Android.OS;
using Android.Widget;
using JudoPayDotNet.Models;
using JudoDotNetXamarin;
using JudoDotNetXamarinAndroidSDK.Utils;
using Android.App;

namespace JudoDotNetXamarinAndroidSDK.Activities
{
    [Activity]
    public class PreAuthActivity : PaymentActivity
    {
        private ClientService clientService;
        IPaymentService _paymentService;
        ServiceFactory factory;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            SetTitle (Resource.String.title_pre_auth);
            FindViewById<Button> (Resource.Id.payButton).Text = Resources.GetString (Resource.String.preauth);
            clientService = new ClientService ();
            factory = new ServiceFactory ();
            _paymentService = factory.GetPaymentService (); 

        }

        public override void MakeCardPayment ()
        {

            ShowLoadingSpinner (true);
            PaymentViewModel cardPayment = new PaymentViewModel ();
            CardViewModel card;
            UiUtils.GatherCardDetails (cardEntryView, avsEntryView, startDateEntryView, out card);
            cardPayment.Card = card;
            cardPayment.JudoID = judoId;
            cardPayment.Currency = judoCurrency;
            cardPayment.Amount = judoAmount;
            cardPayment.PaymentReference = judoPaymentRef;
            cardPayment.ConsumerReference = judoConsumer.YourConsumerReference;

            _paymentService.PreAuthoriseCard (cardPayment, clientService).ContinueWith (HandleServerResponse, TaskScheduler.FromCurrentSynchronizationContext ());
        }
    }
}