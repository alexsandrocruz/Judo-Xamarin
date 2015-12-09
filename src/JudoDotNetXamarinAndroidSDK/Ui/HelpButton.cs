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
using Java.Security;

namespace JudoDotNetXamarinSDK.Ui
{
    public class HelpButton : LinearLayout
    {
        public delegate void HelpClickListenerHandler(bool isHelp);

        private ImageView img;
        private readonly bool isHelp = true;
        public event HelpClickListenerHandler HelpClickListener; 

        public HelpButton(Context context) : base(context, null)
        {
        }

        public HelpButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetGravity(GravityFlags.Center);

            img = new ImageView(context);
            img.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            int sourceResourceId = attrs.GetAttributeResourceValue("http://schemas.android.com/apk/res/android", "src", 0);

            img.SetImageResource(sourceResourceId);

            Click += (sender, args) =>
            {
                if (HelpClickListener != null)
                {
                    HelpClickListener(isHelp);
                }
            };

            AddView(img);
        }
    }
}