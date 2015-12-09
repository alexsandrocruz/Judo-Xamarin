using System;
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
	internal static class ColourHelper
	{
		public static UIColor GetColour (int hex)
		{
			const int mask = 0xFF;
			nfloat divisor = 255.0f;

			nfloat r = (hex >> 24) & mask;
			nfloat g = (hex >> 16) & mask;
			nfloat b = (hex >> 8) & mask;
			nfloat a = hex & mask;

			return UIColor.FromRGB(r/divisor, g / divisor, b / divisor);
		}

		public static UIColor GetColour(string color)
		{
		    var hex = Convert.ToInt32 (color, 16);
		    return hex != 0 ? GetColour(hex) : null;
		}
	}
}

