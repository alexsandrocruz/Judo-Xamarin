using System;
using Foundation;
using UIKit;
#if __UNIFIED__
// Mappings Unified CoreGraphic classes to MonoTouch classes
using RectangleF = global::CoreGraphics.CGRect;

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

namespace JudoDotNetXamariniOSSDK.Controllers
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

