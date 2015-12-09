using JudoDotNetXamariniOSSDK.Helpers;
using JudoPayDotNet.Models;
using UIKit;

namespace JudoDotNetXamariniOSSDK.Controllers
{

	internal static class CardImageHelper
	{
		public static UIImage CreditCardImage ( this CardType type)
		{
			string name;

			switch (type) {
			case CardType.VISA:
				name = @"ic_card_large_visa";
				break;
			case CardType.MASTERCARD:
				name = @"ic_card_large_mastercard";
				break;
			case CardType.MAESTRO:
				name = @"ic_card_large_maestro";
				break;
			case CardType.AMEX:
				name = @"ic_card_large_amex";
				break;
			default:
				name = @"ic_card_large_unknown";
				break;
			}
			return ThemeBundleReplacement.BundledOrReplacementImage (name, BundledOrReplacementOptions.BundledOrReplacement);
		}

		public static UIImage CreditCardBackImage (this CardType type)
		{
			string backName;

			switch (type) {
			case CardType.AMEX:
				backName = @"ic_card_large_cv2_amex";
				break;
			default:
				backName = @"ic_card_large_cv2";
				break;
			}	
			return ThemeBundleReplacement.BundledOrReplacementImage (backName, BundledOrReplacementOptions.BundledOrReplacement);
		}
	}
}

