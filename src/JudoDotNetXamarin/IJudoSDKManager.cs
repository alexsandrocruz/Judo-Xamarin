using System;
using JudoDotNetXamarin;


namespace JudoDotNetXamarin
{
    public interface IJudoSDKManager
    {

        /// <summary>
        /// Enable 3D security process
        /// </summary>
        bool ThreeDSecureEnabled{ get; set; }

        /// <summary>
        /// Enable/Disable AVS check
        /// </summary>
        bool AVSEnabled { get; set; }

        /// <summary>
        /// Enable/Disable Amex card support
        /// </summary>
        bool AmExAccepted { get; set; }

        /// <summary>
        /// Enable/Disable Mestro card support
        /// </summary>
        bool MaestroAccepted { get; set; }


        /// <summary>
        /// Enable/Disable risk signal to pass fruad monitoring device data
        /// default is true
        /// </summary>
        bool RiskSignals{ get; set; }


        /// <summary>
        /// SSLPinningEnabled
        /// </summary>
        bool SSLPinningEnabled { get; set; }

        bool AllowRooted { get; set; }

        void Payment (PaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure);

        void PreAuth (PaymentViewModel preAuthorisation, JudoSuccessCallback success, JudoFailureCallback failure);

        void TokenPayment (TokenPaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure);

        void TokenPreAuth (TokenPaymentViewModel payment, JudoSuccessCallback success, JudoFailureCallback failure);

        void RegisterCard (PaymentViewModel registerCard, JudoSuccessCallback success, JudoFailureCallback failure);

    }
}

