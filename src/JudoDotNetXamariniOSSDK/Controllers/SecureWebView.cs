using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using JudoDotNetXamarin;
using JudoDotNetXamariniOSSDK.Services;
using JudoPayDotNet.Models;
using UIKit;
using CoreFoundation;
using AVFoundation;

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

namespace JudoDotNetXamariniOSSDK.Controllers
{
    [Register ("SecureWebView")]
    internal partial class SecureWebView :UIWebView
    {

        public SecureWebView (IntPtr p) : base (p)
        {
            this.LoadFinished += delegate {
                this.ScrollView.SetZoomScale (2.0f, true);
            };
            this.ShouldStartLoad = (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType) => {
				
                if (request.Url.ToString ().Contains ("threedsecurecallback") && ReceiptID != null) {
                    Dictionary<string,string> queryStringDictionary = new Dictionary<string,string> ();

                    var TrackTraceDataArray = request.Body.ToString ().Split (new char[] { '&' });

                    foreach (string keyValuePair in TrackTraceDataArray) {
                        var pairComponents = keyValuePair.Split (new char[] { '=' });
                        string key = pairComponents.First ();
                        string value = pairComponents.Last ();
                        queryStringDictionary.Add (key, value);
                    }

                    NSString paRes = new NSString (queryStringDictionary ["PaRes"]);
                    var paResUnEncoded = paRes.CreateStringByRemovingPercentEncoding ().ToString ();
                    paResUnEncoded = paResUnEncoded.Replace ("\r\n", string.Empty);

                    NSString md = new NSString (queryStringDictionary ["MD"]);
                    var mdUnEncoded = md.CreateStringByRemovingPercentEncoding ().ToString ();
                    mdUnEncoded = mdUnEncoded.Replace ("\r\n", string.Empty);
                    _paymentService.CompleteDSecure (ReceiptID, paResUnEncoded, mdUnEncoded).ContinueWith (reponse => {
                        var result = reponse.Result;
                        if (result != null && !result.HasError && result.Response.Result != "Declined") {
                            var paymentreceipt = result.Response as PaymentReceiptModel;

                            if (paymentreceipt != null) {
                                // call success callback
                                if (_successCallback != null) {
                                    CloseView ();
                                    _successCallback (paymentreceipt);
                                }	
                            } else {
                                throw new Exception ("JudoXamarinSDK: unable to find the receipt in response.");
                            }

                        } else {
                            // Failure callback
                            if (_failureCallback != null) {
                                var judoError = new JudoError { ApiError = result != null ? result.Error : null };
                                var paymentreceipt = result != null ? result.Response as PaymentReceiptModel : null;

                                if (paymentreceipt != null) {
                                    // send receipt even we got card declined
                                    CloseView ();
                                    _failureCallback (judoError, paymentreceipt);
                                   
                                } else {
                                    CloseView ();
                                    _failureCallback (judoError);


                                }
                            }
                        }
                    });

                }

                return true;
            };
        }

        internal void CloseView ()
        {
            DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
                var window = UIApplication.SharedApplication.KeyWindow;
                var vc = window.RootViewController;
                while (vc.PresentedViewController != null) {
                    vc = vc.PresentedViewController;

                }
                if (vc is UISplitViewController) {
                    var splitView = vc as UISplitViewController;
                    vc = splitView.ViewControllers [0];
                }

                if (vc is UINavigationController) {
                    var navC = vc as UINavigationController;
                    navC.PopViewController (true);
                }

              
                vc.DismissViewController (true, null);
             
            });

        }

        private IPaymentService _paymentService;
        public string ReceiptID;

        public JudoSuccessCallback _successCallback { get; set; }

        public JudoFailureCallback _failureCallback { get; set; }


        public void SetupWebView (IPaymentService paymentService, JudoSuccessCallback successCallback, JudoFailureCallback failureCallback)
        {
            _paymentService = paymentService;
            _successCallback = successCallback;
            _failureCallback = failureCallback;
        }

        public override void LoadRequest (NSUrlRequest r)
        {
            base.LoadRequest (r);
        }

    }
}

