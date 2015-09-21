﻿using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CoreFoundation;
using JudoPayDotNet.Models;
using CoreGraphics;

namespace JudoDotNetXamariniOSSDK
{
	public partial class PreAuthorisationView : UIViewController
	{

		private UIView activeview;
		private bool moveViewUp = false;

		IPaymentService _paymentService;
		bool KeyboardVisible = false;

		private List<CardCell> CellsToShow { get; set; }

		CardEntryCell detailCell;

		ReassuringTextCell reassuringCell{ get; set; }

		MaestroCell maestroCell { get; set; }

		AVSCell avsCell{ get; set; }

		public SuccessCallback successCallback { get; set; }

		public FailureCallback failureCallback { get; set; }

		public PaymentViewModel authorisationModel { get; set; }

		public PreAuthorisationView (IPaymentService paymentService) : base ("PreAuthorisationView", null)
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
				if (KeyboardVisible) {
					DismissKeyboardAction ();
				}
			});

			tapRecognizer.NumberOfTapsRequired = 1;
			tapRecognizer.NumberOfTouchesRequired = 1;

			EncapsulatingView.AddGestureRecognizer (tapRecognizer);

			RegisterButton.SetTitleColor (UIColor.White, UIControlState.Application);

			RegisterButton.TouchUpInside += (sender, ev) => {
				PreAuthCard ();
			};

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				FormClose.TouchUpInside += (sender, ev) => {
					this.DismissViewController (true, null);
				};
			}
			RegisterButton.Disable();

		}

		private void OnKeyboardNotification (NSNotification notification)
		{
			KeyboardVisible = notification.Name == UIKeyboard.WillShowNotification;

			if (!KeyboardVisible) {
				if (moveViewUp) {
					ScrollTheView (false);
				}
			}
		}

		private void KeyBoardUpNotification (NSNotification notification)
		{

			CGRect r = UIKeyboard.BoundsFromNotification (notification);

			if (avsCell.PostcodeTextFieldOutlet.IsFirstResponder)
				activeview = avsCell.PostcodeTextFieldOutlet;
			if (activeview != null && !detailCell.HasFocus ()) {
				moveViewUp = true;
				ScrollTheView (moveViewUp);

			}
		}


		private void ScrollTheView (bool move)
		{
			if (move) {
				TableView.SetContentOffset (new CoreGraphics.CGPoint (0, 100f), true);
				TableView.ScrollEnabled = false;
			} else {
				TableView.SetContentOffset (new CoreGraphics.CGPoint (0, 0), true);

			}

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			detailCell.ccTextOutlet.BecomeFirstResponder ();
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
			enable = detailCell.CompletelyDone;

			List<CardCell> cellsToRemove = new List<CardCell> ();
			List<CardCell> insertedCells = new List<CardCell> ();
			List<CardCell> cellsBeforeUpdate = CellsToShow.ToList ();
			if (enable) {
				bool ccIsFirstResponder = detailCell.ccTextOutlet.IsFirstResponder;
				int row = CellsToShow.IndexOf (detailCell) + 1;

				if (JudoSDKManager.AVSEnabled) {
					if (!CellsToShow.Contains (avsCell)) {
						TableView.BeginUpdates ();
						CellsToShow.Insert (row, avsCell);
						row++;// incrementing the row incase an avs cell is also needed;
						insertedCells.Add (avsCell);
						avsCell.PostcodeTextFieldOutlet.BecomeFirstResponder ();
						ccIsFirstResponder = false;


						TableView.InsertRows (new NSIndexPath[]{ NSIndexPath.FromRowSection (CellsToShow.IndexOf (avsCell), 0) }, UITableViewRowAnimation.Fade);
						TableView.EndUpdates ();
					}

				}
					if (detailCell.Type == CardType.MAESTRO && JudoSDKManager.MaestroAccepted) {
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
				if (JudoSDKManager.MaestroAccepted) {
					if (CellsToShow.Contains (maestroCell)) {
						cellsToRemove.Add (maestroCell);
					}
				}

				if (JudoSDKManager.AVSEnabled) {
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


			RegisterButton.Enabled = enable;
			RegisterButton.Alpha = (enable == true ? 1f : 0.25f);

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

		private void PreAuthCard ()
		{
			try {

				JudoSDKManager.ShowLoading (this.View);
				authorisationModel.Card = GatherCardDetails ();

				RegisterButton.Disable();

                _paymentService.PreAuthoriseCard(authorisationModel).ContinueWith(HandleResponse);
                
		    }
            catch (Exception ex)
            {
                JudoSDKManager.HideLoading();
                // Failure callback
                if (failureCallback != null)
                {
                    var judoError = new JudoError { Exception = ex };
                    failureCallback(judoError);
                }
            }


		}

        private void HandleResponse(Task<IResult<ITransactionResult>> reponse)
	    {
            try
            {
                var result = reponse.Result;
                if (result != null && !result.HasError && result.Response.Result != "Declined")
                {
                    var paymentreceipt = result.Response as PaymentReceiptModel;

                    if (paymentreceipt != null)
                    {
                        // call success callback
                        if (successCallback != null) successCallback(paymentreceipt);
                    }
                    else
                    {
                        throw new Exception("JudoXamarinSDK: unable to find the receipt in response.");
                    }

                }
                else
                {
                    // Failure callback
                    if (failureCallback != null)
                    {
                        var judoError = new JudoError {ApiError = result != null ? result.Error : null};
                        var paymentreceipt = result != null ? result.Response as PaymentReceiptModel : null;

                        if (paymentreceipt != null)
                        {
                            // send receipt even we got card declined
                            failureCallback(judoError, paymentreceipt);
                        }
                        else
                        {
                            failureCallback(judoError);
                        }
                    }

                }
                JudoSDKManager.HideLoading();
            }
			 catch (Exception ex) {
				JudoSDKManager.HideLoading ();
				// Failure callback
				if (failureCallback != null) {
					var judoError = new JudoError { Exception = ex };
					failureCallback (judoError);
				}
			}
	    }

	    void CleanOutCardDetails ()
		{
			detailCell.CleanUp ();

			if (JudoSDKManager.MaestroAccepted) {
				maestroCell.CleanUp ();

			}	
			if (JudoSDKManager.AVSEnabled) {
				avsCell.CleanUp ();
			}
		}

		CardViewModel GatherCardDetails ()
		{
			CardViewModel cardViewModel = new CardViewModel ();
			detailCell.GatherCardDetails (cardViewModel);


			if (JudoSDKManager.AVSEnabled) {
				avsCell.GatherCardDetails (cardViewModel);

			}

			if (detailCell.Type == CardType.MAESTRO) {
				maestroCell.GatherCardDetails (cardViewModel);

			}

			return cardViewModel;
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			this.View.Hidden=true;

		}


	}
}

