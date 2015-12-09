// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//


using Foundation;
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

namespace JudoDotNetXamariniOSSDK.Views
{
	partial class SlideUpMenu
	{
		[Outlet]
		UISwitch AmexSwitch { get; set; }

		[Outlet]
		UIImageView ArrowIcon { get; set; }

		[Outlet]
		UISwitch AVSSwitch { get; set; }

		[Outlet]
		UISwitch MaestroSwitch { get; set; }

		[Outlet]
		UISwitch NoneUISwitch { get; set; }

		[Outlet]
		UISwitch RiskSwitch { get; set; }

		[Outlet]
		UISwitch ThreeDSwitch { get; set; }

		[Outlet]
		UIView View { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (NoneUISwitch != null) {
				NoneUISwitch.Dispose ();
				NoneUISwitch = null;
			}

			if (AmexSwitch != null) {
				AmexSwitch.Dispose ();
				AmexSwitch = null;
			}

			if (ArrowIcon != null) {
				ArrowIcon.Dispose ();
				ArrowIcon = null;
			}

			if (AVSSwitch != null) {
				AVSSwitch.Dispose ();
				AVSSwitch = null;
			}

			if (MaestroSwitch != null) {
				MaestroSwitch.Dispose ();
				MaestroSwitch = null;
			}

			if (RiskSwitch != null) {
				RiskSwitch.Dispose ();
				RiskSwitch = null;
			}

			if (ThreeDSwitch != null) {
				ThreeDSwitch.Dispose ();
				ThreeDSwitch = null;
			}

			if (View != null) {
				View.Dispose ();
				View = null;
			}
		}
	}
}
