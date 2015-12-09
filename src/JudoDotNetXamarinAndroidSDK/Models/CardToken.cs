using System;
using JudoPayDotNet.Models;
using System.IO;
using JudoDotNetXamarinAndroidSDK.Utils;

namespace JudoDotNetXamarinAndroidSDK
{
    public class CardToken
    {
        public string CardLastFour { get; set; }

        public string CardFirstSix { get; set; }

        public string Token { get; set; }

        public CardType CardType { get; set; }

        public string ExpiryDate { get; set; }

        public string ConsumerToken { get; set; }

        public CardToken ()
        {

        }

        public bool IsValidCard ()
        {
            if (!ValidationHelper.CheckExpDate (ExpiryDate)) {
                throw new InvalidDataException ("Invalid Expiry Date");
            }

            if (String.IsNullOrWhiteSpace (Token)) {
                throw new InvalidDataException ("Invalid Card Token");
            }

            if (!ValidationHelper.CheckCardType (CardType)) {
                throw new InvalidDataException ("Invalid Card Type");
            }

            return true;
        }

    }
}

