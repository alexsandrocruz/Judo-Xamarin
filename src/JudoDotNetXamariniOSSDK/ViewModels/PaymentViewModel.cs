using System;
using System.Collections.Generic;

namespace JudoDotNetXamariniOSSDK
{
	public class PaymentViewModel 
	{
        /// <summary>
        /// Card Detail
        /// </summary>
		public CardViewModel Card { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
		public decimal Amount { get; set; }

        /// <summary>
        /// Currency 
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// your Payment reference 
        /// </summary>
        public string PaymentReference { get; set; }

        /// <summary>
        /// your consumer reference 
        /// </summary>
        public string ConsumerReference { get; set; }

        /// <summary>
        /// your meta data 
        /// </summary>
        public IDictionary<string, string> YourPaymentMetaData { get; set; }

		public PaymentViewModel Clone()
		{
			return new PaymentViewModel
			{
				Card = this.Card.Clone(),
				Amount = this.Amount,
				Currency = this.Currency,
				PaymentReference = this.PaymentReference,
				ConsumerReference = this.ConsumerReference,
				YourPaymentMetaData = this.YourPaymentMetaData
			}; 
		}
	}
}

