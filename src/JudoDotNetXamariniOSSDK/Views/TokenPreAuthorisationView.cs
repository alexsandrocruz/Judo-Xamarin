﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using JudoPayDotNet.Models;

#if __UNIFIED__
using Foundation;
using UIKit;
using CoreFoundation;
using CoreAnimation;
using CoreGraphics;
using ObjCRuntime;
// Mappings Unified CoreGraphic classes to MonoTouch classes
using RectangleF = global::CoreGraphics.CGRect;
using SizeF = global::CoreGraphics.CGSize;
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

namespace JudoDotNetXamariniOSSDK
{
	internal partial class TokenPreAuthorisationView : UIViewController
	{
		IPaymentService _paymentService;
		bool KeyboardVisible = false;

		public TokenPreAuthorisationView (IPaymentService paymentService) : base ("TokenPreAuthorisationView", null)
		{
			_paymentService = paymentService;
		}

		TokenPaymentCell tokenCell;

		private List<CardCell> CellsToShow { get; set; }

		public SuccessCallback successCallback { get; set; }

		public FailureCallback failureCallback { get; set; }

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

            if (String.IsNullOrEmpty(tokenPayment.Token))
            {

				DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {						

					UIAlertView _error = new UIAlertView ("Missing Token", "No Card Token found. Please provide application with token via Pre-Authentication or Payment", null, "ok", null);
					_error.Show ();

					_error.Clicked += (sender, args) => {
						PaymentButton.Disable();
						if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
							this.DismissViewController (true, null);
						} else {
							this.NavigationController.PopToRootViewController (true);
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
				PaymentButton.Disable();

				PaymentButton.SetTitleColor (UIColor.Black, UIControlState.Application);

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
            tokenCell = new TokenPaymentCell(new IntPtr());

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


		public override void ViewDidDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				this.View.Hidden=true;
			}
		}

		private void UpdateUI ()
		{	
			PaymentButton.Enabled = tokenCell.Complete;
			PaymentButton.Alpha = (tokenCell.Complete == true ? 1f : 0.25f);
			if (tokenCell.Complete) {
				DismissKeyboardAction ();
			}
		}

		public void MakeTokenPayment ()
		{
			try {

				JudoSDKManager.ShowLoading (this.View);
				var instance = JudoConfiguration.Instance;
				tokenPayment.CV2 = tokenCell.CCV;

				PaymentButton.Disable();

				_paymentService.MakeTokenPreAuthorisation (tokenPayment).ContinueWith (reponse => {
					var result = reponse.Result;
					if (result != null && !result.HasError && result.Response.Result != "Declined") {
						PaymentReceiptModel paymentreceipt = result.Response as PaymentReceiptModel;
						// call success callback
						if (successCallback != null)
							successCallback (paymentreceipt);
					} else {
						// Failure callback
						if (failureCallback != null) {
							var judoError = new JudoError { ApiError = result != null ? result.Error : null };
							var paymentreceipt = result != null ? result.Response as PaymentReceiptModel : null;

							if (paymentreceipt != null) {
								// send receipt even we got card declined
								failureCallback (judoError, paymentreceipt);
							} else {
								failureCallback (judoError);
							}
						}


					}
					JudoSDKManager.HideLoading ();
				});
			} catch (Exception ex) {
				JudoSDKManager.HideLoading ();
				// Failure callback
				if (failureCallback != null) {
					var judoError = new JudoError { Exception = ex };
					failureCallback (judoError);
				}
			}


		}

	}
}

