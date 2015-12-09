using System.Collections.Generic;
using JudoPayDotNet.Models;

namespace JudoDotNetXamarin
{
    public class Card
    {
        public string CardNumber { get; set; }

        public string ExpiryDate { get; set; }

        public string CV2 { get; set; }

        public string CardToken { get; set; }

        public string LastFour { get; set; }

        public string StartDate { get; set; }

        public string IssueNumber { get; set; }

        public string PostCode { get; set; }

        public string CountryCode { get; set; }

        public CardType CardType { get; set; }

        public Card (string cardNumber = null, string expiryDate = null, string cv2 = null, string startDate = null, string issueNumber = null, 
               string cardToken = null, string lastFour = null, CardType? cardType = null, string postCode = null, string countryCode = null)
        {
            CardNumber = cardNumber;
            ExpiryDate = expiryDate;
            CV2 = cv2;
            StartDate = startDate;
            IssueNumber = issueNumber;
            CardToken = cardToken;
            LastFour = lastFour;

            if (cardType.HasValue) {
                CardType = cardType.Value;
            }

            PostCode = postCode;
            CountryCode = countryCode;
        }

        public static int CC_LEN_FOR_TYPE{ get { return 4; } }

        public Card (Dictionary<string, object> details)
        {
            CardToken = details ["cardToken"] as string;
            ExpiryDate = details ["endDate"] as string;
            LastFour = details ["cardLastFour"] as string;
            var type = details ["cardType"] as string;

            int cardType;
            if (int.TryParse (type, out cardType)) {
                CardType = (CardType)cardType;
            }

            StartDate = details ["startDate"] as string;
            IssueNumber = details ["issueNumber"] as string;

            Dictionary<string, string> cardAddress;
            if ((cardAddress = details ["cardAddress"] as Dictionary<string, string>) != null) {
                PostCode = cardAddress ["postCode"];
                CountryCode = cardAddress ["countryCode"];
            }
        }
    }
}

