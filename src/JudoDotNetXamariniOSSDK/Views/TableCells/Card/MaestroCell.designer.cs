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
	[Register ("MaestroCell")]
	partial class MaestroCell
	{
		[Outlet]
		UITextField IssueNumberTextField { get; set; }

		[Outlet]
		UILabel StartDatePlaceHolder { get; set; }

		[Outlet]
		UITextField StartDateTextField { get; set; }

		[Outlet]
		UILabel StartDateWarningLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (IssueNumberTextField != null) {
				IssueNumberTextField.Dispose ();
				IssueNumberTextField = null;
			}

			if (StartDateTextField != null) {
				StartDateTextField.Dispose ();
				StartDateTextField = null;
			}

			if (StartDatePlaceHolder != null) {
				StartDatePlaceHolder.Dispose ();
				StartDatePlaceHolder = null;
			}

			if (StartDateWarningLabel != null) {
				StartDateWarningLabel.Dispose ();
				StartDateWarningLabel = null;
			}
		}
	}
}
