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
    public class CardImageView : FrameLayout
    {
        private int currentDrawableId = -1;

        public CardImageView(Context context) : base(context)
        {
        }

        public CardImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public CardImageView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public void SetCardImageWithoutAnimation(int drawableId)
        {
            currentDrawableId = drawableId;
            ImageView imageView = new ImageView(Context);
            imageView.SetImageResource(drawableId);
            AddView(imageView);
        }

        public void SetCardType(CardBase.CardType cardType)
        {
            SetCardImageWithoutAnimation(JudoSDKManager.GetCardResourceId(Context, cardType, true));            
        }

        public void SetCardImage(int drawbleId, bool vertical)
        {
            // If this is already our card image, don't reanimate
            if (drawbleId == currentDrawableId)
            {
                return;
            }

            currentDrawableId = drawbleId;

            int objAnim = vertical ? Resource.Animation.flipping_out_vert : Resource.Animation.flipping_out;
            int bakAnim = Resource.Animation.fade_out;

            CompatibilityAnimation compatibilityAnimationOut = new CompatibilityAnimation(Context, objAnim, bakAnim);

            if (ChildCount > 0)
            {
                ImageView imageView = (ImageView) GetChildAt(0);
                compatibilityAnimationOut.Duration = 350;
                compatibilityAnimationOut.AnimatioEnd = () => Handler.Post(() => RemoveView(imageView));
                compatibilityAnimationOut.StartAnimation(imageView);
            }

            ImageView imageView2 = new ImageView(Context);
            imageView2.SetImageResource(drawbleId);
            imageView2.Visibility = ViewStates.Invisible;
            AddView(imageView2);

            objAnim = vertical ? Resource.Animation.flipping_in_vert : Resource.Animation.flipping_in;
            bakAnim = Resource.Animation.fade_in;
            CompatibilityAnimation compatibilityAnimationIn = new CompatibilityAnimation(Context, objAnim, bakAnim);
            compatibilityAnimationIn.Duration = 350;
            compatibilityAnimationIn.Delay = 350;
            compatibilityAnimationIn.AnimationStart += () =>
            {
                imageView2.Visibility = ViewStates.Visible;
            };
            compatibilityAnimationIn.StartAnimation(imageView2);
        }
    }
}