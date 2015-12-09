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
	[Register("FixedScrollView")]
	internal class FixedScrollView :UIScrollView
	{


		public FixedScrollView(IntPtr p) : base(p)
		{
		}
		public override void ScrollRectToVisible (RectangleF rect, bool animated)
		{
			
		}
			
	}
}

