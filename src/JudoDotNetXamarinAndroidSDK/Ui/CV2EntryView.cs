using System;
using System.IO;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using JudoDotNetXamarinAndroidSDK.Utils;
using Orientation = Android.Widget.Orientation;
using JudoPayDotNet.Models;

namespace JudoDotNetXamarinAndroidSDK.Ui
{
    public class CV2EntryView : LinearLayout
    {
        public CardType CurrentCard = CardType.UNKNOWN;

        private CV2TextView cv2TextView;
        private FrameLayout cardImageLayout;
        private TextView last4CCNosTextView;

        public event Action<string> OnCreditCardEntered;
        public event Action<string, string> OnExpiryAndCV2Entered;
        public event Action NoLongerComplete;

        int NeededCVTwoLength;

        private bool _complete;

        bool Complete {
            get{ return _complete; }
            set { 
                if (value == false) {
                    NoLongerComplete ();
                }
                _complete = value;
            }
        }

        public EditText GetCV2EditText ()
        {
            return cv2TextView.GetEditText ();
        }

        public String GetCV2 ()
        {
            string expiryAndCV2 = cv2TextView.GetEditText ().Text;
            string[] temp = expiryAndCV2.Split (' ');
       
            var cv2 = "";
            if (temp.Length > 1) {
                cv2 = temp [1];
            }

            return cv2;
        }

        public String GetExpiry ()
        {
            string expiryAndCV2 = cv2TextView.GetEditText ().Text;
            string[] temp = expiryAndCV2.Split (' ');

         

            string expiry = temp [0];
           

            return expiry;
        }

        public IEditable GetText ()
        {
            return GetCV2EditText ().EditableText;
        }

        public CV2EntryView (Context context) : base (context)
        {
            Init ();
        }

        public CV2EntryView (Context context, IAttributeSet attributeSet) : base (context, attributeSet)
        {
            Init ();
        }

        private void Init ()
        {
            RemoveAllViews ();
 
            Orientation = Orientation.Horizontal;

            LayoutParams lp = new LayoutParams (LayoutParams.WrapContent, LayoutParams.WrapContent);
            lp.Gravity = GravityFlags.CenterVertical;
            LayoutParameters = lp;

            cardImageLayout = new FrameLayout (Context);

            LayoutParams cardImageLayoutParams = new LayoutParams (LayoutParams.WrapContent, LayoutParams.WrapContent);
            cardImageLayoutParams.Gravity = GravityFlags.CenterVertical;
            cardImageLayout.LayoutParameters = cardImageLayoutParams;

            cardImageLayout.SetPadding (0, 0, UiUtils.ToPixels (Context, 8), 0);

            SetCardImageWithoutAnimation (Resource.Drawable.ic_card_cv2);

            cv2TextView = new CV2TextView (Context);

            LayoutParams parameters = new LayoutParams (LayoutParams.WrapContent, LayoutParams.WrapContent);
            parameters.Weight = 1;
            parameters.Gravity = GravityFlags.Center;


            NeededCVTwoLength = (CurrentCard != CardType.AMEX ? 3 : 4);

            cv2TextView.LayoutParameters = parameters;

            cv2TextView.OnEntryComplete += cardNumber => {
                Complete = true;
                if (OnCreditCardEntered != null) {
                    OnCreditCardEntered (cardNumber);
                }
            };

            cv2TextView.OnProgress += position => {
                if (position < (NeededCVTwoLength + 6)) {
                    if (Complete)
                        Complete = false;
                }
            };

            LayoutParams textViewLayoutParams = new LayoutParams (LayoutParams.WrapContent, LayoutParams.MatchParent);
            textViewLayoutParams.Gravity = GravityFlags.Center;

            last4CCNosTextView = new TextView (Context);
            last4CCNosTextView.Gravity = GravityFlags.Center;
            last4CCNosTextView.Text = "0000";
            last4CCNosTextView.LayoutParameters = textViewLayoutParams;
            last4CCNosTextView.SetTypeface (Typeface.Monospace, TypefaceStyle.Normal);
            last4CCNosTextView.TextSize = 18;
            last4CCNosTextView.SetTextColor (Resources.GetColor (Resource.Color.normal_text));
            last4CCNosTextView.Focusable = false;
            last4CCNosTextView.Enabled = false;
            last4CCNosTextView.SetSingleLine ();
            last4CCNosTextView.SetBackgroundDrawable (null);

            AddView (cardImageLayout);
            AddView (last4CCNosTextView);
            AddView (cv2TextView);
        }

        public void SetCardDetails (CardToken cardToken)
        {
            SetLast4CCNosText (cardToken.CardLastFour);
            SetCardType (cardToken.CardType);
        }

        public void RestoreState (string expiry, string cv2)
        {
            cv2TextView.SetText (expiry + " " + cv2);
        }

        public void SetLast4CCNosText (string text)
        {
            last4CCNosTextView.Text = Resources.GetString (Resource.String.card_no_obscured, text);
        }

        public void SetCardType (CardType cardType)
        {
            cv2TextView.SetHintText (JudoSDKManager.GetExpiryAndValidationHintFormat (cardType));
            cv2TextView.SetInputFilter ("/");
            cv2TextView.SetErrorText (JudoSDKManager.GetExpiryAndValidationErrorMessage (cardType));
            SetCardImageWithoutAnimation (JudoSDKManager.GetCardResourceId (Context, cardType, false));
        }

        public void SetCardImageWithoutAnimation (int drawableId)
        {
            ImageView imageView = new ImageView (Context);
            imageView.SetImageResource (drawableId);
            cardImageLayout.RemoveAllViews ();
            cardImageLayout.AddView (imageView);
        }

        public void SetCardImage (int drawableId, bool vertical)
        {
            int objectAnimation = vertical ? Resource.Animation.flipping_out_vert : Resource.Animation.flipping_out;
            int backAnim = Resource.Animation.fade_out;

            CompatibilityAnimation compatibilityAnimationOut = new CompatibilityAnimation (Context, objectAnimation, backAnim);

            if (cardImageLayout.ChildCount > 0) {
                ImageView imageView = (ImageView)cardImageLayout.GetChildAt (0);
                compatibilityAnimationOut.Duration = 350;
                compatibilityAnimationOut.AnimatioEnd += () => Handler.Post (() => cardImageLayout.RemoveView (imageView));

                compatibilityAnimationOut.StartAnimation (imageView);
            }

            ImageView imageView2 = new ImageView (Context);
            imageView2.SetImageResource (drawableId);
            imageView2.Visibility = ViewStates.Invisible;
            cardImageLayout.AddView (imageView2);

            objectAnimation = vertical ? Resource.Animation.flipping_in_vert : Resource.Animation.flipping_in;
            backAnim = Resource.Animation.fade_in;

            CompatibilityAnimation compatibilityAnimationIn = new CompatibilityAnimation (Context, objectAnimation, backAnim);
            compatibilityAnimationIn.Duration = 350;
            compatibilityAnimationIn.Delay = 350;
            compatibilityAnimationIn.AnimationStart += () => imageView2.Visibility = ViewStates.Visible;

            compatibilityAnimationIn.StartAnimation (imageView2);

        }
    }
}