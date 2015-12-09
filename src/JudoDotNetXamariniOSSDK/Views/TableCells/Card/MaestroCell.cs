
using System;
using System.Text;

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
	public partial class MaestroCell : CardCell
	{
		public static readonly UINib Nib = UINib.FromName ("MaestroCell", NSBundle.MainBundle);
	
		CreditCard cardHelper = new CreditCard ();
		int currentYear = DateTime.Now.Year - 2000;


		public UITextField StartDateTextFieldOutlet { get { return StartDateTextField; } }
		public UITextField IssueNumberTextFieldOutlet { get { return IssueNumberTextField; } }
	

		public MaestroCell (IntPtr handle) : base (handle)
		{
			Key = "MaestroCell";
		}

		public override CardCell Create ()
		{
			return (MaestroCell)Nib.Instantiate (null, null) [0];
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
		}

		void FlashCheckDateLabel ()
		{
			StartDateWarningLabel.Hidden = false;
			DispatchQueue.MainQueue.DispatchAfter (new DispatchTime (DispatchTime.Now, 1 * 1000000000), () => {
				StartDateWarningLabel.Hidden = true;
			});
		}


		public override void SetUpCell ()
		{
			StartDateTextField.ShouldChangeCharacters = (UITextField textField, NSRange nsRange, string replacementString) => {
				if(replacementString!=""&&!Char.IsDigit(replacementString.ToCharArray()[0]))
				{
					return false;
				}
				CSRange range = new CSRange ((int)nsRange.Location, (int)nsRange.Length);
				DispatchQueue.MainQueue.DispatchAsync (() => {
					UpdateUI ();
				});
				bool changeText = true;

				if (range.Length > 1) {
					return false;
				}
				if (replacementString.Length > 1) {
					return false;
				}
				if (replacementString.Length == 1 && !char.IsDigit (replacementString.ToCharArray () [0])) {
					return false;
				}
				if (textField.Text.Length + replacementString.Length - range.Length > 5) {
					return false;
				}

				int textLengthAfter = (int)(textField.Text.Length + replacementString.Length - range.Length);

				if (replacementString.Length == 0 && range.Location < 2 && textField.Text.Contains ("/")) {
					textField.Text = textField.Text.Replace (@"/", @"");
					textLengthAfter--;
				}
				if (range.Length == 1 && textField.Text.Substring (range.Location, 1) == "/") { 
					textField.Text = textField.Text.Substring (0, 1);
					textLengthAfter = 1;
					changeText = false;
				}

				if (range.Location == 1 && textField.Text.Length == 1) {						

					var aStringBuilder = new StringBuilder (textField.Text);
					aStringBuilder.Remove (range.Location, range.Length);
					aStringBuilder.Insert (range.Location, replacementString);
					string newTextOrig = aStringBuilder.ToString ();

					string text = newTextOrig;
					if (Int32.Parse (text) > 12 || Int32.Parse (text) == 0) {
						FlashCheckDateLabel ();
						return false;
					}

					textField.Text = text;
					textField.Text = textField.Text + @"/";
					textLengthAfter++;
					changeText = false;
				} else if (range.Location == 0 && textField.Text.Length == 0) {
					if (replacementString.Substring (0, 1).ToCharArray () [0] > '1') { 


						var formatedString = string.Format (@"0{0}/", replacementString);

						var aStringBuilder = new StringBuilder (textField.Text);
						aStringBuilder.Remove (range.Location, range.Length);
						aStringBuilder.Insert (range.Location, formatedString);

						textField.Text = aStringBuilder.ToString ();
						textLengthAfter += 2;
						changeText = false;
					}
				}

				if (textLengthAfter >= 4) {

					var aStringBuilder = new StringBuilder (textField.Text);
					aStringBuilder.Remove (range.Location, range.Length);
					aStringBuilder.Insert (range.Location, replacementString);

					string textAfter =  aStringBuilder.ToString ();


					int proposedDecade = (textAfter.ToCharArray () [3] - '0') * 10;
					int yearDecade = currentYear - (currentYear % 10);

					if (proposedDecade > yearDecade) {
						FlashCheckDateLabel ();
						return false;
					}

					if (textLengthAfter == 5) {
						if (!cardHelper.IsStartDateValid (textAfter)) {
							FlashCheckDateLabel ();
							return false;
						}


						var bStringBuilder = new StringBuilder (textField.Text);
						bStringBuilder.Remove (range.Location, range.Length);
						bStringBuilder.Insert (range.Location, replacementString);


						textField.Text =  bStringBuilder.ToString ();
						DispatchQueue.MainQueue.DispatchAsync (() => {
						IssueNumberTextField.BecomeFirstResponder ();
						});
						changeText = false;
					}
				}

				char[] placeHolder = "MM/YY".ToCharArray ();
				for (int iii = 0; iii < textLengthAfter; iii++) {
					placeHolder [iii] = ' ';
				}

				StartDatePlaceHolder.Text = new string (placeHolder);
				return changeText;
			};


			IssueNumberTextField.ShouldChangeCharacters = (UITextField textField, NSRange nsRange, string replacementString) => {
				if(replacementString!=""&&!Char.IsDigit(replacementString.ToCharArray()[0]))
				{
					return false;
				}
				CSRange range = new CSRange ((int)nsRange.Location, (int)nsRange.Length);
				DispatchQueue.MainQueue.DispatchAsync (() => {
					UpdateUI ();
				});
				if (range.Length > 1) {
					return false;
				}
				if (replacementString.Length > 1) {
					return false;
				}
				if (replacementString.Length == 1 && !char.IsDigit (replacementString.ToCharArray () [0])) {
					return false;
				}
				if (textField.Text.Length + replacementString.Length - range.Length > 3) {
					return false;
				}
				if (textField.Text.Length + replacementString.Length==3&&replacementString!="")
				{
					var aStringBuilder = new StringBuilder (textField.Text);
					aStringBuilder.Remove (range.Location, range.Length);
					aStringBuilder.Insert (range.Location, replacementString);
					textField.Text = aStringBuilder.ToString ();
					DismissKeyboardAction();
					return true;
				}
				return true;
			};
		}

		public void GatherCardDetails (CardViewModel cardViewModel)
		{
		    cardViewModel.StartDate = StartDateTextField.Text;
			cardViewModel.IssueNumber = IssueNumberTextField.Text;
		}

		public void CleanUp ()
		{
			SetUpCell ();
		}



		public override void DismissKeyboardAction ()
		{
			IssueNumberTextField.ResignFirstResponder ();
			StartDateTextField.ResignFirstResponder ();
		}
			
	}
}

