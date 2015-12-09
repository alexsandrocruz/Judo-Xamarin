
using System;

#if __UNIFIED__
using Foundation;
using UIKit;
using CoreFoundation;
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
// Mappings Unified types to MonoTouch types
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

namespace JudoDotNetXamariniOSSDK
{
	public partial class AVSCell : CardCell
	{
		public static readonly UINib Nib = UINib.FromName ("AVSCell", NSBundle.MainBundle);

		public UIActionSheet countrySheet;
		BillingCountryOptions selectedCountry = BillingCountryOptions.BillingCountryOptionUK;


		public UITextField PostcodeTextFieldOutlet { get { return PostcodeTextField; } }
		public AVSCell (IntPtr handle) : base (handle)
		{
			Key = "AVSCell";
		}

		public override CardCell Create ()
		{
			return (AVSCell)Nib.Instantiate (null, null) [0];
		}


		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
		}


		public override void  SetUpCell ()
		{
			countrySheet = new UIActionSheet ("Select Country");
			countrySheet.TintColor = UIColor.Black;
			selectedCountry = BillingCountryOptions.BillingCountryOptionUK;


			HomeButton.TouchUpInside += (sender, ev) => {
				DismissKeyboardAction();
			};
			countrySheet.Clicked += delegate(object sender, UIButtonEventArgs button) {
				switch (button.ButtonIndex) {
				case (int) BillingCountryOptions.BillingCountryOptionUK:
					selectedCountry = BillingCountryOptions.BillingCountryOptionUK;

					break;
				case (int)BillingCountryOptions.BillingCountryOptionUSA:
					selectedCountry = BillingCountryOptions.BillingCountryOptionUSA;

					break;
				case (int)BillingCountryOptions.BillingCountryOptionCanada:
					selectedCountry = BillingCountryOptions.BillingCountryOptionCanada;

					break;
				case (int)BillingCountryOptions.BillingCountryOptionOther:
					selectedCountry = BillingCountryOptions.BillingCountryOptionOther;

					break;
				default:
					selectedCountry = BillingCountryOptions.BillingCountryOptionUK;
					break;

				}
				CountryLabel.Text = selectedCountry.ToDescriptionString ();
			};

			foreach (BillingCountryOptions option in Enum.GetValues(typeof(BillingCountryOptions))) {
				countrySheet.AddButton (option.ToDescriptionString ());
			}
				CountryButton.TouchUpInside += (sender, ev) => {
					if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
						countrySheet.ShowInView(this.Superview);
					}
					else
					{
						countrySheet.ShowInView (UIApplication.SharedApplication.KeyWindow);
					}
			};
			PostcodeTextField.Text = "";	
			PostcodeTextField.Font = JudoSDKManager.FIXED_WIDTH_FONT_SIZE_20;;
			PostcodeTextField.TextColor = UIColor.Black;

			PostcodeTextField.ShouldChangeCharacters = (UITextField textField, NSRange nsRange, string replacementString) => {
				CSRange range = new CSRange ((int)nsRange.Location, (int)nsRange.Length);
				DispatchQueue.MainQueue.DispatchAsync (() => {
				});
				int textLengthAfter = textField.Text.Length + replacementString.Length - range.Length;
				if (textLengthAfter > 10) {
					return false;
				}
				return true;
			};

		}

		public void GatherCardDetails (CardViewModel cardViewModel)
		{
			cardViewModel.PostCode = PostcodeTextField.Text;

			switch (selectedCountry) {
			case BillingCountryOptions.BillingCountryOptionUK:
                    cardViewModel.CountryCode = ISO3166CountryCodes.UK;
				break;
			case BillingCountryOptions.BillingCountryOptionUSA:
                cardViewModel.CountryCode = ISO3166CountryCodes.USA;
				break;
            case BillingCountryOptions.BillingCountryOptionCanada:
                cardViewModel.CountryCode = ISO3166CountryCodes.Canada;
                break;
            default:					
				break;
			}
		}
		public void CleanUp ()
		{
			SetUpCell ();
		}

		public override void DismissKeyboardAction ()
		{
			PostcodeTextField.ResignFirstResponder ();
		}


	}
}

