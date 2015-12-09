using System;
using CoreFoundation;
using UIKit;
#if __UNIFIED__
// Mappings Unified CoreGraphic classes to MonoTouch classes
using RectangleF = global::CoreGraphics.CGRect;
using SizeF = global::CoreGraphics.CGSize;

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

namespace JudoDotNetXamariniOSSDK.Helpers
{
    internal class LoadingOverlay : UIView
    {
        // control declarations
        UIActivityIndicatorView activitySpinner;
        UILabel loadingLabel;

        public LoadingOverlay(): this(SetFullSize())
        {
            
        }

		public LoadingOverlay(RectangleF frame, bool rounded = false)
            : base(frame)
        {
            // configurable bits
            BackgroundColor = UIColor.Gray;
            Alpha = 0.75f;
			AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleTopMargin;

            nfloat labelHeight = 22;
            nfloat labelWidth = Frame.Width - 20;

            // derive the center x and y
            nfloat centerX = Frame.Width / 2;
            nfloat centerY = Frame.Height / 2;

            // create the activity spinner, center it horizontall and put it 5 points above center x
            activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
			activitySpinner.Frame = new RectangleF(
                centerX - (activitySpinner.Frame.Width / 2),
                centerY - activitySpinner.Frame.Height - 20,
                activitySpinner.Frame.Width,
                activitySpinner.Frame.Height);
            activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
			if (rounded) {
				this.Layer.CornerRadius = 5f;
				this.Layer.MasksToBounds = true;
			}
            AddSubview(activitySpinner);
            activitySpinner.StartAnimating();

            // create and configure the "Loading Data" label
            loadingLabel = new UILabel(new RectangleF(
                centerX - (labelWidth / 2),
                centerY + 20,
                labelWidth,
                labelHeight
                ));
            loadingLabel.BackgroundColor = UIColor.Clear;
            loadingLabel.TextColor = UIColor.White;
		    loadingLabel.Text = "";//"Processing...";
            loadingLabel.TextAlignment = UITextAlignment.Center;
            loadingLabel.AutoresizingMask = UIViewAutoresizing.All;
            AddSubview(loadingLabel);

        }

        /// <summary>
        /// Fades out the control and then removes it from the super view
        /// </summary>
        public void Hide(UIView view)
        {
            DispatchQueue.MainQueue.DispatchAfter(DispatchTime.Now, () =>
            {
                UIView.Animate(
                    0.5, // duration
                    () => { Alpha = 0; },
                    () => { RemoveFromSuperview(); }
                );
            });
        }

        /// <summary>
        /// setting fullsize screen
        /// </summary>
		private static RectangleF SetFullSize()
        {
            // Determine the correct size to start the overlay (depending on device orientation)
            var bounds = UIScreen.MainScreen.Bounds; // portrait bounds
            if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft ||
                UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                bounds.Size = new SizeF(bounds.Size.Height, bounds.Size.Width);
            }

            return bounds;
        }
    }
}