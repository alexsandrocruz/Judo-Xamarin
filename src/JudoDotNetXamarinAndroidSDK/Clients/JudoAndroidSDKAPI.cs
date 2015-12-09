using System;
using JudoDotNetXamarin;
using Android.App;

namespace JudoDotNetXamarinAndroidSDK
{
    public interface JudoAndroidSDKAPI
    {
        void Payment (PaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure, Activity context);

        void PreAuth (PaymentViewModel preAuthorisation, JudoSuccessCallback success, JudoFailureCallback failure, Activity context);

        void TokenPayment (TokenPaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure, Activity context);

        void TokenPreAuth (TokenPaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure, Activity context);

        void RegisterCard (PaymentViewModel registerCard, JudoSuccessCallback success, JudoFailureCallback failure, Activity context);
    }
}

