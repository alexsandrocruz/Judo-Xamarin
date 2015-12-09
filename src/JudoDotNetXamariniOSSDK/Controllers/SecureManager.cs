using System;
using CoreFoundation;
using Foundation;
using JudoDotNetXamarin;
using JudoPayDotNet.Models;

namespace JudoDotNetXamariniOSSDK.Controllers
{
    public class SecureManager
    {
        internal static void SummonThreeDSecure (PaymentRequiresThreeDSecureModel threedDSecureReceipt, SecureWebView secureWebView)
        {
            secureWebView.ReceiptID =	threedDSecureReceipt.ReceiptId;

            NSCharacterSet allowedCharecterSet = NSCharacterSet.FromString (@":/=,!$&'()*+;[]@#?").InvertedSet;
            NSString paReq = new NSString (threedDSecureReceipt.PaReq);
            var encodedPaReq = paReq.CreateStringByAddingPercentEncoding (allowedCharecterSet);

            NSString termUrl = new NSString ("judo1234567890://threedsecurecallback");
            var encodedTermUrl = termUrl.CreateStringByAddingPercentEncoding (allowedCharecterSet);


            NSUrl url = new NSUrl (threedDSecureReceipt.AcsUrl);

            NSMutableUrlRequest req = new NSMutableUrlRequest (url);

            NSString postString = new NSString ("MD=" + threedDSecureReceipt.Md + "&PaReq=" + encodedPaReq + "&TermUrl=" + encodedTermUrl + "");
            NSData postData = postString.Encode (NSStringEncoding.UTF8);

            req.HttpMethod = "POST";
            req.Body = postData;

            try {
                DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
                    secureWebView.LoadRequest (req);

                    LoadingScreen.HideLoading ();
                    secureWebView.Hidden = false;
                });
            } catch (Exception e) {
                if (secureWebView._failureCallback != null) {
                    var judoError = new JudoError { Exception = e };
                    secureWebView.CloseView ();
                    secureWebView._failureCallback (judoError);
                }
            }
        }
    }
}

