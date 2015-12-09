using System;
using Android.Content;

using Android.Util;

using JudoDotNetXamarinAndroidSDK.Utils;
using JudoPayDotNet.Models;

namespace JudoDotNetXamarinAndroidSDK.Ui
{
    public class CardNumberTextView : BackgroundHintTextView
    {
        public CardNumberTextView (Context context) : base (context)
        {
            Init ();
        }

        public CardNumberTextView (Context context, IAttributeSet attributeSet) : base (context, attributeSet)
        {
            Init ();
        }

        public CardNumberTextView (Context context, IAttributeSet attributeSet, int defStyle) : base (context, attributeSet, defStyle)
        {
            Init ();
        }

        private void Init ()
        {
            SetHintText (JudoSDKManager.CardHintFormat (CardType.UNKNOWN));
            SetErrorText ("Please recheck number");
            //Set additional chars to skip
            SetText (" ");

            GetEditText ().SetSingleLine (true);
        }

        public override void ValidateInput (string input)
        {
            // We have finished entering the cc# let's validate it
            input = input.Replace (" ", "");
            if (!ValidationHelper.CheckLuhn (input)) {
                throw new Exception ("Card number is invalid");
            }
        }

        public override void BackKeyPressed ()
        {
        }

        public override void SetHintText (string hintText)
        {
            base.SetHintText (hintText);
            SetInputFilter ("");
        }

        protected override float GetContainerHeight ()
        {
            return Resources.GetDimension (Resource.Dimension.cardnumber_container_height);
        }
    }
}