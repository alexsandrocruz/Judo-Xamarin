using System;
using Android.Content;
using Android.Util;


namespace JudoDotNetXamarinAndroidSDK.Ui
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