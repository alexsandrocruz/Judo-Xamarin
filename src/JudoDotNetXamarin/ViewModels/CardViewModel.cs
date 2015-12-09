using JudoDotNetXamarin;
using JudoPayDotNet.Models;

namespace JudoDotNetXamarin
{
    public class CardViewModel
    {
        /// <summary>
        /// Card Number 
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Card expiry date 
        /// </summary>
        public string ExpireDate { get; set; }

        /// <summary>
        /// Card CV2 number
        /// </summary>
        public string CV2 { get; set; }

        /// <summary>
        /// Card Type
        /// </summary>
        internal CardType CardType { get; set; }

        /// <summary>
        /// Postcode
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// ISO standard CountryCode
        /// </summary>
        public ISO3166CountryCodes CountryCode { get; set; }

        /// <summary>
        /// card start date 
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Issue Number for Mestro card
        /// </summary>
        public string IssueNumber { get; set; }

        public CardViewModel Clone ()
        {
            return new CardViewModel {
                CardNumber = this.CardNumber,
                ExpireDate = this.ExpireDate,
                CV2 = this.CV2,
                CardType = this.CardType,
                PostCode = this.PostCode,
                CountryCode = this.CountryCode,
                StartDate = this.StartDate,
                IssueNumber = this.IssueNumber,
            }; 
        }
    }


}

