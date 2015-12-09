using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using JudoDotNetXamarinAndroidSDK.Utils;

namespace JudoDotNetXamarinAndroidSDK.Ui
{
    public class StartDateIssueNumberEntryView : LinearLayout
    {
        public interface IEntryCompleteListener
        {
            void OnStartDateEntered (string startDate);

            void OnIssueNumberEntered (string issueNumber);
        }

        private StartDateEditText startDateEditText;
        private IssueNumberEditText issueNumberEditText;

        public IEntryCompleteListener EntryCompleteListener { get; set; }


        public StartDateIssueNumberEntryView (Context context) : base (context)
        {
            Init ();
        }

        public StartDateIssueNumberEntryView (Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Init ();
        }

        private void Init ()
        {
            Orientation = Orientation.Horizontal;
            SetGravity (GravityFlags.Center);
            WeightSum = 2;
            SetPadding (0, UiUtils.ToPixels (Context, 16), 0, 0);

            LayoutInflater.From (Context).Inflate (Resource.Layout.startdate_and_issuenumber, this, true);

            startDateEditText = FindViewById<StartDateEditText> (Resource.Id.startDateEditText);
            startDateEditText.OnEntryComplete += startDate => {
                if (EntryCompleteListener != null) {
                    EntryCompleteListener.OnStartDateEntered (startDate);
                }

                //SetFocuts to the issue
                issueNumberEditText.RequestFocus ();
            };

            issueNumberEditText = FindViewById<IssueNumberEditText> (Resource.Id.issueNumberEditText);

            //because the issue number could be 1,2, or 3 char, we have to listen for focus
            issueNumberEditText.FocusChange += (sender, args) => {
                if (!args.HasFocus) {
                    if (!String.IsNullOrWhiteSpace (issueNumberEditText.GetText ())) {
                        var issueStart = issueNumberEditText.GetText ();
                        if (ValidationHelper.CheckIssueNumber (issueStart) && EntryCompleteListener != null) {
                            EntryCompleteListener.OnIssueNumberEntered (issueStart);
                        }
                    }
                }
            };
        }

        public string GetStartDate ()
        {
            return startDateEditText.GetText ();
        }

        public string GetIssueNumber ()
        {
            return issueNumberEditText.GetText ();
        }

        public bool Validate ()
        {
            var issueNumberStr = issueNumberEditText.GetText ();

            int issueNumber;

            return int.TryParse (issueNumberStr, out issueNumber);
        }

        public void RestoreState (string startDate, string issueNumber)
        {
            startDateEditText.SetText (startDate);
            issueNumberEditText.SetText (issueNumber);
        }

        public void Reset ()
        {
            startDateEditText.SetText (String.Empty);
            issueNumberEditText.SetText (String.Empty);
        }
    }
}