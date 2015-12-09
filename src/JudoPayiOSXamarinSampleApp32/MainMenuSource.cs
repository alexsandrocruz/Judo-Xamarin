using System;
using System.Collections.Generic;
using System.Linq;

#if__UNIFIED__
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
	public class MainMenuSource: UITableViewSource
	{

		Dictionary<string,Action> ButtonDictionary;
		KeyValuePair<string,Action>[] ButtonArray;
		string CellIdentifier = "genericCell";
		List<UITableViewCell> TableCells;

		public MainMenuSource (Dictionary<string,Action> buttonDictionary)
		{
			TableCells = new List<UITableViewCell> ();
			ButtonDictionary = buttonDictionary;
			ButtonArray = buttonDictionary.ToArray ();
			foreach (var buttonProperty in ButtonArray) {
				var cell = new UITableViewCell ();
				cell.TextLabel.Text = buttonProperty.Key;
				TableCells.Add (cell);
			}
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return ButtonDictionary.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			UITableViewCell cell = tableView.DequeueReusableCell (CellIdentifier);
			cell = TableCells [indexPath.Row];
			cell.IndentationLevel = 0;

			if (cell != null) {
				return cell;
			} else {
				cell = new UITableViewCell (UITableViewCellStyle.Default, CellIdentifier);
				cell.TextLabel.Text = ButtonArray [indexPath.Row].Key;
				return cell;
			}
		}

		public float GetTableHeight ()
		{
			float height = 0f;
			foreach (UITableViewCell cell in TableCells) {
				height += (float)cell.Frame.Height;
			}
			return height;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			ButtonArray [indexPath.Row].Value.Invoke ();
			var cell = TableCells [indexPath.Row];
			cell.SetSelected (false, false);
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{

			UITableViewCell cell = TableCells [indexPath.Row];
			return cell.Bounds.Height;
		}
	}
}

