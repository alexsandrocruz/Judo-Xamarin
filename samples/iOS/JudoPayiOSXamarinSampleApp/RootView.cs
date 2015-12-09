using System;

using Foundation;
using UIKit;
using JudoDotNetXamariniOSSDK;
using System.Drawing;
using System.Collections.Generic;
using CoreFoundation;
using CoreGraphics;
using JudoPayDotNet.Models;
using PassKit;

namespace JudoPayiOSXamarinSampleApp
{
	public partial class RootView : UIViewController
	{
		SlideUpMenu _menu;

		//keep this detail from last transaction
		private string cardToken;
		private string consumerToken;
		private string lastFour;
		private CardType cardType;

		private string paymentReference = "payment101010102";
		private string consumerRef = "consumer1010102";
		private const string cardNumber = "4976000000003436";
		private const string addressPostCode = "TR14 8PA";
		private const string startDate = "";
		private  const string expiryDate = "12/15";
		private const string cv2 = "452";

		public RootView ()
			: base ("RootView", null)
		{

		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			SetUpTableView ();

			UILabel label = new UILabel (new CGRect (0, 0, 120f, 30f));
			label.TextAlignment = UITextAlignment.Center;
			label.Font = UIFont.FromName ("Courier", 17.0f);
			label.BackgroundColor = UIColor.Clear;

			label.Text = "Judo Sample App";
			this.NavigationController.NavigationBar.TopItem.TitleView = label;

		}

		private void ShowMessage (string title, string message, string btnText = "OK")
		{
			UIAlertView msgbox = new UIAlertView (title, message, null, btnText, null);
			msgbox.Show ();
		}

		private void SuccessPayment (PaymentReceiptModel receipt)
		{
			cardToken = receipt.CardDetails.CardToken;
			consumerToken = receipt.Consumer.ConsumerToken;
			lastFour = receipt.CardDetails.CardLastfour;
			cardType = receipt.CardDetails.CardType;
			DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
				// move back to home screen
				CloseView ();

  
				// show receipt
				ShowMessage ("Transaction Successful", "Receipt ID - " + receipt.ReceiptId);

				// store token to further use
			});
		}

		private void FailurePayment (JudoError error, PaymentReceiptModel receipt)
		{
			DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
				// move back to home screen
				CloseView ();
				// show receipt
				string message = "";
				if (error != null && error.ApiError != null)
					message += error.ApiError.ErrorMessage + Environment.NewLine;

				if (error != null && error.Exception != null)
					message += error.Exception.Message + Environment.NewLine;

				if (receipt != null) {
					message += "Transaction : " + receipt.Result + Environment.NewLine;
					message += receipt.Message + Environment.NewLine;
					message += "Receipt ID - " + receipt.ReceiptId;
				}

				ShowMessage ("Transaction Failed: ", message);
				// store token to further use
			});
		}

		void SetUpTableView ()
		{
			Dictionary<string, Action> buttonDictionary = new Dictionary<string, Action> ();
			SuccessCallback successCallback = SuccessPayment;
			FailureCallback failureCallback = FailurePayment;

			var tokenPayment = new TokenPaymentViewModel () {
				Amount = 3.5m,
				ConsumerReference = consumerRef,
				PaymentReference = paymentReference,
				CV2 = cv2
			};

			buttonDictionary.Add ("Make a Payment", delegate {
				JudoSDKManager.Payment (GetCardViewModel (), successCallback, failureCallback, this.NavigationController);
			});



			buttonDictionary.Add ("PreAuthorise", delegate {
				JudoSDKManager.PreAuth (GetCardViewModel (), successCallback, failureCallback, this.NavigationController);
			});


			buttonDictionary.Add ("Token Payment", delegate {
				tokenPayment.Token = cardToken;
				tokenPayment.ConsumerToken = consumerToken;
				tokenPayment.LastFour = lastFour;
				tokenPayment.CardType = cardType;

				JudoSDKManager.TokenPayment (tokenPayment, successCallback, failureCallback, this.NavigationController);
			});

			buttonDictionary.Add ("Token PreAuthorise", delegate {
				tokenPayment.Token = cardToken;
				tokenPayment.ConsumerToken = consumerToken;
				tokenPayment.LastFour = lastFour;
				tokenPayment.CardType = cardType;

				JudoSDKManager.TokenPreAuth (tokenPayment, successCallback, failureCallback, this.NavigationController);
			});

			buttonDictionary.Add ("Register a Card", delegate {
				JudoSDKManager.RegisterCard (GetCardViewModel (), successCallback, failureCallback, this.NavigationController);
			});
			if (JudoSDKManager.ApplePayAvailable) {

				buttonDictionary.Add ("Make a ApplePay Payment", delegate {
					JudoSDKManager.MakeApplePayment (GetApplePayViewModel (), successCallback, failureCallback, this.NavigationController);
				});

				buttonDictionary.Add ("Make a ApplePay PreAuthorise", delegate {
					JudoSDKManager.MakeApplePreAuth (GetApplePayViewModel (), successCallback, failureCallback, this.NavigationController);
				});

			}

			MainMenuSource menuSource = new MainMenuSource (buttonDictionary);
			ButtonTable.Source = menuSource;
			TableHeightConstrant.Constant = menuSource.GetTableHeight () + 60f;
		}

		/// <summary>
		/// just for sample app, you can set all settings while configuring SDK
		/// </summary>
		/// <param name="animated"></param>
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			_menu = new SlideUpMenu (new RectangleF (0, (float)this.View.Frame.Bottom - 40f, (float)this.View.Frame.Width, 448f));
			_menu.AwakeFromNib ();
			_menu.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			this.View.AddSubview (_menu);
		}

		PaymentViewModel GetCardViewModel ()
		{
			var cardPayment = new PaymentViewModel {
				Amount = 4.5m, 
				ConsumerReference = consumerRef,
				PaymentReference = paymentReference,
				Currency = "GBP",
				// Non-UI API needs to pass card detail
				Card = new CardViewModel {
					CardNumber = cardNumber,
					CV2 = cv2,
					ExpireDate = expiryDate,
					PostCode = addressPostCode,
					CountryCode = ISO3166CountryCodes.UK
				}
			};
			return cardPayment;
		}

		ApplePayViewModel GetApplePayViewModel ()
		{
			var summaryItems = new PKPaymentSummaryItem[] {
				new PKPaymentSummaryItem () {
					Amount = new NSDecimalNumber ("0.90"),
					Label = @"Judo Burrito"

				},
				new PKPaymentSummaryItem () {
					Amount = new NSDecimalNumber ("0.10"),
					Label = @"Extra Guac"

				}
			};
			
			var applePayment = new ApplePayViewModel {
				
				CurrencyCode = new NSString ("GBP"),
				CountryCode = new NSString (@"GB"),
				SupportedNetworks = new NSString[3]{ new NSString ("Visa"), new NSString ("MasterCard"), new NSString ("Amex") },
				SummaryItems = summaryItems,
				TotalSummaryItem = new PKPaymentSummaryItem () {
					Amount = new NSDecimalNumber ("1.00"),
					Label = @"El Judorito"

				},
				ConsumerRef=new NSString (@"GenerateYourOwnCustomerRefHere"),
				MerchantIdentifier = new NSString ("merchant.com.judo.Xamarin")
			};
			return applePayment;
		}

		public override void ViewWillDisappear (bool animated)
		{
			_menu.RemoveFromSuperview ();
			base.ViewWillDisappear (animated);
		}

		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			_menu.ResetMenu ();
		}

		void CloseView ()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				NavigationController.DismissViewController (true, null);
			} else {
				NavigationController.PopToRootViewController (true);
			}
		}
	}
}

