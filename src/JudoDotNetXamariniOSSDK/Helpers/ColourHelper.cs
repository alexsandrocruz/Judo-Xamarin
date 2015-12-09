using System;
using UIKit;
#if __UNIFIED__
// Mappings Unified CoreGraphic classes to MonoTouch classes

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

namespace JudoDotNetXamariniOSSDK.Helpers
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

