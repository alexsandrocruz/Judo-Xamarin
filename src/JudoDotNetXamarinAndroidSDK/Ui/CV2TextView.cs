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

namespace JudoDotNetXamarinSDK.Ui
{
    public class CV2TextView : BackgroundHintTextView
    {
        public Action OnBackKeyPressed { get; set; }

        public CV2TextView(Context context) : base(context)
        {
            Init();
        }

        public CV2TextView(Context context, IAttributeSet attributeSet) : base(context, attributeSet)
        {
            Init();
        }

        public CV2TextView(Context context, IAttributeSet attributeSet, int defStyle)
            : base(context, attributeSet, defStyle)
        {
            Init();
        }

        private void Init()
        {
            SetHintText(Resources.GetString(Resource.String.payment_hint_cv2));
        }

        public override void ValidateInput(string input)
        {
            
        }

        public override void BackKeyPressed()
        {
            if (OnBackKeyPressed != null)
            {
                OnBackKeyPressed();
            }
        }
    }
}