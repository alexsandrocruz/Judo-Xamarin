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

namespace JudoDotNetXamariniOSSDK.Views.TableCells.Card
{
	[Register ("TokenPaymentCell")]
	partial class TokenPaymentCell
	{
		[Outlet]
		UIImageView cardImage { get; set; }

		[Outlet]
		UIView EntryEnclosingView { get; set; }

		[Outlet]
		UITextField entryField { get; set; }

		[Outlet]
		UILabel PreviousCardNumber { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (EntryEnclosingView != null) {
				EntryEnclosingView.Dispose ();
				EntryEnclosingView = null;
			}

			if (cardImage != null) {
				cardImage.Dispose ();
				cardImage = null;
			}

			if (entryField != null) {
				entryField.Dispose ();
				entryField = null;
			}

			if (PreviousCardNumber != null) {
				PreviousCardNumber.Dispose ();
				PreviousCardNumber = null;
			}
		}
	}
}
