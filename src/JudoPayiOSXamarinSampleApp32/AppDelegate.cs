using System;
using JudoDotNetXamariniOSSDK;
using Environment = JudoPayDotNet.Enums.Environment;


#if __UNIFIED__
using Foundation;
using UIKit;

// Mappings Unified CoreGraphic classes to MonoTouch classes
using RectangleF = global::CoreGraphics.CGRect;
using SizeF = global::CoreGraphics.CGSize;
using PointF = global::CoreGraphics.CGPoint;
#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
	// Mappings Unified types to MonoTouch types
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif


namespace JudoPayiOSXamarinSampleApp
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		public override UIWindow Window {
			get;
			set;
		}


		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{  
			Window = new UIWindow (UIScreen.MainScreen.Bounds); 
			RootView root = new RootView ();
			Window.RootViewController = new UINavigationController (root);
			Window.TintColor = UIColor.Black;
			Window.MakeKeyAndVisible ();
			ConfigureJudoSettings ();
			return true;   
		}
			
		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation (UIApplication application)
		{
		}
		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
		}
		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
		}
		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}

		void ConfigureJudoSettings ()
		{
			//Configure JudoPay app here
			var configInstance = JudoConfiguration.Instance;

			//setting for Sandnox
			configInstance.Environment = Environment.Live;

			configInstance.ApiToken = "5tZfrXDngpvu8iGS";
			configInstance.ApiSecret = "da36e4c8f5805173060c934b12dcc14bb05761af310ea364cd787710b1da346b";
			configInstance.JudoId = "100515592";

//			configInstance.ApiToken = "MzEtkQK1bHi8v8qy";
//			configInstance.ApiSecret = "c158b4997dfc7595a149a20852f7af2ea2e70bd2df794b8bdbc019cc5f799aa1";
//			configInstance.JudoId = "100915867";

            /*
            // setting up 3d secure, AVS, Amex and mestro card support
		    JudoSDKManager.AVSEnabled = true;
		    JudoSDKManager.AmExAccepted = true;
		    JudoSDKManager.MaestroAccepted = true;
            
            // this will turn off UI mode and you can use same judo APIs to link with your own UI
		    //JudoSDKManager.UIMode = false;
            */
		}
	}


}
