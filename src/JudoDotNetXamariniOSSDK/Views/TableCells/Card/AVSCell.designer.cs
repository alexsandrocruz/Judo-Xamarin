// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
#if __UNIFIED__
using Foundation;
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
using UIKit;

namespace JudoDotNetXamariniOSSDK.Views.TableCells.Card
{
	[Register ("AVSCell")]
	partial class AVSCell
	{
		[Outlet]
		UIButton CountryButton { get; set; }

		[Outlet]
		UILabel CountryLabel { get; set; }

		[Outlet]
		UIButton HomeButton { get; set; }

		[Outlet]
		UIView PostCodeCountainerView { get; set; }

		[Outlet]
		UITextField PostcodeTextField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CountryButton != null) {
				CountryButton.Dispose ();
				CountryButton = null;
			}

			if (CountryLabel != null) {
				CountryLabel.Dispose ();
				CountryLabel = null;
			}

			if (PostCodeCountainerView != null) {
				PostCodeCountainerView.Dispose ();
				PostCodeCountainerView = null;
			}

			if (PostcodeTextField != null) {
				PostcodeTextField.Dispose ();
				PostcodeTextField = null;
			}

			if (HomeButton != null) {
				HomeButton.Dispose ();
				HomeButton = null;
			}
		}
	}
}
