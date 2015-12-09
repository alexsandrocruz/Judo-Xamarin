using System;
using System.Collections.Generic;
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

namespace JudoDotNetXamarinSDK.Ui
{
    public class StartDateEditText : BackgroundHintTextView
    {

        public Action BackKeyPressedListener { get; set; }

        public StartDateEditText(Context context) : base(context)
        {
            Init();
        }

        public StartDateEditText(Context context, IAttributeSet attributes) : base(context, attributes)
        {
            Init();
        }

        public StartDateEditText(Context context, IAttributeSet attributes, int defStyle) : base(context, attributes, defStyle)
        {
            Init();
        }

        private void Init()
        {
            //Set hint text
            SetHintText("MM/YY");
            //Set error text
            SetErrorText("Invalid start date");
            //Set additional chars to skip
            SetInputFilter("/");
        }

        public void ValidateStartDate(string start)
        {
            var replaceOnce = new Regex(Regex.Escape(" "));
            start = replaceOnce.Replace(start, "", 1);

            if (!String.IsNullOrWhiteSpace(start))
            {
                var month = start.Substring(0, 2);

                int monthInt;

                try
                {
                    if ((monthInt = int.Parse(month)) > 12)
                    {
                        SetErrorText("Invalid Month");
                        throw new Exception(Resources.GetString(Resource.String.msg_check_start_date));
                    }
                }
                catch (FormatException e)
                {
                    SetErrorText("Invalid Month");
                    throw e;
                }

                //validate not in the past
                var year = start.Substring(3, 2);
                int yearInt;
                if (!int.TryParse(year, out yearInt))
                {
                    SetErrorText("Invalid Year");
                    throw new Exception(Resources.GetString(Resource.String.msg_check_start_date));
                }

                yearInt += 2000;

                var now = DateTime.Now;

                var startDate = new DateTime(yearInt, monthInt, now.Day, now.Hour, now.Minute, now.Second);

                if (startDate > now)
                {
                    SetErrorText("Invalid");
                    throw new Exception(Resources.GetString(Resource.String.msg_check_start_date));
                }

                if (startDate < now.AddYears(-10))
                {
                    SetErrorText("Invalid");
                    throw new Exception(Resources.GetString(Resource.String.msg_check_start_date));
                }
            }
        }

        public void ValidatePartialInput()
        {
            var input = GetText().ToString();

            if (input.Length >= 2)
            {
                if (!Regex.IsMatch(input.Substring(1, 2), "[0-1]"))
                {
                    SetText("");
                }
            }

            if (input.Length >= 3)
            {
                var value = int.Parse(input.Substring(1, 3));

                if (value > 12)
                {
                    SetText(input.Substring(0, 2));
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


        public override void BackKeyPressed()
        {
            if (BackKeyPressedListener != null)
            {
                BackKeyPressedListener();
            }
        }

        public override void ValidateInput(string input)
        {
            ValidateStartDate(input);
        }
    }
}