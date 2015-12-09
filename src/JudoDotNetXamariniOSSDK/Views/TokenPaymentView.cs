using System;
using System.Collections.Generic;
using CoreFoundation;
using Foundation;
using JudoDotNetXamarin;
using JudoDotNetXamariniOSSDK.Controllers;
using JudoDotNetXamariniOSSDK.Helpers;
using JudoDotNetXamariniOSSDK.Services;
using JudoDotNetXamariniOSSDK.TableSources;
using JudoDotNetXamariniOSSDK.Views.TableCells.Card;
using JudoPayDotNet.Models;
using UIKit;

#if __UNIFIED__
// Mappings Unified CoreGraphic classes to MonoTouch classes
#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreGraphics;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreAnimation;
// Mappings Unified types to MonoTouch types
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif


namespace JudoDotNetXamariniOSSDK.Views
{
    internal partial class TokenPaymentView : UIViewController
    {
        IPaymentService _paymentService;
        bool KeyboardVisible = false;

        public TokenPaymentView (IPaymentService paymentService) : base ("TokenPaymentView", null)
        {
            _paymentService = paymentService;
        }

        TokenPaymentCell tokenCell;

        private List<CardCell> CellsToShow { get; set; }

        public JudoSuccessCallback successCallback { get; set; }

        public JudoFailureCallback failureCallback { get; set; }

        public TokenPaymentViewModel tokenPayment { get; set; }

        public override void ViewWillLayoutSubviews ()
        {
            base.ViewWillLayoutSubviews ();
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
                this.View.Superview.RepositionFormSheetForiPad ();
            }
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            TableView.SeparatorColor = UIColor.Clear;

            if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad) {
                NSNotificationCenter defaultCenter = NSNotificationCenter.DefaultCenter;
                defaultCenter.AddObserver (UIKeyboard.WillHideNotification, OnKeyboardNotification);
                defaultCenter.AddObserver (UIKeyboard.WillShowNotification, OnKeyboardNotification);
            }

            if (String.IsNullOrEmpty (tokenPayment.Token)) {

                DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {						

                    UIAlertView _error = new UIAlertView ("Missing Token", "No Card Token found. Please provide application with token via Pre-Authentication or Payment", null, "ok", null);
                    _error.Show ();

                    _error.Clicked += (sender, args) => {
                        PaymentButton.Disable ();
                        if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
                            this.DismissViewController (true, null);
                        } else {
                            this.NavigationController.PopViewController (true);
                        }
                    };

                });
            } else {

                SetUpTableView ();

                UITapGestureRecognizer tapRecognizer = new UITapGestureRecognizer ();

                tapRecognizer.AddTarget (() => { 
                    if (KeyboardVisible) {
                        DismissKeyboardAction ();
                    }
                });

                tapRecognizer.NumberOfTapsRequired = 1;
                tapRecognizer.NumberOfTouchesRequired = 1;

                EncapsulatingView.AddGestureRecognizer (tapRecognizer);
                PaymentButton.Alpha = 0.25f;
                PaymentButton.Enabled = false;


                PaymentButton.TouchUpInside += (sender, ev) => {
                    MakeTokenPayment ();
                };

                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
                    FormClose.TouchUpInside += (sender, ev) => {
                        this.DismissViewController (true, null);
                    };
                }
            }
        }

        private void OnKeyboardNotification (NSNotification notification)
        {
            KeyboardVisible = notification.Name == UIKeyboard.WillShowNotification;
        }

        void DismissKeyboardAction ()
        {
            tokenCell.DismissKeyboardAction ();
        }

        void SetUpTableView ()
        {
            tokenCell = new TokenPaymentCell (new IntPtr ());

            tokenCell = (TokenPaymentCell)tokenCell.Create ();
            tokenCell.CardType = tokenPayment.CardType;
            tokenCell.LastFour = tokenPayment.LastFour;

            tokenCell.UpdateUI = () => {
                UpdateUI ();
            };
				
            CellsToShow = new List<CardCell> (){ tokenCell };

            CardCellSource tableSource = new CardCellSource (CellsToShow);
            TableView.Source = tableSource;
        }

        private void UpdateUI ()
        {	
            PaymentButton.Enabled = tokenCell.Complete;
            PaymentButton.Alpha = (tokenCell.Complete == true ? 1f : 0.25f);
            if (tokenCell.Complete) {
                DismissKeyboardAction ();
            }
        }

        public override void ViewDidDisappear (bool animated)
        {
            base.ViewWillDisappear (animated);

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
                this.View.Hidden = true;
            }
        }

        public void MakeTokenPayment ()
        {
            try {

                LoadingScreen.ShowLoading (this.View);
                var instance = JudoConfiguration.Instance;
                tokenPayment.CV2 = tokenCell.CCV;

                PaymentButton.Disable ();

                _paymentService.MakeTokenPayment (tokenPayment, new ClientService ()).ContinueWith (reponse => {

                    if (reponse.Exception != null) {
                        LoadingScreen.HideLoading ();
                        DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
                            NavigationController.CloseView ();
                        
                            reponse.Exception.FlattenToJudoFailure (failureCallback);
                        });
                    } else {
                        var result = reponse.Result;
                        if (result != null && !result.HasError && result.Response.Result != "Declined") {
                            PaymentReceiptModel paymentreceipt = result.Response as PaymentReceiptModel;
                      
                            // call success callback
                            if (successCallback != null) {
                                DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
                                    NavigationController.CloseView ();
                              
                                    successCallback (paymentreceipt);
                                });
                            }

                        } else {
                            // Failure callback
                            if (failureCallback != null) {
                                var judoError = new JudoError { ApiError = result != null ? result.Error : null };
                                var paymentreceipt = result != null ? result.Response as PaymentReceiptModel : null;

                                if (paymentreceipt != null) {
                                    // send receipt even we got card declined
                                    DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
                                        NavigationController.CloseView ();
                                    
                                        failureCallback (judoError, paymentreceipt);
                                    });
                                } else {
                                    DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
                                        NavigationController.CloseView ();
                                   
                                        failureCallback (judoError);
                                    });
                                }
                            }


                        }
                    }
                    LoadingScreen.HideLoading ();
                    
                });
                
            } catch (Exception ex) {
                LoadingScreen.HideLoading ();
                // Failure callback
                if (failureCallback != null) {
                    var judoError = new JudoError { Exception = ex };
                    DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
                        NavigationController.CloseView ();
                    
                        failureCallback (judoError);
                    });
                }
            }


        }

    }
}

