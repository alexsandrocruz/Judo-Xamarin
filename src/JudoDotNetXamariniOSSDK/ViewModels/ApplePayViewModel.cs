using System.Linq;
using Foundation;
using PassKit;

namespace JudoDotNetXamariniOSSDK.ViewModels
{
	public class ApplePayViewModel
	{
		public NSString CurrencyCode  { get; set;}

		public NSString CountryCode  { get; set;}

		public PKMerchantCapability MerchantCapabilities  { get {return PKMerchantCapability.ThreeDS;}}

		//[PKPaymentNetworkVisa, PKPaymentNetworkMasterCard] or @[PKPaymentNetworkVisa, PKPaymentNetworkMasterCard, PKPaymentNetworkAmex]
	
		public NSString[] SupportedNetworks { get; set;}
		//# Next you need to include a summary of the user's basket, including total amount payable as the last line.

		public PKPaymentSummaryItem[] SummaryItems {get;set;}

		/// <summary>
		/// Final item on sleeve totalling previous items. Made out to merchant
		/// </summary>
		/// <value>The total summary item.</value>
		public PKPaymentSummaryItem TotalSummaryItem { get; set;}

		public PKPaymentSummaryItem[] Basket {
			get{ 
				if (SummaryItems.Length == 0) {
					return new PKPaymentSummaryItem[0];
				} else {
					var _basket = SummaryItems.ToList ();
					if (TotalSummaryItem != null) {
						_basket.Add (TotalSummaryItem);
					}
					return _basket.ToArray ();
				}
				 
				}
		}

		public NSString MerchantIdentifier  { get; set;}

		public NSString ConsumerRef {
			get;
			set;
		}
	}
}

