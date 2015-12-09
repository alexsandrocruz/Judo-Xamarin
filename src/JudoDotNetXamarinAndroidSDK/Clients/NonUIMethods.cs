using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using JudoPayDotNet.Models;
using JudoDotNetXamarin;
using System;
using Android.Views.InputMethods;
using Android.Views;

namespace JudoDotNetXamarinAndroidSDK
{
    internal class NonUIMethods : Activity, JudoAndroidSDKAPI
    {
     
        IPaymentService _paymentService;
        ServiceFactory factory;

        public NonUIMethods ()
        {
            factory = new ServiceFactory ();
            _paymentService = factory.GetPaymentService (); 
        }

        public void Payment (PaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure, Activity context)
        {
            
            try {
                _paymentService.MakePayment (payment, new ClientService ()).ContinueWith (reponse => HandleResponse (success, failure, reponse));
            } catch (Exception ex) {
                // Failure
                HandleFailure (failure, ex);
            }
        }

        public void PreAuth (PaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure, Activity context)
        {
            try {
                _paymentService.PreAuthoriseCard (payment, new ClientService ()).ContinueWith (reponse => HandleResponse (success, failure, reponse));
            } catch (Exception ex) {
                // Failure
                HandleFailure (failure, ex);
            }
        }

        public void TokenPayment (TokenPaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure, Activity context)
        {
            try {
                _paymentService.MakeTokenPayment (payment, new ClientService ()).ContinueWith (reponse => HandleResponse (success, failure, reponse));
            } catch (Exception ex) {
                // Failure
                HandleFailure (failure, ex);
            }
        }

        public void TokenPreAuth (TokenPaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure, Activity context)
        {
            try {
                _paymentService.MakeTokenPreAuthorisation (payment, new ClientService ()).ContinueWith (reponse => HandleResponse (success, failure, reponse));
            } catch (Exception ex) {
                // Failure
                HandleFailure (failure, ex);
            }
        }

        public void RegisterCard (PaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure, Activity context)
        {
            try {
                _paymentService.RegisterCard (payment, new ClientService ()).ContinueWith (reponse => HandleResponse (success, failure, reponse));
            } catch (Exception ex) {
                // Failure
                HandleFailure (failure, ex);
            }
        }

        private void HandleFailure (JudoFailureCallback failure, Exception ex)
        {
            if (failure != null) {
                var judoError = new JudoError {
                    Exception = ex
                };
                failure (judoError);
            }
        }

        private void HandleResponse (JudoSuccessCallback success, JudoFailureCallback failure, Task<IResult<ITransactionResult>> reponse)
        {
            var result = reponse.Result;
            if (result != null && !result.HasError && result.Response.Result != "Declined") {
                var secureReceipt = result.Response as PaymentRequiresThreeDSecureModel;
                if (secureReceipt != null) {
                    var judoError = new JudoError { ApiError = result != null ? result.Error : null };
                    failure (new JudoError { ApiError = new JudoPayDotNet.Errors.JudoApiErrorModel {
                            ErrorMessage = "Account requires 3D Secure but non UI Mode does not support this",
                            ErrorType = JudoApiError.General_Error,
                            ModelErrors = null
                        }
                    });
                }

                var paymentReceipt = result.Response as PaymentReceiptModel;

                if (success != null) {
                    success (paymentReceipt);
                } else {
                    throw new Exception ("SuccessCallback is not set.");
                }
            } else {
                // Failure
                if (failure != null) {
                    var judoError = new JudoError { ApiError = result != null ? result.Error : null };
                    var paymentreceipt = result != null ? result.Response as PaymentReceiptModel : null;

                    if (paymentreceipt != null) {
                        // send receipt even we got card declined
                        failure (judoError, paymentreceipt);
                    } else {
                        failure (judoError);
                    }
                } else {
                    throw new Exception ("FailureCallback is not set.");
                }
            }
        }




    }
}