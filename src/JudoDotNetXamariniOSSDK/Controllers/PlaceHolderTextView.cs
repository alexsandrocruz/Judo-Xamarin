/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *
 * This file is part of CreditCard -- an iOS project that provides a smooth and elegant 
 * means to enter or edit credit cards. It was inspired by  a similar form created from 
 * scratch by Square (https://squareup.com/). To see this form in action visit:
 * 
 *   http://functionsource.com/post/beautiful-forms)
 *
 * Copyright 2012 Lot18 Holdings, Inc. All Rights Reserved.
 *
 *
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 *
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *       conditions and the following disclaimer.
 *
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *       of conditions and the following disclaimer in the documentation and/or other materials
 *       provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY Lot18 Holdings ''AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL David Hoerl OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using CoreGraphics;
using Foundation;
using JudoDotNetXamarin;
using UIKit;

#if __UNIFIED__
// Mappings Unified CoreGraphic classes to MonoTouch classes
using RectangleF = global::CoreGraphics.CGRect;
using PointF = global::CoreGraphics.CGPoint;

#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreGraphics;
// Mappings Unified types to MonoTouch types
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

namespace JudoDotNetXamariniOSSDK.Controllers
{
    [Register ("PlaceHolderTextView")]
    internal partial class PlaceHolderTextView : UIView
    {
		
        public  string Text { get; set; }

        public  UIFont Font { get; set; }

        private int _showTextOffset;

        public int ShowTextOffset { get { return _showTextOffset; } }

        public RectangleF Offset { get; set; }


        public PlaceHolderTextView (IntPtr p) : base (p)
        {
            Font = JudoSDKManager.FIXED_WIDTH_FONT_SIZE_20;
        }

        public void SetText (string newText)
        {
            if (newText == Text) {
                return;
            }

            bool animate = (Text != null && newText.Length > Text.Length); 
            Text = newText;

            if (animate) {
                Alpha = (nfloat)0.0;
                Animate (0.25, () => {
                    Alpha = 1;
                });
            }
            SetNeedsDisplay ();
        }

        public void SetShowTextOffSet (int newOffset)
        {
            _showTextOffset = newOffset;
            SetNeedsDisplay ();
        }


        public override void Draw (RectangleF rect)
        {
            RectangleF r = Offset;

            string clearText = Text.Substring (0, ShowTextOffset);
            string grayText = Text.Substring (ShowTextOffset, Text.Length - ShowTextOffset);

            CGContext context = UIGraphics.GetCurrentContext ();

            CGColor clearColor = UIColor.Clear.CGColor;
            context.SetStrokeColor (clearColor);
            context.SetFillColor (clearColor);
            context.FillRect (rect);

            if (clearText.Length != 0) {
                r.Location = new PointF (r.Location.X + clearText.DrawString (Offset.Location, Font).Width, r.Location.Y);
            }

            CSRange charsToDraw = new CSRange (0, grayText.Length);

            if (charsToDraw.Length != 0) {
                CGColor grayColor = UIColor.LightGray.CGColor;

                context.SetStrokeColor (grayColor);
                context.SetFillColor (grayColor);

                var chars = grayText.ToCharArray ();
                int i;
                for (i = 0; i < chars.Length; i++) {
                    char character = chars [i];
                    if (character == ' ') {
                        r.Location = new PointF (r.Location.X + Offset.Size.Width, r.Location.Y);
                        continue;
                    }
                    if (character == 'X') {
                        #if LED_FONT
						RectangleF box = rect.Inset(2, 1);
                        #else
                        CGRect box = rect.Inset (3, 3);
                        #endif
                        nfloat radius = 3;
                        context.BeginPath ();
                        context.MoveTo (box.GetMinX () + radius, box.GetMinY ());
                        var mask = "0";
                        UIFont drawFont = JudoSDKManager.FIXED_WIDTH_FONT_SIZE_20;
                        mask.DrawString (box, drawFont);
                        r.Location = new PointF (r.Location.X + Offset.Size.Width, r.Location.Y);
                        continue;
                    }
                    break;
                }

                charsToDraw.Location += i;
                charsToDraw.Length -= i;
                if (charsToDraw.Length != 0) {					
                    grayText.Substring ((int)charsToDraw.Location, (int)charsToDraw.Length).DrawString (r.Location, JudoSDKManager.FIXED_WIDTH_FONT_SIZE_20);
                }
            }
        }

        public float WidthToOffset ()
        {
            var startText = Text.Substring (0, ShowTextOffset);
            return (float)startText.StringSize (Font).Width;
        }

        public float WidthFromOffset ()
        {
            var endText = Text.Substring (ShowTextOffset, Text.Length - ShowTextOffset);

            return (float)endText.StringSize (Font).Width;
        }
    }
}

