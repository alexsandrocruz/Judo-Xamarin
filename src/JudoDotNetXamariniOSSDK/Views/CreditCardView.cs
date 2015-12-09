﻿using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using JudoDotNetXamarin;
using JudoDotNetXamariniOSSDK.Controllers;
using JudoDotNetXamariniOSSDK.Helpers;
using JudoDotNetXamariniOSSDK.Services;
using JudoDotNetXamariniOSSDK.TableSources;
using JudoDotNetXamariniOSSDK.Views.TableCells.Card;
using JudoPayDotNet.Models;
using UIKit;
using JudoPayDotNet.Errors;
using CoreFoundation;


#if __UNIFIED__
// Mappings Unified CoreGraphic classes to MonoTouch classes
using PointF = global::CoreGraphics.CGPoint;

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
    internal partial class CreditCardView : UIViewController
    {
        private UIView _activeview;
        private bool _moveViewUp;
        private readonly IPaymentService _paymentService;
        private bool _keyboardVisible;

        private List<CardCell> CellsToShow { get; set; }

        CardEntryCell detailCell;

        ReassuringTextCell reassuringCell{ get; set; }

        MaestroCell maestroCell { get; set; }

        AVSCell avsCell{ get; set; }

        public JudoSuccessCallback successCallback { private get; set; }

        public JudoFailureCallback failureCallback { private get; set; }


        public PaymentViewModel cardPayment { get; set; }

        public CreditCardView (IPaymentService paymentService) : base ("CreditCardView", null)
        {
            _paymentService = paymentService;
        }

        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);
            this.View.BackgroundColor = UIColor.FromRGB (245f, 245f, 245f);
        }

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
            SetUpTableView ();

            this.View.BackgroundColor = UIColor.FromRGB (245f, 245f, 245f);

            if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad) {
                NSNotificationCenter defaultCenter = NSNotificationCenter.DefaultCenter;
                defaultCenter.AddObserver (UIKeyboard.WillHideNotification, OnKeyboardNotification);
                defaultCenter.AddObserver (UIKeyboard.WillShowNotification, OnKeyboardNotification);
                defaultCenter.AddObserver (UIKeyboard.DidShowNotification, KeyBoardUpNotification);
            }

            UITapGestureRecognizer tapRecognizer = new UITapGestureRecognizer ();

            tapRecognizer.AddTarget (() => { 
                if (_keyboardVisible) {
                    DismissKeyboardAction ();
                }
            });

            tapRecognizer.NumberOfTapsRequired = 1;
            tapRecognizer.NumberOfTouchesRequired = 1;

            EncapsulatingView.AddGestureRecognizer (tapRecognizer);

            SubmitButton.SetTitleColor (UIColor.Black, UIControlState.Application);

            SubmitButton.TouchUpInside += (sender, ev) => {
                MakePayment ();
            };

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
                FormClose.TouchUpInside += (sender, ev) => {
                    this.DismissViewController (true, null);
                };
            }
            SubmitButton.Disable ();
            detailCell.ccTextOutlet.BecomeFirstResponder ();


            SecureWebView.SetupWebView (_paymentService, successCallback, failureCallback);
        }

        private void OnKeyboardNotification (NSNotification notification)
        {
            _keyboardVisible = notification.Name == UIKeyboard.WillShowNotification;

            if (!_keyboardVisible) {
                if (_moveViewUp) {
                    ScrollTheView (false);
                }
            }
        }

        private void KeyBoardUpNotification (NSNotification notification)
        {

            if (avsCell.PostcodeTextFieldOutlet.IsFirstResponder)
                _activeview = avsCell.PostcodeTextFieldOutlet;
            if (_activeview != null && !detailCell.HasFocus ()) {
                _moveViewUp = true;
                ScrollTheView (_moveViewUp);

            }
        }


        private void ScrollTheView (bool move)
        {
            if (move) {
                TableView.SetContentOffset (new PointF (0, 100f), true);
                TableView.ScrollEnabled = false;
            } else {
                TableView.SetContentOffset (new PointF (0, 0), true);

            }

        }

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);

        }

        void DismissKeyboardAction ()
        {
            detailCell.DismissKeyboardAction ();
            avsCell.DismissKeyboardAction ();
            maestroCell.DismissKeyboardAction ();
        }

        private void UpdateUI ()
        {
            bool enable = false;
            enable = detailCell.EntryComplete ();

            List<CardCell> cellsToRemove = new List<CardCell> ();
            List<CardCell> insertedCells = new List<CardCell> ();
            List<CardCell> cellsBeforeUpdate = CellsToShow.ToList ();


            if (enable) {
                bool ccIsFirstResponder = detailCell.ccTextOutlet.IsFirstResponder;
                int row = CellsToShow.IndexOf (detailCell) + 1;

                if (JudoSDKManager.Instance.AVSEnabled) {
                    if (!CellsToShow.Contains (avsCell)) {
                        TableView.BeginUpdates ();
                        CellsToShow.Insert (row, avsCell);
                        row++;
                        insertedCells.Add (avsCell);
                        avsCell.PostcodeTextFieldOutlet.BecomeFirstResponder ();
                        ccIsFirstResponder = false;

	
                        TableView.InsertRows (new NSIndexPath[]{ NSIndexPath.FromRowSection (CellsToShow.IndexOf (avsCell), 0) }, UITableViewRowAnimation.Fade);
                        TableView.EndUpdates ();
                    }

                }
                if (detailCell.Type == CardType.MAESTRO && JudoSDKManager.Instance.MaestroAccepted) {
                    if (!CellsToShow.Contains (maestroCell)) {
                        TableView.BeginUpdates ();
                        CellsToShow.Insert (row, maestroCell);

                        insertedCells.Add (maestroCell);
                        maestroCell.StartDateTextFieldOutlet.BecomeFirstResponder ();
                        ccIsFirstResponder = false;
                        TableView.InsertRows (new NSIndexPath[]{ NSIndexPath.FromRowSection (CellsToShow.IndexOf (maestroCell), 0) }, UITableViewRowAnimation.Fade);
                        TableView.EndUpdates ();
                    }

                    if (maestroCell.IssueNumberTextFieldOutlet.Text.Length == 0 && maestroCell.StartDateTextFieldOutlet.Text.Length != 5) {
                        enable = false;
                    }
						
                }
					
                if (ccIsFirstResponder) {
                    DismissKeyboardAction ();

                    ccIsFirstResponder = false;
                }
            } else {
                TableView.BeginUpdates ();
                if (JudoSDKManager.Instance.MaestroAccepted) {
                    if (CellsToShow.Contains (maestroCell)) {
                        cellsToRemove.Add (maestroCell);
                    }
                }

                if (JudoSDKManager.Instance.AVSEnabled) {
                    if (CellsToShow.Contains (avsCell)) {
                        cellsToRemove.Add (avsCell);
                    }
                }
                List<NSIndexPath> indexPathsToRemove = new List<NSIndexPath> ();

                foreach (CardCell cell in cellsToRemove) {
                    indexPathsToRemove.Add (NSIndexPath.FromRowSection (cellsBeforeUpdate.IndexOf (cell), 0));
                }

                TableView.DeleteRows (indexPathsToRemove.ToArray (), UITableViewRowAnimation.Fade);

                foreach (CardCell cell in cellsToRemove) {
                    CellsToShow.Remove (cell);
                }

                TableView.EndUpdates ();
            }
				
            SubmitButton.Enabled = enable;
            SubmitButton.Alpha = (enable == true ? 1f : 0.25f);

        }

        void SetUpTableView ()
        {
            detailCell = new CardEntryCell (new IntPtr ());
            reassuringCell = new ReassuringTextCell (new IntPtr ());
            avsCell = new AVSCell (new IntPtr ());
            maestroCell = new MaestroCell (new IntPtr ());


            detailCell = (CardEntryCell)detailCell.Create ();
            reassuringCell = (ReassuringTextCell)reassuringCell.Create ();
            avsCell = (AVSCell)avsCell.Create ();
            maestroCell = (MaestroCell)maestroCell.Create ();


            detailCell.UpdateUI = () => {
                UpdateUI ();
            };

            avsCell.UpdateUI = () => {
                UpdateUI ();
            };

            maestroCell.UpdateUI = () => {
                UpdateUI ();
            };

            CellsToShow = new List<CardCell> (){ detailCell, reassuringCell };



            CardCellSource tableSource = new CardCellSource (CellsToShow);
            TableView.Source = tableSource;
            TableView.SeparatorColor = UIColor.Clear;

        }


        public override void ViewDidDisappear (bool animated)
        {
            base.ViewWillDisappear (animated);

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
                this.View.Hidden = true;
            }
        }



        private void MakePayment ()
        {
            try {
                LoadingScreen.ShowLoading (this.View);
             
                cardPayment.Card = GatherCardDetails ();

                SubmitButton.Disable ();

                _paymentService.MakePayment (cardPayment, new ClientService ()).ContinueWith (reponse => {
                    if (reponse.Exception != null) {
                        LoadingScreen.HideLoading ();
                        DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
                            NavigationController.CloseView ();
                        
                            reponse.Exception.FlattenToJudoFailure (failureCallback);
                        });
                    } else {
                        var result = reponse.Result;
                        if (JudoSDKManager.Instance.ThreeDSecureEnabled && result.Response != null && result.Response.GetType () == typeof(PaymentRequiresThreeDSecureModel)) {

                            var threedDSecureReceipt = result.Response as PaymentRequiresThreeDSecureModel;


                            SecureManager.SummonThreeDSecure (threedDSecureReceipt, SecureWebView);

                        } else {
                            if (result != null && !result.HasError && result.Response.Result != "Declined") {
                                var paymentreceipt = result.Response as PaymentReceiptModel;

                                if (paymentreceipt != null) {
                                    // call success callback
                                    if (successCallback != null) {
                                        DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {

                                            NavigationController.CloseView ();
                                            successCallback (paymentreceipt);
                                        });

                                    }
                                   
                                } else {
                                    var threedDSecureReceipt = result.Response as PaymentRequiresThreeDSecureModel;
                                    if (threedDSecureReceipt != null) {
                                        DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
                                            NavigationController.CloseView ();
                                      
                                            failureCallback (new JudoError { ApiError = new JudoPayDotNet.Errors.JudoApiErrorModel {
                                                    ErrorMessage = "Account requires 3D Secure but application is not configured to accept it",
                                                    ErrorType = JudoApiError.General_Error,
                                                    ModelErrors = null
                                                }
                                            });
                                        });
                                    } else {
                                        throw new Exception ("JudoXamarinSDK: unable to find the receipt in response.");
                                    }
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

                            LoadingScreen.HideLoading ();
                        }
                    }
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

        void CleanOutCardDetails ()
        {
            detailCell.CleanUp ();

            if (JudoSDKManager.Instance.MaestroAccepted) {
                maestroCell.CleanUp ();

            }	
            if (JudoSDKManager.Instance.AVSEnabled) {
                avsCell.CleanUp ();
            }
        }

        CardViewModel GatherCardDetails ()
        {
            CardViewModel cardViewModel = new CardViewModel ();
            detailCell.GatherCardDetails (cardViewModel);


            if (JudoSDKManager.Instance.AVSEnabled) {
                avsCell.GatherCardDetails (cardViewModel);

            }

            if (detailCell.Type == CardType.MAESTRO) {
                maestroCell.GatherCardDetails (cardViewModel);

            }

            return cardViewModel;
        }

    }


}

