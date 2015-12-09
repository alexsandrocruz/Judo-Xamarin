using System;
using System.Drawing;


#if __UNIFIED__
using Foundation;
using UIKit;
using CoreFoundation;
using CoreGraphics;
// Mappings Unified CoreGraphic classes to MonoTouch classes
using RectangleF = global::CoreGraphics.CGRect;
using SizeF = global::CoreGraphics.CGSize;
using PointF = global::CoreGraphics.CGPoint;
#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreGraphics;
// Mappings Unified types to MonoTouch types
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

namespace JudoDotNetXamariniOSSDK
{
	internal static class UIExtensions
	{
		public static void Disable(this UIButton button)
		{
			button.Alpha = 0.25f;
			button.Enabled = false;
		}

		public static void Enable(this UIButton button)
		{
			button.Alpha = 1f;
			button.Enabled = true;
		}

		public static void RepositionFormSheetForiPad(this UIView superview)
		{
			superview.Bounds = new RectangleF (0, 0, 320f, 460f);

			RectangleF frame = superview.Frame;
			frame.Location = new PointF (frame.Location.X, 180f);
			superview.Frame = frame;
		}
	}
}

