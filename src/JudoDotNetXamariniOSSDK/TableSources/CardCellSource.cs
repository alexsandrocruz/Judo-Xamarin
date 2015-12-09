using System;
using System.Collections.Generic;
using Foundation;
using JudoDotNetXamariniOSSDK.Views.TableCells.Card;
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

namespace JudoDotNetXamariniOSSDK.TableSources
{
	internal class CardCellSource : UITableViewSource
	{
	    private List<CardCell> TableItems;

		public CardCellSource (List<CardCell> items)
		{
			TableItems = items;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return TableItems.Count;
		}



		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)  
		{
			var cell = tableView.DequeueReusableCell (TableItems [indexPath.Row].Key);
			if (cell == null) {
				var ccell = (CardCell)TableItems [indexPath.Row];
				ccell.SetUpCell ();
				return ccell;
			}

			return cell;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (TableItems [indexPath.Row].Key);
			if (cell == null) {
				cell = (CardCell)TableItems [indexPath.Row];

			}
			return cell.Bounds.Height;
		}


		public float GetTableHeight ()
		{
			float height = 0f;
			foreach (UITableViewCell cell in TableItems) {
				height += (float)cell.Frame.Height;
			}
			return height;
		}
			
	}
}

