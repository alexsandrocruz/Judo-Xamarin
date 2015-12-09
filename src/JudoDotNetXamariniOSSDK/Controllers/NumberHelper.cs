using System.Globalization;
using System.Linq;
using Foundation;

namespace JudoDotNetXamariniOSSDK.Controllers
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

		public static bool IsLuhnValid ( this string number)
		{

			var cardArray = number.Select (c => c - '0').ToArray ();

			return (cardArray.Select ((d, i) => i % 2 == cardArray.Length % 2 ? ((2 * d) % 10) + d / 5 : d).Sum () % 10) == 0;
		}

		}

}

