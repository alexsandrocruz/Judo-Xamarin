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
using Android.Views.InputMethods;
using Android.Widget;

namespace JudoDotNetXamarinSDK.Ui
{
    public class NoCursorMovingEditText : EditText
    {
        private Action backKeyPressed;

        private class CustomInputConnection : InputConnectionWrapper
        {
            private NoCursorMovingEditText noCursorMovingEditText;

            public CustomInputConnection(IInputConnection target, bool mutable,
                NoCursorMovingEditText noCursorMovingEditText) : base(target, mutable)
            {
                this.noCursorMovingEditText = noCursorMovingEditText;
            }

            public override bool SendKeyEvent(KeyEvent e)
            {
                if (e.Action == KeyEventActions.Down && e.KeyCode == Keycode.Del)
                {
                    noCursorMovingEditText.OnBackKeyPressed();
                }

                return base.SendKeyEvent(e);
            }

            public override bool DeleteSurroundingText(int beforeLength, int afterLength)
            {

                if (beforeLength == 1 && afterLength == 0)
                {
                    // Simulate backspace
                    return SendKeyEvent(new KeyEvent(KeyEventActions.Down, Keycode.Del)) &&
                           SendKeyEvent(new KeyEvent(KeyEventActions.Up, Keycode.Del));
                }

                return base.DeleteSurroundingText(beforeLength, afterLength);
            }
        }

        public NoCursorMovingEditText(Context context, Action backKeyPressed) : base(context)
        {
            this.backKeyPressed = backKeyPressed;
        }

        public NoCursorMovingEditText(Context context, IAttributeSet attributeSet, Action backKeyPressed)
            : base(context, attributeSet)
        {
            this.backKeyPressed = backKeyPressed;
        }

        public NoCursorMovingEditText(Context context, IAttributeSet attributeSet, int defStyle, Action backKeyPressed)
            : base(context, attributeSet, defStyle)
        {
            this.backKeyPressed = backKeyPressed;
        }

        protected void OnBackKeyPressed()
        {
            if (backKeyPressed != null)
            {
                backKeyPressed();    
            }
        }

        protected override void OnSelectionChanged(int selStart, int selEnd)
        {
            string text = Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                if (selStart != text.Length || selEnd != text.Length)
                {
                    SetSelection(text.Length, text.Length);
                    return;
                }
            }

            base.OnSelectionChanged(selStart, selEnd);
        }

        public override Android.Views.InputMethods.IInputConnection OnCreateInputConnection(Android.Views.InputMethods.EditorInfo outAttrs)
        {
            return new CustomInputConnection(base.OnCreateInputConnection(outAttrs), true, this);
        }
    }
}