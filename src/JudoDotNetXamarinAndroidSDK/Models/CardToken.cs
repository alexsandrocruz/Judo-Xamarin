using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using JudoDotNetXamarinSDK.Utils;
using JudoPayDotNet.Clients;
using Object = Java.Lang.Object;

namespace JudoDotNetXamarinSDK.Models
{
    public class CardToken : Object, IParcelable
    {

        public string CardLastFour { get; set; }
        public string CardFirstSix { get; set; }
        public string Token { get; set; }
        public CardBase.CardType CardType { get; set; }
        public string ExpiryDate { get; set; }

        public string ConsumerToken { get; set; }

        [ExportField("CREATOR")]
        public static CardTokenCreator InitializeCreator()
        {
            return new CardTokenCreator();
        }

        public CardToken() { }

        public CardToken(Parcel parcel)
        {
            CardLastFour = parcel.ReadString();
            ExpiryDate = parcel.ReadString();
            Token = parcel.ReadString();
            CardType = (CardBase.CardType)parcel.ReadInt();
            ConsumerToken = parcel.ReadString();
        }

        public bool IsValidCard()
        {
            if (!ValidationHelper.CheckExpDate(ExpiryDate))
            {
                throw new InvalidDataException("Invalid Expiry Date");
            }

            if (String.IsNullOrWhiteSpace(Token))
            {
                throw new InvalidDataException("Invalid Card Token");
            }

            if (!ValidationHelper.CheckCardType(CardType))
            {
                throw new InvalidDataException("Invalid Card Type");
            }

            return true;
        }

        public static CardToken InitCardToken(string token, string consumerToken, string cv2, CardBase.CardType cardType, string expiryDate = null, string firstSix = null, string lastfour = null)
        {
            CardToken cd = new CardToken();

            cd.Token = token;
            cd.ConsumerToken = consumerToken;

            int cv2Number;

            if (!int.TryParse(cv2, out cv2Number))
            {
                throw new InvalidDataException("CV2");
            }

            cd.CardType = cardType;
            cd.ExpiryDate = expiryDate;
            cd.CardFirstSix = firstSix;
            cd.CardLastFour = lastfour;

            cd.IsValidCard();

            return cd;
        }

        public void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
            dest.WriteString(CardLastFour);
            dest.WriteString(ExpiryDate);
            dest.WriteString(Token);
            dest.WriteInt((int)CardType);
            dest.WriteString(ConsumerToken);
        }

        public int DescribeContents()
        {
            return 0;
        }
    }

    public class CardTokenCreator : Object, IParcelableCreator
    {
        public Object CreateFromParcel(Parcel source)
        {
            return new CardToken(source);
        }

        public Object[] NewArray(int size)
        {
            return new CardToken[size];
        }
    }
}