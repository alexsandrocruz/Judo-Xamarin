using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using JudoDotNetXamarinSDK.Utils;

namespace JudoDotNetXamarinSDK.Ui
{
    public class CardNumberTextView : BackgroundHintTextView
    {
        public CardNumberTextView(Context context) : base(context)
        {
            Init();
        }

        public CardNumberTextView(Context context, IAttributeSet attributeSet) : base(context, attributeSet)
        {
            Init();
        }

        public CardNumberTextView(Context context, IAttributeSet attributeSet, int defStyle) : base(context, attributeSet, defStyle)
        {
            Init();
        }

        private void Init()
        {
            SetHintText(JudoSDKManager.CardHintFormat(CardBase.CardType.UNKNOWN));
            SetErrorText("Please recheck number");
            //Set additional chars to skip
            SetText(" ");

            GetEditText().SetSingleLine(true);
        }

        public override void ValidateInput(string input)
        {
            // We have finished entering the cc# let's validate it
            input = input.Replace(" ", "");
            if (!ValidationHelper.CheckLuhn(input))
            {
                throw new Exception("Card number is invalid");
            }
        }

        public override void BackKeyPressed()
        {
        }

        public override void SetHintText(string hintText)
        {
            base.SetHintText(hintText);
            SetInputFilter("");
        }

        protected override float GetContainerHeight()
        {
            return Resources.GetDimension(Resource.Dimension.cardnumber_container_height);
        }
    }
}