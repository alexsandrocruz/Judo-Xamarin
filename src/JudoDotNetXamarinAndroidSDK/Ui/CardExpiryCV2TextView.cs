using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Sql;
using JudoDotNetXamarinSDK.Utils;

namespace JudoDotNetXamarinSDK.Ui
{
    public class CardExpiryCV2TextView : BackgroundHintTextView
    {
        private string last4NumbersOfCard;

        public Action OnBackKeyPressed { get; set; }

        public CardExpiryCV2TextView(Context context) : base(context)
        {
            Init();
        }

        public CardExpiryCV2TextView(Context context, IAttributeSet attributeSet) : base(context, attributeSet)
        {
            Init();
        }

        public CardExpiryCV2TextView(Context context, IAttributeSet attributeSet, int defStyle) : base(context, attributeSet, defStyle)
        {
            Init();
        }

        private void Init()
        {
            // Set our hint text
            SetHintText(JudoSDKManager.GetExpiryAndValidationHintFormat(CardBase.CardType.UNKNOWN));
            // Set error text
            SetErrorText("Invalid CV2");
            // Set additional chars to skip
            SetInputFilter("/");
        }

        public void ValidateExpiryDate(string expiry)
        {
            if (!string.IsNullOrWhiteSpace(expiry))
            {
                string month = expiry.Substring(0, 2);

                //validate month
                int monthNumber;
                try
                {
                    if ((monthNumber = int.Parse(month)) > 12)
                    {
                        SetErrorText("Invalid Month");
                        throw new Exception(Resources.GetString(Resource.String.msg_check_exp_date));
                    }
                }
                catch (FormatException e)
                {
                    SetErrorText("Invalid Month");
                    throw e;
                }

                var checkExpDate = Resources.GetString(Resource.String.msg_check_exp_date);

                //validate year
                var year = expiry.Substring(3, 2);
                int yearNumber;

                if (!int.TryParse(year, out yearNumber))
                {
                    SetErrorText("Invalid Year");
                    throw new Exception(checkExpDate);
                }

                yearNumber += 2000;

                //validate not in the past
                var now = DateTime.Now;

                var expiryDate = new DateTime(yearNumber, monthNumber, now.Day, now.Hour, now.Minute, now.Second);

                if (expiryDate < now)
                {
                    throw new Exception(checkExpDate);
                }

                var tenYearsInTheFuture = now.AddYears(10);

                if (expiryDate > tenYearsInTheFuture)
                {
                    throw new Exception(checkExpDate);
                }
            }
        }

        public void SetLast4NumbersOfCard(string last4NumbersOfCard)
        {
            if (!String.IsNullOrWhiteSpace(last4NumbersOfCard) && last4NumbersOfCard.Length == 4)
            {
                this.last4NumbersOfCard = last4NumbersOfCard;
                AddFixedText(last4NumbersOfCard);
            }
        }

        public string GetCardCV2()
        {
            string expiryAndCV2 = GetEditText().Text;
            string[] temp = expiryAndCV2.Split(' ');

            if (temp.Length < 2)
            {
                Log.Error(this.ToString(), "Error: Invalid expiry and/or cv2");
                throw new InvalidDataException("Expiry date and/or cv2");
            }

            string expiry = temp[0];
            string cv2 = temp[1];

            return cv2;
        }

        public string GetCardExpiry()
        {
            string expiryAndCV2 = GetEditText().Text;
            var temp = expiryAndCV2.Split(' ');
            if (temp.Length < 2)
            {
                Log.Error(ToString(), "Error: Invalid expiry and/or cv2");
                throw new ArgumentException("Expiry date and/or cv2");
            }

            var expiry = temp[0];
            var cv2 = temp[1];

            return expiry;
        }

        public override void ValidateInput(string input)
        {
            string[] temp = input.Split(' ');
            if (temp.Length < 2)
            {
                SetErrorText("Invalid Expiry / CV2");
                throw new Java.Lang.Exception("Invalid expiry / CV2");
            }

            string expiry = temp[0];

            ValidateExpiryDate(expiry);
        }

        public override void BackKeyPressed()
        {
            if (OnBackKeyPressed != null)
            {
                OnBackKeyPressed();
            }
        }

        public void ValidatePartialInput()
        {
            string input = GetText();

            if (input.Length > 1)
            {
                if (!Regex.IsMatch(input.Substring(0, 1), "[0-1]"))
                {
                    SetText("");
                }
            }

            if (input.Length >= 2)
            {
                int value = int.Parse(input.Substring(0, 2));
                if (value > 12)
                {
                    SetText(input.Substring(0, 1));
                }
                else
                {
                    if (value == 0)
                    {
                        SetText("0");
                    }
                }
            }
        }

        public override void SetHintText(string hintText)
        {
            base.SetHintText(hintText);
            SetInputFilter("/");
        }

        protected override float GetContainerHeight()
        {
            return Resources.GetDimension(Resource.Dimension.cardnumber_container_height);
        }
    }
}