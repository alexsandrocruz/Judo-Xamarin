// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using System.CodeDom.Compiler;

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
	[Register ("PreAuthorisationView")]
	partial class PreAuthorisationView
	{
		[Outlet]
		UIView EncapsulatingView { get; set; }

		[Outlet]
		UIButton FormClose { get; set; }

		[Outlet]
		UIButton RegisterButton { get; set; }

		[Outlet]
		JudoDotNetXamariniOSSDK.SecureWebView SWebView { get; set; }

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
