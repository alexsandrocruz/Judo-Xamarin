using System;
using System.Threading.Tasks;
using JudoDotNetXamarin;
using JudoDotNetXamariniOSSDK.Services;
using JudoPayDotNet.Models;

#if __UNIFIED__
// Mappings Unified CoreGraphic classes to MonoTouch classes

#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreGraphics;
// Mappings Unified types to MonoTouch types
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

namespace JudoDotNetXamariniOSSDK.Clients
{
    internal class NonUIMethods :IJudoSDKApi
    {
        private readonly IPaymentService _paymentService;

        public NonUIMethods (IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public void Payment (PaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure)
        {
            try {
                _paymentService.MakePayment (payment, new ClientService ()).ContinueWith (reponse => HandResponse (success, failure, reponse));
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

        public void PreAuth (PaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure)
        {
            try {
                _paymentService.PreAuthoriseCard (payment, new ClientService ()).ContinueWith (reponse => HandResponse (success, failure, reponse));
            } catch (Exception ex) {
                // Failure
                HandleFailure (failure, ex);
            }
        }

        public void TokenPayment (TokenPaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure)
        {
            try {
                _paymentService.MakeTokenPayment (payment, new ClientService ()).ContinueWith (reponse => HandResponse (success, failure, reponse));
            } catch (Exception ex) {
                // Failure
                HandleFailure (failure, ex);
            }
        }

        public void TokenPreAuth (TokenPaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure)
        {
            try {
                _paymentService.MakeTokenPreAuthorisation (payment, new ClientService ()).ContinueWith (reponse => HandResponse (success, failure, reponse));
            } catch (Exception ex) {
                // Failure
                HandleFailure (failure, ex);
            }
        }

        public void RegisterCard (PaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure)
        {
            try {
                _paymentService.RegisterCard (payment, new ClientService ()).ContinueWith (reponse => HandResponse (success, failure, reponse));
            } catch (Exception ex) {
                // Failure
                HandleFailure (failure, ex);
            }
        }

        private static void HandResponse (JudoSuccessCallback success, JudoFailureCallback failure, Task<IResult<ITransactionResult>> reponse)
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
                        } });
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