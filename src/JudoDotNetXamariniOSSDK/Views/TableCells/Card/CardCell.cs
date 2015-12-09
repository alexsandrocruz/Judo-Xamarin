using System;
using UIKit;
#if __UNIFIED__
// Mappings Unified CoreGraphic classes to MonoTouch classes

#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreGraphics;
using MonoTouch.ObjCRuntime;
// Mappings Unified types to MonoTouch types
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

namespace JudoDotNetXamariniOSSDK.Views.TableCells.Card
{
	public abstract  class CardCell : UITableViewCell
	{
		public string Key{ get; set; }

		public Action UpdateUI{ get; set; }

		public CardCell (IntPtr handle) : base (handle)
		{
		}

		public abstract CardCell Create ();

		public abstract void SetUpCell ();

		public abstract void DismissKeyboardAction();

	}


}

