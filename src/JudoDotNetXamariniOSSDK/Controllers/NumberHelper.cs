using System;
using Foundation;
using System.Globalization;

namespace JudoDotNetXamariniOSSDK
{

	internal static class NumberHelper
		{
			public static decimal ToDecimal(this NSDecimalNumber number)
			{
				var stringRepresentation = number.ToString ();
				return decimal.Parse(stringRepresentation, CultureInfo.InvariantCulture);
			}

		public static NSDecimalNumber ToNSDecimal(this decimal number)
			{
				return new NSDecimalNumber(number.ToString(CultureInfo.InvariantCulture));
			}
		}

}

