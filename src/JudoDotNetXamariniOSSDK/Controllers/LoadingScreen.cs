using CoreGraphics;
using JudoDotNetXamariniOSSDK.Helpers;
using UIKit;

namespace JudoDotNetXamariniOSSDK.Controllers
{
	public class LoadingScreen
	{
		// get the top most view 
		private static UIView appView = UIApplication.SharedApplication.Windows [0].RootViewController.View;
		private static LoadingOverlay _loadPop;
		/// <summary>
		/// shows loading screen while processing payment
		/// </summary>
		/// <param name="view"></param>
		internal static void ShowLoading (UIView view)
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				_loadPop = new LoadingOverlay (new CGRect ((view.Frame.Width / 2) - 75f, (view.Frame.Height / 2) - 75f, 150f, 150f), true);
			} else {
				view = UIApplication.SharedApplication.Windows[0].RootViewController.View;
				_loadPop = new LoadingOverlay();
				_loadPop.Frame = view.Frame;
			}
			view.Add (_loadPop);
		}

		/// <summary>
		/// hides loading screen while processing payment
		/// </summary>
		internal static void HideLoading ()
		{
			if (_loadPop != null)
			{
				_loadPop.Hide (appView);
				_loadPop.Dispose();
			}
		}

	}
}

