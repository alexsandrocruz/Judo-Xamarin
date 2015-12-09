using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace JudoDotNetXamarinSDK.Utils
{
    public class ValidationHelper
    {
        // TODO: this regex are Java and may not run well on C#
        // Regex expressions for various card types
        public static Regex REGEX_VISA = new Regex("^4[0-9]{3}.*?");//"^4[0-9]{12}(?:[0-9]{3})?$";//
        public static Regex REGEX_MC = new Regex("^5[1-5][0-9]{2}.*?");//"^5[1-5][0-9]{14}$";//
        public static Regex REGEX_MAESTRO = new Regex("^(5018|5020|5038|6304|6759|6761|6763|6334|6767|4903|4905|4911|4936|564182|633110|6333|6759|5600|5602|5603|5610|5611|5656|6700|6706|6773|6775|6709|6771|6773|6775).*?");
        public static Regex REGEX_AMEX = new Regex("^3[47][0-9]{2}.*?");
        public static Regex REGEX_DISCOVER = new Regex("^6(?:011|5[0-9]{2})$");
        public static Regex REGEX_DINERS = new Regex("^3(?:0[0-5]|[68][0-9])[0-9]$");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ccNumber"></param>
        /// <returns>true if the ccNumber is valid</returns>
        public static bool CheckLuhn(string ccNumber)
        {
            int sum = 0;
            bool alternate = false;
            for (int i = ccNumber.Length - 1; i >= 0; --i)
            {
                int n = int.Parse(ccNumber.Substring(i, 1));
                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                    {
                        n = (n%10) + 1;
                    }
                }
                sum += n;
                alternate = !alternate;
            }

            return (sum%10 == 0);
        }

        /// <summary>
        /// Returns true if judo can process the given card type.
        /// </summary>
        /// <param name="cardType"></param>
        /// <remarks>AMEX and MAESTRO card types must be enabled on your account before you can process them.</remarks>
        /// <returns></returns>
        public static bool CheckCardType(CardBase.CardType cardType)
        {
            switch (cardType)
            {
                case CardBase.CardType.VISA:
                case CardBase.CardType.MASTERCARD:
                case CardBase.CardType.VISA_DEBIT:
                case CardBase.CardType.VISA_ELECTRON:
                    return true;

                case CardBase.CardType.MASTRO:
                case CardBase.CardType.AMEX:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CheckExpDate(string date)
        {
            try
            {
                string[] dataString = date.Split('/');
                int month = int.Parse(dataString[0]);
                int year = int.Parse(dataString[1]);
                return CheckExpDate(month, year);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CheckExpDate(string month, string year)
        {
            try
            {
                int monthInt = int.Parse(month);
                int yearInt = int.Parse(year);
                return CheckExpDate(monthInt, yearInt);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CheckExpDate(string date, char separator)
        {
            try
            {
                string[] d = date.Split(separator);
                int monthInt = int.Parse(d[0]);
                int yearInt = int.Parse(d[1]);
                return CheckExpDate(monthInt, yearInt);
            }
            catch (FormatException)
            {
                return false;
            }
            catch(IndexOutOfRangeException){
                return false;
            }
        }

        public static bool CheckExpDate(int month, int year)
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            year += 2000;

            if (year > currentYear)
            {
                return true;
            }

            if (year < currentYear)
            {
                return false;
            }

            return currentMonth <= month;
        }

        public static bool CheckStartDate(string date)
        {
            return CheckStartDate(date, '/');
        }

        public static bool CheckStartDate(string date, char separator)
        {
            try
            {
                string[] d = date.Split(separator);
                int monthInt = int.Parse(d[0]);
                int yearInt = int.Parse(d[1]);
                return CheckStartDate(monthInt, yearInt);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }

        }

        public static bool CheckStartDate(int month, int year)
        {
            year += 2000;
            return new DateTime(year, month, 1) < DateTime.Now;
        }

        public static bool CheckCVV(string cvv, CardBase.CardType cardType)
        {
            try
            {
                switch (cardType)
                {
                    case CardBase.CardType.AMEX:
                        return CheckCIDV(int.Parse(cvv));
                    default:
                        return CheckCV2(int.Parse(cvv));
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CheckCV2(string cv2)
        {
            try
            {
                return CheckCV2(int.Parse(cv2));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CheckCV2(int cv2)
        {
            return (cv2 > 0 && cv2 < 1000);
        }

        public static bool CheckCIDV(int cidv)
        {
            return (cidv > 0 && cidv < 10000);
        }

        public static bool CheckIssueNumber(string issueNumber)
        {
            try
            {
                return CheckIssueNumber(int.Parse(issueNumber));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CheckIssueNumber(int issueNumber)
        {
            return issueNumber > 0 && issueNumber < 100;
        }

        public static CardBase.CardType GetCardType(string cardNumber)
        {
            if (REGEX_VISA.IsMatch(cardNumber))
            {
                return CardBase.CardType.VISA;
            }

            if (REGEX_MC.IsMatch(cardNumber))
            {
                return CardBase.CardType.MASTERCARD;
            }

            if (REGEX_MAESTRO.IsMatch(cardNumber))
            {
                return CardBase.CardType.MASTRO;
            }

            if (REGEX_AMEX.IsMatch(cardNumber))
            {
                return CardBase.CardType.AMEX;
            }

            return CardBase.CardType.UNKNOWN;
        }

        public static bool IsStartDateRequiredForCardNumber(string cardNumber)
        {
            return IsStartDateOrIssueNumberRequiredForCardType(GetCardType(cardNumber));
        }

        public static bool IsStartDateOrIssueNumberRequiredForCardType(CardBase.CardType cardType)
        {
            return CardBase.CardType.MASTRO == cardType;
        }

        public static bool CheckLength(string cardNumber)
        {
            //TODO: Regex for Java may not work on C#
            // Remove all non numeric characters first
            cardNumber = Regex.Replace(cardNumber, "[^\\d.]", "");
            return cardNumber.Length == 16;
        }

        public static bool CanProcess(string cardNumber)
        {
            if (!CheckLuhn(cardNumber))
            {
                return false;
            }

            return CheckCardType(GetCardType(cardNumber));
        }

        public static bool CheckCard(CardBase card)
        {
            return card.IsValidCard();
        }
    }
}