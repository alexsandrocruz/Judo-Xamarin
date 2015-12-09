using System;

namespace JudoDotNetXamarin
{
	public static class BillingCountryOptionsExtensions
	{
		public static  ISO3166CountryCodes GetISOCode (this BillingCountryOptions option )
		{
			switch (option) {
			case BillingCountryOptions.BillingCountryOptionUK:
				return ISO3166CountryCodes.UK;
				break;
			case BillingCountryOptions.BillingCountryOptionUSA:
				return ISO3166CountryCodes.USA;
				break;
			case BillingCountryOptions.BillingCountryOptionCanada:
				return ISO3166CountryCodes.Canada;
				break;
			default: 
				return ISO3166CountryCodes.Others;
				break;
			}
		}
	}
}

