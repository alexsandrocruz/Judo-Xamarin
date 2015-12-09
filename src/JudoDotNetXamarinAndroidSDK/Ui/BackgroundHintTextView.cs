using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using Java.Util;
using JudoDotNetXamarinSDK.Utils;
using Exception = System.Exception;

namespace JudoDotNetXamarinSDK.Ui
{
    public abstract class BackgroundHintTextView : RelativeLayout
    {
        private class Filter : Java.Lang.Object, IInputFilter
        {
            private BackgroundHintTextView backgroundHintTextView;

            public Filter(BackgroundHintTextView background)
            {
                backgroundHintTextView = background;
            }

            public void Dispose()
            {
            }

            public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
            {
                for (int i = start; i < end; ++i)
                {
                    if (!Character.IsDigit(source.CharAt(i)) && !backgroundHintTextView.IsCharInFilter(source.CharAt(i)))
                    {
                        return new Java.Lang.String("");
                    }
                }

                return null;
            }
        }

        private EditText textTextView;
        private TextView hintTextView;

        private List<int> skipCharsAtPositions = new List<int>();
        private string hintText = "";
        private int beforeTextSize;
        private string filterString = "";
        private TextView textErrorView;
        private string errorText;
        private bool nextMustBeDeleted = false;
        private LinearLayout linearLayout;
        private RelativeLayout textLayout;

        public event Action<string> OnEntryComplete;
        public event Action<int> OnProgress;

        public BackgroundHintTextView(Context context) : base(context)
        {
            Init();
        }

        public BackgroundHintTextView(Context context, IAttributeSet attributes) : base(context, attributes)
        {
            Init();
        }

        public BackgroundHintTextView(Context context, IAttributeSet attributes, int defStyle) : base(context, attributes, defStyle)
        {
            Init();
        }

        private void Init()
        {
            base.RemoveAllViews();

            hintTextView = new EditText(Context);
            textTextView = new NoCursorMovingEditText(Context, BackKeyPressed);

            linearLayout = new LinearLayout(Context);
            linearLayout.LayoutParameters = new ViewGroup.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)GetContainerHeight());

            textLayout = new RelativeLayout(Context);
            textLayout.LayoutParameters = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.MatchParent, 1);
            textLayout.SetGravity(GravityFlags.Center);

            linearLayout.AddView(textLayout);

            textTextView.SetTextAppearance(Context, Resource.Style.judo_payments_CardText);
            hintTextView.SetTextAppearance(Context, Resource.Style.judo_payments_HintText);

            LayoutParams lp = new LayoutParams(LinearLayout.LayoutParams.MatchParent,
                LinearLayout.LayoutParams.WrapContent);
            lp.AddRule(LayoutRules.CenterVertical);

            hintTextView.LayoutParameters = lp;
            textTextView.LayoutParameters = lp;
            hintTextView.Enabled = false;
            hintTextView.Focusable = false;

            textTextView.InputType = InputTypes.ClassNumber | InputTypes.TextFlagNoSuggestions;
            hintTextView.InputType = InputTypes.ClassNumber | InputTypes.TextFlagNoSuggestions;

            hintTextView.SetBackgroundColor(Resources.GetColor(Android.Resource.Color.Transparent));
            textTextView.SetBackgroundColor(Resources.GetColor(Android.Resource.Color.Transparent));
            int horizontalPadding =
                Resources.GetDimensionPixelOffset(Resource.Dimension.backgroundhinttextview_horizontal_padding);
            hintTextView.SetPadding(horizontalPadding, 0 , horizontalPadding, 0);
            textTextView.SetPadding(horizontalPadding, 0, horizontalPadding, 0);

            textErrorView = new EditText(Context);
            RelativeLayout.LayoutParams errorLP = new RelativeLayout.LayoutParams(LayoutParams.MatchParent,
                LayoutParams.WrapContent);
            errorLP.AddRule(LayoutRules.CenterVertical);

            int margin = UiUtils.ToPixels(Context, 2);
            errorLP.SetMargins(margin, 0, margin, 0);

            textErrorView.LayoutParameters = errorLP;
            textErrorView.Enabled = false;
            textErrorView.Focusable = false;
            textErrorView.Visibility = ViewStates.Gone;
            textErrorView.SetBackgroundColor(Color.Red);
            int errorVerticalPadding =
                Context.Resources.GetDimensionPixelOffset(
                    Resource.Dimension.backgroundhinttextview_error_vertical_padding);
            textErrorView.SetPadding(horizontalPadding, errorVerticalPadding, horizontalPadding, errorVerticalPadding);
            textErrorView.Text = errorText;
            textErrorView.SetSingleLine(true);
            textErrorView.Gravity = GravityFlags.Center;
            textErrorView.SetTextAppearance(Context, Resource.Style.judo_payments_ErrorText);

            //set courier font
            Typeface type = Typefaces.LoadTypefaceFromRaw(Context, Resource.Raw.courier);
            //hintTextView.Typeface = type;
            hintTextView.SetTypeface(Typeface.Monospace, TypefaceStyle.Normal);
            //textTextView.Typeface = type;
            textTextView.SetTypeface(Typeface.Monospace, TypefaceStyle.Normal);
            textErrorView.Typeface = type;


            textLayout.AddView(hintTextView);
            textLayout.AddView(textTextView);

            AddView(linearLayout);

            IEnumerable<char> previousCharSequence;
            EventHandler<TextChangedEventArgs> beforeTextChanged = (sender, args) =>
                                                                    {
                                                                        previousCharSequence = args.Text;
                                                                    };

            EventHandler<TextChangedEventArgs> textChanged = (sender, args) =>
                                                            {
                                                                beforeTextSize = args.BeforeCount;
                                                            };

            EventHandler<AfterTextChangedEventArgs> afterTextChanged = null;

            
            Action<string> updateTextView = newText =>
            {
                textTextView.TextChanged -= textChanged;
                textTextView.BeforeTextChanged -= beforeTextChanged;
                textTextView.AfterTextChanged -= afterTextChanged;

                textTextView.Text = newText;

                textTextView.TextChanged += textChanged;
                textTextView.BeforeTextChanged += beforeTextChanged;
                textTextView.AfterTextChanged += afterTextChanged;
            };

            afterTextChanged = (sender, args) =>
            {
                var length = args.Editable.ToString().Length;
                var deleting = beforeTextSize == 1;

                if (deleting)
                {
                    nextMustBeDeleted = false;
                }
                else
                {
                    if (nextMustBeDeleted && length > 0)
                    {
                        updateTextView(args.Editable.SubSequence(0, length - 1));
                        UpdateHintTextForCurrentTextEntry();
                        return;
                    }
                }

                // If we are deleting (we've just removed a space char, so delete another char please:
                // Or if we've pressed space don't allow it!
                if ((deleting && skipCharsAtPositions.Contains(length)) ||
                    (length > 0 && IsCharInFilter(args.Editable.CharAt(length - 1)) &&
                     !skipCharsAtPositions.Contains(length - 1)))
                {
                    updateTextView(length == 0 ? "" : args.Editable.SubSequence(0, length - 1));
                    textTextView.SetSelection(length == 0 ? 0 : length - 1);
                    UpdateHintTextForCurrentTextEntry();
                    return;
                }

                // Adds a non numeric char at positions needed
                for (int i = 0; i < skipCharsAtPositions.Count; ++i)
                {
                    // We rescan all letters recursively to catch when a users pastes into the edittext
                    int charPosition = skipCharsAtPositions[i];
                    if (length > charPosition)
                    {
                        if (hintText[charPosition] != args.Editable.ToString()[charPosition])
                        {
                            updateTextView(args.Editable.SubSequence(0, charPosition) + "" + hintText[charPosition] +
                                           args.Editable.SubSequence(charPosition, args.Editable.Length()));
                            UpdateHintTextForCurrentTextEntry();
                            return;
                        }
                    }
                    else
                    {
                        if (length == charPosition)
                        {
                            updateTextView(textTextView.Text + "" + hintText[charPosition]);
                        }
                    }
                }

                UpdateHintTextForCurrentTextEntry();

                // We've got all the chars we need, fire off our listener
                if (length >= LengthToStartValidation())
                {
                    try
                    {
                        ValidateInput(args.Editable.ToString());
                        if (OnEntryComplete != null)
                        {
                            OnEntryComplete(args.Editable.ToString());
                        } 
                        return;
                    }
                    catch (Exception exception)
                    {
                        Log.Error(JudoSDKManager.DEBUG_TAG, exception.Message, exception);
                    }

                    ShowInvalid();
                }
                else
                {
                    if (OnProgress != null)
                    {
                        OnProgress(length);
                    }
                }
            };

            textTextView.BeforeTextChanged += beforeTextChanged;

            textTextView.TextChanged += textChanged;

            textTextView.AfterTextChanged += afterTextChanged;
        }

        public bool IsCharInFilter(char c)
        {
            return filterString.Contains(c);
        }

        public EditText GetEditText()
        {
            return textTextView;
        }

        protected TextView GetHintText()
        {
            return hintTextView;
        }

        public string GetText()
        {
            return textTextView.Text;
        }

        public void SetText(string text)
        {
            textTextView.Text = text;
        }

        public void SetErrorText(string errorText)
        {
            this.errorText = errorText;
            textErrorView.Text = errorText;
        }

        public virtual void SetHintText(string hintText)
        {
            this.hintText = hintText;
            hintTextView.Text = hintText;
            UpdateHintTextForCurrentTextEntry();
        }

        protected virtual int LengthToStartValidation()
        {
            return hintText.Length;
        }

        private void UpdateHintTextForCurrentTextEntry()
        {
            string hintText = "";
            for (int i = 0; i < this.hintText.Length; ++i)
            {
                if (i < textTextView.Length())
                {
                    hintText += " ";
                }
                else
                {
                    hintText += this.hintText.Substring(i, 1);
                }
            }
            hintTextView.Text = hintText;
        }

        public void SetHintTextVisible(bool visible)
        {
            hintTextView.Visibility = visible ? ViewStates.Visible : ViewStates.Invisible;
        }

        public void SetRealHintText(string hint)
        {
            textTextView.Text = hint;
        }

        public void SetOnFocusChangeListener(Action<View, bool> focusListener)
        {
            textTextView.FocusChange += (sender, args) => focusListener(textTextView, args.HasFocus);
        }

        public virtual void SetInputFilter(string filter)
        {
            filterString = " " + filter;
            textTextView.SetFilters(new IInputFilter[]{new InputFilterLengthFilter(hintText.Length), new Filter(this) });

            skipCharsAtPositions.Clear();
            for (int i = 0; i < hintText.Length; ++i)
            {
                if (IsCharInFilter(hintText[i]))
                {
                    skipCharsAtPositions.Add(i);
                }
            }
        }

        protected virtual float GetContainerHeight()
        {
            return Resources.GetDimension(Resource.Dimension.default_backgroundhinttextview_container_height);
        }

        public abstract void BackKeyPressed();

        public abstract void ValidateInput(string input);

        public void ShowInvalid()
        {
            ShowInvalid(errorText);
        }

        public void ShowInvalid(string errorMessage)
        {
            nextMustBeDeleted = true;
            textErrorView.Text = errorMessage;
            AddView(textErrorView);
            textErrorView.Visibility = ViewStates.Visible;

            AnimationSet animationSet = new AnimationSet(true);

            AlphaAnimation fadeIn = new AlphaAnimation(0, 1);
            fadeIn.Duration = 300;

            AlphaAnimation fadeOut = new AlphaAnimation(1, 0);
            fadeOut.StartOffset = 300 + 300;
            fadeOut.Duration = 300;

            animationSet.AddAnimation(fadeIn);
            animationSet.AddAnimation(fadeOut);

            animationSet.FillAfter = true;
            animationSet.FillBefore = true;

            animationSet.AnimationEnd += (sender, args) =>
            {
                textErrorView.Visibility = ViewStates.Gone;
                RemoveView(textErrorView);
            };

            textErrorView.StartAnimation(animationSet);
        }

        public void AddFixedText(string text)
        {
            EditText fixedText = new EditText(Context);
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            layoutParams.Gravity = GravityFlags.CenterVertical;
            fixedText.SetTextAppearance(Context, Resource.Style.judo_payments_CardText);
            fixedText.LayoutParameters = layoutParams;
            fixedText.Enabled = false;
            fixedText.Focusable = false;
            fixedText.Text = text;
            fixedText.SetBackgroundColor(Resources.GetColor(Android.Resource.Color.Transparent));
            fixedText.SetPadding(0,0,0,0);
            Typeface type = Typefaces.LoadTypefaceFromRaw(Context, Resource.Raw.courier);
            fixedText.Typeface = type;

            if (linearLayout.ChildCount > 1)
            {
                linearLayout.RemoveViewAt(0);
            }
            linearLayout.AddView(fixedText, 0);
        }
    }
}