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

namespace JudoDotNetXamariniOSSDK.Views
{
	[Register ("RegisterCardView")]
	partial class RegisterCardView
	{
		[Outlet]
		UIView EncapsulatingView { get; set; }

		[Outlet]
		UIButton FormClose { get; set; }

		[Outlet]
		UIButton RegisterButton { get; set; }

		[Outlet]
		SecureWebView SWebView { get; set; }

		[Outlet]
		UITableView TableView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SWebView != null) {
				SWebView.Dispose ();
				SWebView = null;
			}

			if (EncapsulatingView != null) {
				EncapsulatingView.Dispose ();
				EncapsulatingView = null;
			}

			if (FormClose != null) {
				FormClose.Dispose ();
				FormClose = null;
			}

			if (RegisterButton != null) {
				RegisterButton.Dispose ();
				RegisterButton = null;
			}

			if (TableView != null) {
				TableView.Dispose ();
				TableView = null;
			}
		}
	}
}
