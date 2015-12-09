using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;
using JudoDotNetXamarinAndroidSDK.Utils;
using JudoPayDotNet.Models;

namespace JudoDotNetXamarinAndroidSDK.Ui
{
    public enum Stage
    {
        STAGE_CC_NO,
        STAGE_CC_EXP,
        STAGE_CC_CV2
    }

    public class CardEntryView : LinearLayout
    {
        

        private CardNumberTextView cardNumberTextView;
        private CardExpiryCV2TextView cardExpiryCv2TextView;
        private CardImageView cardImageView;

        public TextView HintTextView { get; set; }

        private CardType currentCard = CardType.UNKNOWN;

        public Stage CurrentStage = Stage.STAGE_CC_NO;

        public event Action<string> OnCreditCardEntered;
        public event Action<string, string> OnExpireAndCV2Entered;
        public event Action OnReturnToCreditCardNumberEntry;
        public event Action NoLongerComplete;

    

        int NeededCVTwoLength;

        public bool CVTwoComplete;

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

        public CardEntryView (Context context) : base (context)
        {
            Init ();
        }

        public CardEntryView (Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Init ();
        }

        private void Init ()
        {
            base.RemoveAllViews ();

            Orientation = Orientation.Vertical;

            cardImageView = new CardImageView (Context);
            cardImageView.SetCardImageWithoutAnimation (JudoSDKManager.GetCardResourceId (Context, currentCard, true));

            LinearLayout layoutHolder = new LinearLayout (Context);
            layoutHolder.Orientation = Orientation.Horizontal;
            layoutHolder.SetGravity (GravityFlags.CenterVertical);

            LinearLayout cardFrame = new LinearLayout (Context);
            cardFrame.SetGravity (GravityFlags.CenterVertical);
            cardFrame.SetPadding (UiUtils.ToPixels (Context, 8), 0, UiUtils.ToPixels (Context, 8), 0);
            cardFrame.AddView (cardImageView);

            LayoutParams parameters = new LayoutParams (LayoutParams.MatchParent, LayoutParams.WrapContent);
            parameters.Gravity = GravityFlags.Center;

            cardNumberTextView = new CardNumberTextView (Context);
            cardNumberTextView.LayoutParameters = parameters;

            cardExpiryCv2TextView = new CardExpiryCV2TextView (Context, null, Resource.Style.judo_payments_CardText);
            cardExpiryCv2TextView.LayoutParameters = parameters;
            cardExpiryCv2TextView.Visibility = ViewStates.Invisible;

            layoutHolder.AddView (cardFrame, LayoutParams.WrapContent, LayoutParams.MatchParent);
            layoutHolder.AddView (cardNumberTextView);
            layoutHolder.AddView (cardExpiryCv2TextView);

            AddView (layoutHolder);

            int lastPos = 0;
            CardType currentCardType;

            cardNumberTextView.OnEntryComplete += cardNumber => {
                if (OnCreditCardEntered != null) {
                    OnCreditCardEntered (cardNumber);
                }
                SetStage (Stage.STAGE_CC_EXP);
            };
            cardExpiryCv2TextView.OnProgress += position => {
                if (position < (NeededCVTwoLength + 6)) {
                    if (Complete)
                        Complete = false;
                }
            };

            cardNumberTextView.OnProgress += position => {
                if (position == 0) {
                    return;
                }

                string cardNumber = cardNumberTextView.GetText ();
                currentCardType = ValidationHelper.GetCardType (cardNumber);
                if (currentCardType != CardType.UNKNOWN) {
                    //if (currentCardType != currentCard) {
                    if (currentCardType == CardType.AMEX && !JudoSDKManager.AVSEnabled) {
                        cardNumberTextView.ShowInvalid ("AmEx not accepted");
                    } else if (currentCardType == CardType.MAESTRO && !JudoSDKManager.MaestroAccepted) {
                        cardNumberTextView.ShowInvalid ("Maestro not accepted");
                    } else {
                        cardImageView.SetCardImage (JudoSDKManager.GetCardResourceId (Context, currentCardType, true), false);
                        cardNumberTextView.SetHintText (JudoSDKManager.GetCardHintFormat (currentCardType));
                        cardExpiryCv2TextView.SetHintText (JudoSDKManager.GetExpiryAndValidationHintFormat (currentCardType));
                        cardExpiryCv2TextView.SetErrorText (JudoSDKManager.GetExpiryAndValidationErrorMessage (currentCardType));
                    }

                    currentCard = currentCardType;
                    NeededCVTwoLength = (currentCard != CardType.AMEX ? 3 : 4);
                }
                lastPos = position;
            };

            cardExpiryCv2TextView.OnEntryComplete += expiryAndCV2 => {
                string[] temp = expiryAndCV2.Split (' ');
                if (temp.Length < 2) {
                    Log.Error (this.ToString (), "Error: Invalid expiry and/or cv2");
                    return;
                }

                var expiry = temp [0];
                var cv2 = temp [1];
                Complete = true;

                if (OnExpireAndCV2Entered != null) {
                    OnExpireAndCV2Entered (expiry, cv2);
                }
            };

    

            cardExpiryCv2TextView.OnProgress += position => {
                cardExpiryCv2TextView.ValidatePartialInput ();

                if (position > 4) {
                    cardImageView.SetCardImage (JudoSDKManager.GetCardResourceId (Context, currentCard, false), true);
                    if (HintTextView != null) {
                        HintTextView.SetText (Resource.String.enter_card_cv2);
                    }

                    if (position == 5) {
                        try {
                            cardExpiryCv2TextView.ValidateExpiryDate (cardExpiryCv2TextView.GetText ());
                        } catch (Exception e) {
                            cardExpiryCv2TextView.ShowInvalid (e.Message);
                        }
                    }
                } else {
                    cardImageView.SetCardImage (JudoSDKManager.GetCardResourceId (Context, currentCard, true), true);
                    if (HintTextView != null) {
                        HintTextView.SetText (Resource.String.enter_card_expiry);
                    }
                }
            };

            cardExpiryCv2TextView.OnBackKeyPressed = () => {
                if (cardExpiryCv2TextView.GetText ().Length == 0) {
                    cardExpiryCv2TextView.SetText ("");
                    SetStage (Stage.STAGE_CC_NO);
                }
            };
        }

        public void Reset ()
        {
            cardNumberTextView.GetEditText ().Text = "";
            cardExpiryCv2TextView.GetEditText ().Text = "";
            currentCard = CardType.UNKNOWN;
            cardImageView.SetCardImage (JudoSDKManager.GetCardResourceId (Context, currentCard, true), false);

            if (HintTextView != null) {
                HintTextView.SetText (Resource.String.enter_card_no);
            }

            SetStage (Stage.STAGE_CC_NO);

            //Show keyboard
            InputMethodManager imm = (InputMethodManager)Context.GetSystemService (Context.InputMethodService);
            imm.HideSoftInputFromInputMethod (cardNumberTextView.WindowToken, 0);
            imm.ToggleSoftInput (ShowFlags.Forced, 0);
        }

        public string GetCardNumber (bool validation = true)
        {
            var cardNumber = cardNumberTextView.GetText ().Replace (" ", "");

            if (!ValidationHelper.CanProcess (cardNumber) && validation == true) {
                throw new ArgumentException ("Credit card number");
            } else {
                return cardNumber;
            }
        }

        public string GetCardExpiry (bool validation = true)
        {
            return cardExpiryCv2TextView.GetCardExpiry (validation);
        }

        public string GetCardCV2 (bool validation = true)
        {
            return cardExpiryCv2TextView.GetCardCV2 (validation);
        }

        public void RestoreState (String cardNumber, String expiry, String cv2, Stage stage)
        {
            
            cardNumberTextView.SetText (cardNumber);
            SetStage (stage);
            cardExpiryCv2TextView.SetExpiryCV2Text (expiry, cv2);
          
        }

        public void SetStage (Stage stage)
        {
            bool animate = stage != CurrentStage;

            if (stage == Stage.STAGE_CC_EXP || stage == Stage.STAGE_CC_CV2) {
                cardNumberTextView.Enabled = false;
                cardExpiryCv2TextView.Enabled = true;
                cardExpiryCv2TextView.Visibility = ViewStates.Visible;
                // This will last 4 digits of cc# visible on screen
                cardNumberTextView.Visibility = ViewStates.Gone;
                cardExpiryCv2TextView.SetText (" ");
                string cardNumber = cardNumberTextView.GetEditText ().Text;
                cardExpiryCv2TextView.SetLast4NumbersOfCard (cardNumber.Substring (cardNumber.Length - 4));

                cardExpiryCv2TextView.RequestFocus ();

                if (HintTextView != null) {
                    HintTextView.SetText (Resource.String.enter_card_expiry);
                }

                if (animate) {
                    animateInRight (cardExpiryCv2TextView);
                    animateOutLeft (cardNumberTextView);
                }
            } else {
                cardNumberTextView.Enabled = true;
                cardExpiryCv2TextView.Enabled = false;
                cardExpiryCv2TextView.Visibility = ViewStates.Gone;
                cardNumberTextView.Visibility = ViewStates.Visible;
                cardNumberTextView.RequestFocus ();
                if (animate) {
                    animateInLeft (cardNumberTextView);
                    animateOutRight (cardExpiryCv2TextView);
                }
                if (HintTextView != null) {
                    HintTextView.SetText (Resource.String.enter_card_no);
                }
                if (Complete)
                    Complete = false;
                if (OnReturnToCreditCardNumberEntry != null) {
                    OnReturnToCreditCardNumberEntry ();
                }
            }

            CurrentStage = stage;
        }



        private void animateOutLeft (View view)
        {
            animateAlphaTranslate (view, 1, 0, 0, -1, false);
        }

        private void animateOutRight (View view)
        {
            animateAlphaTranslate (view, 1, 0, 0, 1, false);
        }

        private void animateInRight (View view)
        {
            animateAlphaTranslate (view, 0, 1, 1, 0, true);
        }

        private void animateInLeft (View view)
        {
            animateAlphaTranslate (view, 0, 1, -1, 0, true);
        }

        private void animateAlphaTranslate (View view, float alphaFrom, float alphaTo, float xFrom, float xTo,
                                            bool requestFocus)
        {
            AnimationSet animationSet = new AnimationSet (true);

            AlphaAnimation fade = new AlphaAnimation (alphaFrom, alphaTo);
            fade.Duration = 350;

            TranslateAnimation slide = new TranslateAnimation (Dimension.RelativeToSelf, xFrom,
                                           Dimension.RelativeToSelf, xTo,
                                           Dimension.RelativeToSelf, 0,
                                           Dimension.RelativeToSelf, 0);

            slide.Duration = 350;

            animationSet.AddAnimation (fade);
            animationSet.AddAnimation (slide);
            view.StartAnimation (animationSet);

            if (requestFocus) {
                animationSet.AnimationEnd += (obj, args) => view.RequestFocus ();
            }
        }

        

    }
}