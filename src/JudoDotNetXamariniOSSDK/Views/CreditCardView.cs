﻿using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CoreFoundation;
using JudoPayDotNet.Models;
using CoreGraphics;

namespace JudoDotNetXamariniOSSDK
{
	public partial class CreditCardView : UIViewController
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

        public SuccessCallback successCallback { private get; set; }
        public FailureCallback failureCallback { private get; set; }
	    public PaymentViewModel cardPayment { get; set; }

	    readonly IErrorPresenter errorPresenter;


		public CreditCardView (IPaymentService paymentService) : base ("CreditCardView", null)
		{
			_paymentService = paymentService;
			errorPresenter = new ResponseErrorPresenter ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			this.View.BackgroundColor = UIColor.FromRGB (245f, 245f, 245f);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			SetUpTableView ();

			this.View.BackgroundColor = UIColor.FromRGB (245f, 245f, 245f);


			NSNotificationCenter defaultCenter = NSNotificationCenter.DefaultCenter;
			defaultCenter.AddObserver (UIKeyboard.WillHideNotification, OnKeyboardNotification);
			defaultCenter.AddObserver (UIKeyboard.WillShowNotification, OnKeyboardNotification);
			defaultCenter.AddObserver (UIKeyboard.DidShowNotification, KeyBoardUpNotification);

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
			SubmitButton.Enabled = false;
			SubmitButton.Alpha = 0.25f;
		}

		private void OnKeyboardNotification (NSNotification notification)
		{
			_keyboardVisible = notification.Name == UIKeyboard.WillShowNotification;

			if (!_keyboardVisible) {
				if(_moveViewUp){ScrollTheView(false);}
			}
		}

		private void KeyBoardUpNotification(NSNotification notification)
		{

			CGRect r = UIKeyboard.BoundsFromNotification (notification);

			if (avsCell.PostcodeTextFieldOutlet.IsFirstResponder)
				_activeview = avsCell.PostcodeTextFieldOutlet;
			if (_activeview != null) {
				_moveViewUp = true;
				ScrollTheView (_moveViewUp);

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

		}

		void DismissKeyboardAction ()
		{
			detailCell.DismissKeyboardAction();
			avsCell.DismissKeyboardAction();
			maestroCell.DismissKeyboardAction();
		}

		private void UpdateUI ()
		{
			bool enable = false;
			enable = detailCell.EntryComplete();

			List<CardCell> cellsToRemove = new List<CardCell> ();
			List<CardCell> insertedCells = new List<CardCell> ();
			List<CardCell> cellsBeforeUpdate = CellsToShow.ToList ();
			TableView.BeginUpdates ();

			if (enable) {
				bool ccIsFirstResponder = detailCell.ccTextOutlet.IsFirstResponder;

				if (detailCell.Type == CreditCardType.Maestro && JudoSDKManager.MaestroAccepted) {
					if (!CellsToShow.Contains (maestroCell)) {
						int row = CellsToShow.IndexOf (detailCell) + 1;
						CellsToShow.Insert (row, maestroCell);

						insertedCells.Add (maestroCell);
					}

					if (maestroCell.IssueNumberTextFieldOutlet.Text.Length == 0 || maestroCell.StartDateTextFieldOutlet.Text.Length != 5) {
						enable = false;
					}

					if (ccIsFirstResponder) {
						maestroCell.StartDateTextFieldOutlet.BecomeFirstResponder ();
						ccIsFirstResponder = false;
					}
				}

				if (JudoSDKManager.AVSEnabled) {
					if (!CellsToShow.Contains (avsCell)) {
						int row = CellsToShow.IndexOf (reassuringCell);
						CellsToShow.Insert (row, avsCell);

						insertedCells.Add (avsCell);
					}

					if (ccIsFirstResponder) {
						avsCell.PostcodeTextFieldOutlet.BecomeFirstResponder ();
						ccIsFirstResponder = false;
					}
				}

				if (ccIsFirstResponder) {
					DismissKeyboardAction ();

					ccIsFirstResponder = false;
				}
			} else {
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
			}
			List<NSIndexPath> indexPathsToRemove = new List<NSIndexPath> ();

			foreach (CardCell cell in cellsToRemove) {
				indexPathsToRemove.Add (NSIndexPath.FromRowSection (cellsBeforeUpdate.IndexOf (cell), 0));
			}

			TableView.DeleteRows (indexPathsToRemove.ToArray (), UITableViewRowAnimation.Fade);

			foreach (CardCell cell in cellsToRemove) {
				CellsToShow.Remove (cell);
			}


			List<NSIndexPath> indexPathsToAdd = new List<NSIndexPath> ();

			foreach (CardCell cell in insertedCells) {
				indexPathsToAdd.Add (NSIndexPath.FromRowSection (CellsToShow.IndexOf (cell), 0));
			}

			TableView.InsertRows (indexPathsToAdd.ToArray (), UITableViewRowAnimation.Fade);
			TableView.EndUpdates ();
			SubmitButton.Enabled = enable;
			SubmitButton.Alpha = (enable == true ? 1f : 0.25f) ;

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

		private void MakePayment ()
		{
		    try
		    {
                cardPayment.Card = GatherCardDetails();

                SubmitButton.Enabled = false;
                SubmitButton.Alpha = 0.25f;
                _paymentService.MakePayment(cardPayment).ContinueWith(reponse =>
                {
                    var result = reponse.Result;
                    if (result != null && !result.HasError && result.Response.Result != "Declined")
                    {
                        PaymentReceiptModel paymentreceipt = result.Response as PaymentReceiptModel;
                        PaymentReceiptViewModel receipt = new PaymentReceiptViewModel()
                        {
                            CreatedAt = paymentreceipt.CreatedAt.DateTime,
                            Currency = paymentreceipt.Currency,
                            OriginalAmount = paymentreceipt.Amount,
                            ReceiptId = paymentreceipt.ReceiptId,
                            Message = "Payment Success"
                        };
                        JudoConfiguration.Instance.CardToken = paymentreceipt.CardDetails.CardToken;
                        JudoConfiguration.Instance.TokenCardType = cardPayment.Card.CardType;
                        JudoConfiguration.Instance.ConsumerToken = paymentreceipt.Consumer.ConsumerToken;
                        JudoConfiguration.Instance.LastFour = cardPayment.Card.CardNumber.Substring(cardPayment.Card.CardNumber.Length - Math.Min(4, cardPayment.Card.CardNumber.Length));

                        DispatchQueue.MainQueue.DispatchAfter(DispatchTime.Now, () =>
                        {
                            SubmitButton.Alpha = 0.25f;
                            SubmitButton.Enabled = false;
                            CleanOutCardDetails();
                        });

                        // call success callback
                        if (successCallback != null) successCallback(receipt);

                    }
                    else
                    {
                        DispatchQueue.MainQueue.DispatchAfter(DispatchTime.Now, () =>
                        {
                            errorPresenter.DisplayError(result, "Payment has failed");
                            SubmitButton.Enabled = true;
                            SubmitButton.Alpha = 1f;
                        });

                        // Failure callback
                        if (failureCallback != null)
                        {
                            var judoError = new JudoError { ApiError = result != null ? result.Error: null };
                            failureCallback(judoError);
                        }
                    }
                });
		    }
		    catch (Exception ex)
		    {
                // Failure callback
                if (failureCallback != null)
                {
                    var judoError = new JudoError { Exception = ex};
                    failureCallback(judoError);
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

			if (detailCell.Type == CreditCardType.Maestro) {
				maestroCell.GatherCardDetails (cardViewModel);

			}

			return cardViewModel;
		}


	}
}

