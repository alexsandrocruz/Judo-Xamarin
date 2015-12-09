using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
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
	public partial class CardEntryCell : CardCell
	{
		public static readonly UINib Nib = UINib.FromName ("CardEntryCell", NSBundle.MainBundle);

		public UITextView ccTextOutlet { get { return ccText; } }

		int currentYear;

		CreditCard cardHelper = new CreditCard ();
		public CardType Type {get; set;}

		bool flashForError = false;
		bool updateText = false;
		bool scrollForward = false;

		int numberLength;

		UIImageView ccBackImage;
		UIImageView ccImage;

		string ccv;
		string creditCardNum;
		bool deleting = false;
		bool ret = false;
		int year;
		bool hasFullNumber = false;


		int cardMonth;

		public bool CompletelyDone {
			get;
			set;
		}

		bool deletedSpace = false;

		public CardEntryCell (IntPtr handle) : base (handle)
		{
			Key = "CardEntryCell";
		}

		public override CardCell Create ()
		{
			return (CardEntryCell)Nib.Instantiate (null, null) [0];
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

		}

		public override void SetUpCell ()
		{

			creditCardImage.Tag = (int)CardType.UNKNOWN;

			creditCardImage.Layer.CornerRadius = 4.0f;
			creditCardImage.Layer.MasksToBounds = true;

			UIImage image = ThemeBundleReplacement.BundledOrReplacementImage ("ic_card_large_unknown", BundledOrReplacementOptions.BundledOrReplacement);

			creditCardImage.Image = image;

			currentYear = DateTime.Now.Year - 2000;

			CALayer layer = containerView.Layer;
			layer.CornerRadius = 4.0f;
			layer.MasksToBounds = true;
			layer.BorderColor = ColourHelper.GetColour ("0xC3C3C3FF").CGColor; 
			layer.BorderWidth = 1;
			layer = textScroller.Layer;
			layer.CornerRadius = 4.0f;
			layer.MasksToBounds = true;
			layer.BorderWidth = 0;

			textScroller.SetContentOffset (new PointF (0, 0), true);
			textScroller.ScrollEnabled = false;

			SetupPlaceViews ();


            Type = CardType.UNKNOWN;
			SetUpMaskedInput ();


			ExpiryInfoButton.TouchUpInside += (sender, ev) => {
				PushExpiryInfoView ();
			};

		}

		void SetupPlaceViews ()
		{
  
			ccText.Text = "11112222333344445555";

			UITextPosition start = ccText.BeginningOfDocument;
			UITextPosition end = ccText.GetPosition (start, 20);
			UITextRange range = ccText.GetTextRange (start, end);
			RectangleF r = ccText.GetFirstRectForRange (range);
			SizeF frameRect = r.Size;
			frameRect.Width = (r.Size.Width / 24.0f);
			ccText.Font = JudoSDKManager.FIXED_WIDTH_FONT_SIZE_20;
			r.Size = frameRect;
			ccText.Text = "";


			RectangleF frame = ccPlaceHolder.Frame;
			ccPlaceHolder.Font = JudoSDKManager.FIXED_WIDTH_FONT_SIZE_20;
			ccPlaceHolder.Text = "0000 0000 0000 0000";

			ccPlaceHolder.SetShowTextOffSet (0);
			ccPlaceHolder.Offset = r;

			ccPlaceHolder.BackgroundColor = UIColor.Clear;
			textScroller.InsertSubview (ccPlaceHolder, 0);

			/////////

			expiryText.Text = "MM/YY";

			UITextPosition exStart = expiryText.BeginningOfDocument;
			UITextPosition exEnd = expiryText.GetPosition (exStart, 5);
			UITextRange exRange = expiryText.GetTextRange (exStart, exEnd);
			RectangleF exR = expiryText.GetFirstRectForRange (exRange);
			SizeF exFrameRect = exR.Size;
			exFrameRect.Width = (exR.Size.Width / 24.0f);
			expiryText.Font = JudoSDKManager.FIXED_WIDTH_FONT_SIZE_20;
			exR.Size = exFrameRect;
			expiryText.Text = "";


	
			RectangleF expiryFrame = expiryPlaceHolder.Frame;
			expiryPlaceHolder.Font = expiryText.Font;
			expiryPlaceHolder.Text = "MM/YY";

			expiryPlaceHolder.SetShowTextOffSet (0);
			expiryPlaceHolder.Offset = exR;

			expiryPlaceHolder.BackgroundColor = UIColor.Clear;
			textScroller.InsertSubview (expiryPlaceHolder, 1);

			////

			cvTwoText.Text = "CV2";

			UITextPosition cvStart = cvTwoText.BeginningOfDocument;
			UITextPosition cvEnd = cvTwoText.GetPosition (cvStart, 3);
			UITextRange cvRange = cvTwoText.GetTextRange (cvStart, cvEnd);
			RectangleF cvR = cvTwoText.GetFirstRectForRange (cvRange);
			SizeF cvFrameRect = cvR.Size;
			cvFrameRect.Width = (cvR.Size.Width / 24.0f);
			cvTwoText.Font = JudoSDKManager.FIXED_WIDTH_FONT_SIZE_20;
			cvR.Size = cvFrameRect;
			cvTwoText.Text = "";

			RectangleF cvTwoFrame = cvTwoPlaceHolder.Frame;
			cvTwoPlaceHolder.Font = cvTwoText.Font;
			cvTwoPlaceHolder.Text = "CV2";

			cvTwoPlaceHolder.SetShowTextOffSet (0);
			cvTwoPlaceHolder.Offset = cvR;

			cvTwoPlaceHolder.BackgroundColor = UIColor.Clear;
			textScroller.InsertSubview (cvTwoPlaceHolder, 2);
		}

		void PushExpiryInfoView ()
		{
			var alertText = "";
			if (creditCardImage != ccBackImage) {
				alertText = string.Format (@"MM/YY: {0}\n\n CV2: {1}", "The month and year your card expires", "The security code printed on the signature strip on the back of your card");
			} else {
				alertText = string.Format (@"CV2: {0}", "The security code printed on the signature strip on the back of your card");
			}
			UIAlertView alert = new UIAlertView ("Expiry Info", alertText, null, "OK", null);
			alert.Show ();
		}

		void SetUpMaskedInput ()
		{
			SetupCC ();
			SetupExpire ();
			SetupCVTwo ();
		}

		void SetupCC ()
		{
			ccText.ShouldChangeText = (UITextView textView, NSRange NSRange, string replace) => {
				if(replace!=""&&!Char.IsDigit(replace.ToCharArray()[0]))
				{
					return false;
				}
				CSRange range = new CSRange ((int)NSRange.Location, (int)NSRange.Length);
				var formattedText = "";
				flashForError = false;
				updateText = false;
				scrollForward = false;
				deleting = false;
				ret = false;
				deletedSpace = false;
				cardMonth = 0;

				if (replace.Length == 0) {
					updateText = true;
					deleting = true;
					if (textView.Text.Length != 0) {	// handle case of delete when there are no characters left to delete

						char c = textView.Text.Substring (range.Location, 1).ToCharArray () [0];
						if (range.Location != 0 && range.Length == 1 && (c == ' ' || c == '/')) {
							range.Location--;
							range.Length++;
							deletedSpace = true;
						}
					} else {
						return false;
					}

				}

				string newTextOrig =ReplaceInPlace(range.Location, range.Length,textView.Text,replace);

				int newTextLen = newTextOrig.Length;

				// causes the cc entry field to scroll back if the user deletes back beyond the end of the cc number
				if (range.Location <= numberLength) {
					hasFullNumber = false;
				}

				if (textScroller.ContentOffset.X < 10f) {
					ccPlaceHolderWidthConstraint.Constant = 198f;

				}
				if (!hasFullNumber) {

					// scrolls backward
					int textViewLen = ccText.Text.Length; 
					int formattedLen = ccPlaceHolder.Text.Length;
				

					if (textScroller.ContentOffset.X != 0) {
						textScroller.SetContentOffset (new PointF (0, 0), true);
					}

					UpdateCCimageWithTransitionTime (0, false, true); 

					StatusHelpLabel.Text = "Enter Card Details";

					string newText = newTextOrig.Replace (" ", String.Empty);
					int len = newText.Length;
					if (len < Card.CC_LEN_FOR_TYPE) {
						updateText = true;
						formattedText = newTextOrig;

						Type = CardType.UNKNOWN;
					} else {
						Type = cardHelper.GetCCType (newText);
						switch (Type) {

						case CardType.UNKNOWN:
							flashForError = true;
							break;
						case CardType.MAESTRO:
							if (!JudoSDKManager.MaestroAccepted) {

								flashForError = true;
								return EndDelegate (ccPlaceHolder, ccText, formattedText);
							}
							break;
						case CardType.AMEX:
							if (!JudoSDKManager.AmExAccepted) {

								flashForError = true; 
								return EndDelegate (ccPlaceHolder, ccText, formattedText);
							}
							break;
						}

						if (len == Card.CC_LEN_FOR_TYPE) {
							ccPlaceHolder.Text = cardHelper.PromptStringForType (Type, true);
							cvTwoPlaceHolder.Text = cardHelper.CVTwoPromptForType (Type, true);
							cvTwoPlaceHolder.SetShowTextOffSet (Math.Min (0, 0));


						}

						formattedText = cardHelper.FormatForViewing (newText); 
						int lenForCard = cardHelper.LengthOfStringForType (Type); 

						if (len < lenForCard) {
							updateText = true;
						} else if (len == lenForCard) {
							if (cardHelper.isValidNumber (newText)) {
								if (cardHelper.IsLuhnValid (newText)) {
									numberLength = cardHelper.LengthOfFormattedStringForType (Type);
									creditCardNum = newText;

									updateText = true;
									scrollForward = true;
									hasFullNumber = true;
								} else {
									FlashRecheckNumberMessage ();
								}
							} else {
								FlashRecheckNumberMessage ();
							}	
						}
					}
					UpdateCCimageWithTransitionTime (0.25f); 
				}

				return EndDelegate (ccPlaceHolder, ccText, formattedText);

			};
		}

		void SetupExpire ()
		{

			expiryText.ShouldChangeText = (UITextView textView, NSRange NSRange, string replace) => {
				if(replace!=""&&!Char.IsDigit(replace.ToCharArray()[0]))
				{
					return false;
				}
				CSRange range = new CSRange ((int)NSRange.Location, (int)NSRange.Length);
				flashForError = false;
				updateText = false;
				scrollForward = false;
				deleting = false;
				ret = false;
				deletedSpace = false;

				if (replace.Length == 0) {
					updateText = true;
					deleting = true;


					if (textView.Text.Length != 0 && (range.Length != 0)) {	// handle case of delete when there are no characters left to delete

						char c = textView.Text.Substring (range.Location, 1).ToCharArray () [0];
						if (range.Location != 0 && range.Length == 1 && (c == ' ' || c == '/')) {
							range.Location--;
							range.Length++;
							deletedSpace = true;
						}
					} else {
						ccText.BecomeFirstResponder ();

						if (ccText.Text.Length == (cardHelper.LengthOfFormattedStringForType (Type))) {
							ccText.Text = ccText.Text.Remove (ccText.Text.Length - 1);
						}


						return EndDelegate (ccPlaceHolder, ccText, ccText.Text);
					}
				}

				var formattedText = "";
				scrollForward = false;

				string newTextOrig =ReplaceInPlace(range.Location, range.Length,textView.Text,replace);

				int newTextLen = newTextOrig.Length;

				// Test for delete of a space or /
				if (deleting) {
					formattedText = newTextOrig.Substring (0, range.Location);
					updateText = true;
					return EndDelegate (expiryPlaceHolder, expiryText, formattedText);
				}

				if (newTextLen > expiryPlaceHolder.Text.Length) {
					flashForError = true;
					return EndDelegate (expiryPlaceHolder, expiryText, formattedText);
				}

				formattedText = newTextOrig;

				CSRange monthRange = new CSRange (expiryPlaceHolder.Text.IndexOf ("MM"), 2);
				if (newTextLen > monthRange.Location) {
					if (newTextOrig.Substring (monthRange.Location, 1).ToCharArray () [0] > '1') {
						// support short cut - we prepend a '0' for them


						 formattedText =ReplaceInPlace(range.Location, range.Length,textView.Text,"0"+replace);

						newTextLen = newTextOrig.Length;
					}
					if (newTextLen >= (monthRange.Location + monthRange.Length)) {
						cardMonth = Int32.Parse (newTextOrig.Substring (monthRange.Location, monthRange.Length));
						if (cardMonth < 1 || cardMonth > 12) {
							flashRecheckExpiryDateMessage ();
							return EndDelegate (expiryPlaceHolder, expiryText, formattedText);
						}
					}
				}

				CSRange yearRange = new CSRange (expiryPlaceHolder.Text.IndexOf ("YY"), 2);
				if (newTextLen > yearRange.Location) {
					int proposedDecade = (newTextOrig.Substring (yearRange.Location, 1).ToCharArray () [0] - '0') * 10;
					int yearDecade = currentYear - (currentYear % 10);
					if (proposedDecade < yearDecade) {
						flashRecheckExpiryDateMessage ();
						return EndDelegate (expiryPlaceHolder, expiryText, formattedText);
					}
					if (newTextLen >= (yearRange.Location + yearRange.Length)) {
						year = Int32.Parse (newTextOrig.Substring (yearRange.Location, yearRange.Length)); 
						int diff = year - currentYear;
						if (diff < 0 || diff > 10) {	
							flashRecheckExpiryDateMessage ();
							return EndDelegate (expiryPlaceHolder, expiryText, formattedText);
						}
						if (diff == 0) { 

							var todaysDate = DateTime.Today;
							int currentMonth = todaysDate.Month;

							if (cardMonth < currentMonth) {
								flashRecheckExpiryDateMessage ();
								return EndDelegate (expiryPlaceHolder, expiryText, formattedText);
							}
						}
						if (creditCardImage != ccBackImage) {
							UIViewAnimationOptions transType = (Type == CardType.AMEX) ? UIViewAnimationOptions.TransitionCrossDissolve : UIViewAnimationOptions.TransitionFlipFromBottom;

							UIImageView.Animate (
								duration: 0.25f, 
								delay: 0,
								options: transType,
								animation: () => {
									creditCardImage = ccBackImage;
								},
								completion: () => {
									StatusHelpLabel.Text = "Please enter CV2";
								});
						}
					}
				}
				updateText = true;
				if (formattedText.Length == 5) {
					cvTwoText.BecomeFirstResponder ();
				} 
				return EndDelegate (expiryPlaceHolder, expiryText, formattedText);
			};

		}

		private string ReplaceInPlace(int location,int length,string originalText,string replace)
		{
			var aStringBuilder = new StringBuilder (originalText);
			aStringBuilder.Remove (location, length);
			aStringBuilder.Insert (location, replace);
			return aStringBuilder.ToString ();
		}

		void SetupCVTwo ()
		{
			cvTwoText.ShouldChangeText = (UITextView textView, NSRange NSRange, string replace) => {

				if(replace!=""&&!Char.IsDigit(replace.ToCharArray()[0]))
				{
					return false;
				}
				CSRange range = new CSRange ((int)NSRange.Location, (int)NSRange.Length);
				if (replace.Length == 0) {

					if (textView.Text.Length == 0 || range.Location == 0) {	// handle case of delete when there are no characters left to delete
						expiryText.BecomeFirstResponder ();

						if (expiryText.Text.Length == 5) {
							expiryText.Text = expiryText.Text.Remove (expiryText.Text.Length - 1);
						}
				
					}

				}

				if (cvTwoText.Text.Length== (Type == CardType.AMEX ? 4 : 3)&& replace.Length != 0) 
                {
					return false;
				}


				scrollForward = false;

				string newTextOrig = ReplaceInPlace(range.Location, range.Length,textView.Text,replace);

				int newTextLen = newTextOrig.Length;

				if (newTextLen == cvTwoPlaceHolder.Text.Length) {

					var cIndex = cvTwoPlaceHolder.Text.IndexOf ("C");
					CSRange ccvRange = new CSRange (cIndex, cvTwoPlaceHolder.Text.Substring (cIndex).Length);
					ccvRange.Length = Type == CardType.AMEX ? 4 : 3;
					ccv = newTextOrig.Substring (ccvRange.Location, ccvRange.Length);
				}

				updateText = true;
				if (newTextOrig.Length== (Type == CardType.AMEX ? 4 : 3)) {
					DismissKeyboardAction ();
				} 
				return EndDelegate (cvTwoPlaceHolder, cvTwoText, newTextOrig);
			};
		}

		void UpdateCCimageWithTransitionTime (float transittionTime, bool isBack = false, bool force = false)
		{
			if (creditCardImage.Tag != (int)Type || force == true) {

				UIImage frontImage = cardHelper.CreditCardImage (Type);
				ccImage = new UIImageView (frontImage);
				ccImage.Frame = creditCardImage.Frame;
				ccImage.Tag = (int)Type;

				ccBackImage = new UIImageView (cardHelper.CreditCardBackImage (Type));
				ccBackImage.Frame = creditCardImage.Frame;
				ccBackImage.Tag = (int)Type;

				var finalImage = new UIImageView ();
				if (isBack) {
					finalImage = ccBackImage;
				} else {
					finalImage = ccImage;
				}
			
				if (!creditCardImage.Image.AsPNG().IsEqual( finalImage.Image.AsPNG())) {

					UIView.Transition (creditCardImage, finalImage, transittionTime, UIViewAnimationOptions.TransitionFlipFromLeft, null);

					creditCardImage = finalImage;
				}
			}
		}

		private bool EndDelegate (PlaceHolderTextView placeView, UITextView textview, string formattedText)
		{

			if (scrollForward && textScroller.ContentOffset.X < 50f) {
				ScrollForward (true);
				DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
					textScroller.FlashScrollIndicators ();
				});
			} 
			if (updateText) {
				int textViewLen = formattedText.Length;
				int formattedLen = placeView.Text.Length;

				if ((formattedLen > textViewLen) && !deleting) {
					char c = placeView.Text.Substring (textViewLen, 1).ToCharArray () [0];
					if (c == ' ')
						formattedText = formattedText + " ";
					else if (c == '/')
						formattedText = formattedText + "/"; 

					textViewLen = formattedText.Length;
					formattedLen = placeView.Text.Length;

				}
				placeView.SetShowTextOffSet (Math.Min (textViewLen, formattedLen));
				if (!deleting || hasFullNumber || deletedSpace) {
					textview.Text = formattedText;

				} else {
					ret = true;
				}

			}
			if (flashForError) {
				FlashMessage ("Please recheck number");
			}

			if(ccText.Text.Length == cardHelper.LengthOfFormattedStringForType(Type)&&expiryText.Text.Length==5&&cvTwoText.Text.Length==(Type == CardType.AMEX ? 4 : 3))
            {
				CompletelyDone = true;
			} else {
				CompletelyDone = false;
			}
			DispatchQueue.MainQueue.DispatchAsync (() => {

				UpdateUI.Invoke ();
			});
			return ret;
		}

		float widthToLastGroup {
			get { 
				int oldOffset = ccPlaceHolder.ShowTextOffset;
				int offsetToLastGroup = cardHelper.LengthOfFormattedStringTilLastGroupForType (Type);
				ccPlaceHolder.SetShowTextOffSet (offsetToLastGroup);
				float width = ccPlaceHolder.WidthToOffset ();
				ccPlaceHolder.SetShowTextOffSet (oldOffset);
				return width;
			}
		}

		void ScrollForward (bool animated)
		{

			if(Type== CardType.AMEX)
            {
				ccPlaceHolderWidthConstraint.Constant = 178f;

			} else {
				ccPlaceHolderWidthConstraint.Constant = 198f;
			}
			expiryText.BecomeFirstResponder ();
			if (creditCardImage != ccBackImage) {
				StatusHelpLabel.Text = "Please enter Expire Date";
			}
			float width = widthToLastGroup;
			RectangleF frame = new RectangleF (ccText.Frame.Location, new SizeF ((width) + textScroller.Frame.Size.Width, ccText.Frame.Size.Height));
			textScroller.ContentSize = new SizeF (frame.Size.Width, textScroller.ContentSize.Height);
			ccText.Frame = frame;

			ccPlaceHolder.SetText (cardHelper.PromptStringForType (Type, false));

			textScroller.ScrollEnabled = true;
			if (textScroller.ContentOffset != new PointF (width, 0)) {

				textScroller.SetContentOffset (new PointF (width, 0), animated);

			}

			UpdateCCimageWithTransitionTime (0.25f, true, true); 
		}


		public override void DismissKeyboardAction ()
		{			
			ccPlaceHolder.ResignFirstResponder ();
			ccText.ResignFirstResponder ();

			expiryText.ResignFirstResponder ();
			expiryPlaceHolder.ResignFirstResponder ();

			cvTwoPlaceHolder.ResignFirstResponder ();
			cvTwoText.ResignFirstResponder ();
		}

		public void flashRecheckExpiryDateMessage ()
		{
			FlashMessage ("Invalid Expiry Date");
		}

		void FlashRecheckNumberMessage ()
		{
			FlashMessage ("Invalid Card Number");
		}

		void FlashMessage (string message)
		{
			PaymentErrorLabel.Text = message;
			PaymentErrorLabel.Hidden = false;

			DispatchQueue.MainQueue.DispatchAfter (new DispatchTime (DispatchTime.Now, 1 * 1000000000), () => {
				PaymentErrorLabel.Hidden = true;

			});
		}

		public void GatherCardDetails (CardViewModel cardViewModel)
		{
			string _cardMonth = cardMonth.ToString ();
			if (cardMonth < 10) {
				_cardMonth = "0" + _cardMonth;
			}
			cardViewModel.CardNumber = creditCardNum;
			cardViewModel.CV2 = ccv;
			cardViewModel.ExpireDate = _cardMonth + "/" + year;
			cardViewModel.CardType = Type;
		}

		public void CleanUp ()
		{
			SetUpCell ();
	
			DispatchQueue.MainQueue.DispatchAfter (DispatchTime.Now, () => {
				UIImage defaultImage = cardHelper.CreditCardImage (CardType.UNKNOWN);
				creditCardImage.Image = defaultImage;

			});
		}

		public bool EntryComplete ()
		{
			if(ccText.Text.Length == cardHelper.LengthOfFormattedStringForType(Type)&&expiryText.Text.Length==5&&cvTwoText.Text.Length==(Type == CardType.AMEX ? 4 : 3))
            {
				return true;
			} else {
				return false;
			}
		}

		public bool HasFocus ()
		{
			return (ccText.IsFirstResponder || expiryText.IsFirstResponder || cvTwoText.IsFirstResponder);
		}
	}
}

