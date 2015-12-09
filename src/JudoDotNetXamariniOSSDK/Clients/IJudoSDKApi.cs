using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using JudoPayDotNet.Models;

#if __UNIFIED__
using Foundation;
using UIKit;
using CoreFoundation;
using CoreGraphics;
// Mappings Unified CoreGraphic classes to MonoTouch classes
using RectangleF = global::CoreGraphics.CGRect;
using SizeF = global::CoreGraphics.CGSize;
using PointF = global::CoreGraphics.CGPoint;
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
    internal interface IJudoSDKApi 
    {
		void Payment(PaymentViewModel payment, SuccessCallback success, FailureCallback failure, UINavigationController navigationController);
        void PreAuth(PaymentViewModel preAuthorisation, SuccessCallback success, FailureCallback failure, UINavigationController navigationController);
        void TokenPayment(TokenPaymentViewModel payment, SuccessCallback success, FailureCallback failure, UINavigationController navigationController);
        void TokenPreAuth(TokenPaymentViewModel payment, SuccessCallback success, FailureCallback failure, UINavigationController navigationController);
        void RegisterCard(PaymentViewModel payment, SuccessCallback success, FailureCallback failure, UINavigationController navigationController);

		void ApplePayment (ApplePayViewModel payment, SuccessCallback success, FailureCallback failure, UINavigationController navigationController,ApplePaymentType type);
    }
}