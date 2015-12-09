// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//

using Foundation;
using JudoDotNetXamariniOSSDK.Controllers;
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

namespace JudoDotNetXamariniOSSDK.Views.TableCells.Card
{
	[Register ("CardEntryCell")]
	partial class CardEntryCell
	{
		[Outlet]
		PlaceHolderTextView ccPlaceHolder { get; set; }

		[Outlet]
		NSLayoutConstraint ccPLaceHolderToScrollViewConstraint { get; set; }

		[Outlet]
		NSLayoutConstraint ccPlaceHolderWidthConstraint { get; set; }

		[Outlet]
		UITextView ccText { get; set; }

		[Outlet]
		UIView containerView { get; set; }

		[Outlet]
		UIImageView creditCardImage { get; set; }

		[Outlet]
		PlaceHolderTextView cvTwoPlaceHolder { get; set; }

		[Outlet]
		UITextView cvTwoText { get; set; }

		[Outlet]
		NSLayoutConstraint ExpiryGap { get; set; }

		[Outlet]
		UIButton ExpiryInfoButton { get; set; }

		[Outlet]
		PlaceHolderTextView expiryPlaceHolder { get; set; }

		[Outlet]
		UITextView expiryText { get; set; }

		[Outlet]
		UILabel PaymentErrorLabel { get; set; }

		[Outlet]
		UILabel StatusHelpLabel { get; set; }

		[Outlet]
		FixedScrollView textScroller { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ccPlaceHolder != null) {
				ccPlaceHolder.Dispose ();
				ccPlaceHolder = null;
			}

			if (ccPLaceHolderToScrollViewConstraint != null) {
				ccPLaceHolderToScrollViewConstraint.Dispose ();
				ccPLaceHolderToScrollViewConstraint = null;
			}

			if (ccPlaceHolderWidthConstraint != null) {
				ccPlaceHolderWidthConstraint.Dispose ();
				ccPlaceHolderWidthConstraint = null;
			}

			if (ccText != null) {
				ccText.Dispose ();
				ccText = null;
			}

			if (containerView != null) {
				containerView.Dispose ();
				containerView = null;
			}

			if (creditCardImage != null) {
				creditCardImage.Dispose ();
				creditCardImage = null;
			}

			if (cvTwoPlaceHolder != null) {
				cvTwoPlaceHolder.Dispose ();
				cvTwoPlaceHolder = null;
			}

			if (cvTwoText != null) {
				cvTwoText.Dispose ();
				cvTwoText = null;
			}

			if (ExpiryGap != null) {
				ExpiryGap.Dispose ();
				ExpiryGap = null;
			}

			if (ExpiryInfoButton != null) {
				ExpiryInfoButton.Dispose ();
				ExpiryInfoButton = null;
			}

			if (expiryPlaceHolder != null) {
				expiryPlaceHolder.Dispose ();
				expiryPlaceHolder = null;
			}

			if (expiryText != null) {
				expiryText.Dispose ();
				expiryText = null;
			}

			if (PaymentErrorLabel != null) {
				PaymentErrorLabel.Dispose ();
				PaymentErrorLabel = null;
			}

			if (StatusHelpLabel != null) {
				StatusHelpLabel.Dispose ();
				StatusHelpLabel = null;
			}

			if (textScroller != null) {
				textScroller.Dispose ();
				textScroller = null;
			}
		}
	}
}
