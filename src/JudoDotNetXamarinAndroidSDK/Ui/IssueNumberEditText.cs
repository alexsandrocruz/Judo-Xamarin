using System;
using Android.Content;
using Android.Text;
using Android.Util;
using JudoDotNetXamarinAndroidSDK.Utils;

namespace JudoDotNetXamarinAndroidSDK.Ui
{
    public class IssueNumberEditText : BackgroundHintTextView
    {
        public Action BackKeyPressedListener { get; set; }

        public IssueNumberEditText(Context context) : base(context)
        {
            Init();
        }

        public IssueNumberEditText(Context context, IAttributeSet attributes) : base(context, attributes)
        {
            Init();
        }

        public IssueNumberEditText(Context context, IAttributeSet attributes, int defStyle) : base(context, attributes, defStyle)
        {
            Init();
        }

        private void Init()
        {
            SetInputFilter("");
        }

        protected override int LengthToStartValidation()
        {
            return 1;
        }

        public override void SetInputFilter(string filter)
        {
            GetEditText().SetFilters(new IInputFilter[]{ new InputFilterLengthFilter(2)});
        }

        public void ValidateIssueNumberDuringEntry(string issueNumber)
        {
            if ("0" != issueNumber && !ValidationHelper.CheckIssueNumber(issueNumber))
            {
                var message = Resources.GetString(Resource.String.msg_check_issue_number);
                SetErrorText(message);
                throw new Exception(message);
            }
            else
            {
                if ("00" == issueNumber)
                {
                    var message = Resources.GetString(Resource.String.msg_check_issue_number);
                    SetErrorText(message);
                    throw new Exception(message);
                }
            }
        }

        public string GetCardIssueNumber()
        {
            return GetEditText().Text;
        }


        public override void BackKeyPressed()
        {
            if (BackKeyPressedListener != null)
            {
                BackKeyPressedListener();
            }
        }

        public override void ValidateInput(string input)
        {
            ValidateIssueNumberDuringEntry(input);
        }
    }
}