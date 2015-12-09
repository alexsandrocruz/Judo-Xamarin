using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;
using JudoDotNetXamarinSDK.Utils;
using Orientation = Android.Widget.Orientation;

namespace JudoDotNetXamarinSDK.Ui
{

    public class CountryArrayAdapter : ArrayAdapter<string>
    {
        private Typeface typeface;

        public CountryArrayAdapter(Context context, int textViewResourceId, string[] objects, Typeface typeface) : base(context, textViewResourceId, objects)
        {
            this.typeface = typeface;
        }


        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            View row = base.GetDropDownView(position, convertView, parent);
            if (row is TextView)
            {
                TextView textRow = (TextView) row;
                textRow.Typeface = typeface;
                textRow.SetTextColor(Context.Resources.GetColor(Resource.Color.default_text));
                textRow.TextSize = 18;
            }

            return row;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = base.GetView(position, convertView, parent);
            if (row is TextView)
            {
                TextView textRow = (TextView) row;
                textRow.Typeface = typeface;
                textRow.SetTextColor(Context.Resources.GetColor(Resource.Color.default_text));
                textRow.TextSize = 18;
            }
            return row;
        }
    }

    public class AVSEntryView : LinearLayout
    {
        private Spinner countrySpinner;
        private EditText postCodeEditText;
        private TextView postCodeTitleText;
        private TextView avsMsgText;
        private View postCodeContainer;
        private string[] countries;
        private string[] postcodeText;
        private bool ignoreFocus;

        public AVSEntryView(Context context) : base(context)
        {
            Init();
        }

        public AVSEntryView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        private void Init()
        {
            base.RemoveAllViews();

            Orientation = Orientation.Vertical;

            View view = Inflate(Context, Resource.Layout.avs, null);
            AddView(view);

            //get the arrays of values from Strings
            countries = Resources.GetStringArray(Resource.Array.avs_countries);
            postcodeText = Resources.GetStringArray(Resource.Array.avs_countries_postcode_label_text);

            countrySpinner = view.FindViewById<Spinner>(Resource.Id.countrySpinner);
            postCodeContainer = view.FindViewById(Resource.Id.postCodeContainer);
            postCodeEditText = view.FindViewById<EditText>(Resource.Id.postCodeEditText);
            postCodeTitleText = view.FindViewById<TextView>(Resource.Id.postCodeTitleText);
            avsMsgText = view.FindViewById<TextView>(Resource.Id.avsMsgText);

            Typeface type = Typefaces.LoadTypefaceFromRaw(Context, Resource.Raw.courier);
            postCodeEditText.Typeface = type;

            // Populate country spinner
            ArrayAdapter<string> dataAdapter = new CountryArrayAdapter(Context,
                Android.Resource.Layout.SimpleSpinnerItem, countries, type);

            dataAdapter.SetDropDownViewResource(Resource.Layout.country_spinner_dropdown_item);
            countrySpinner.Adapter = dataAdapter;

            countrySpinner.ItemSelected += (sender, args) =>
            {
                postCodeTitleText.Text = postcodeText[args.Position];
                postCodeEditText.Hint = postcodeText[args.Position];
                if (args.Position == 3)
                {
                    postCodeContainer.Visibility = ViewStates.Invisible;
                    avsMsgText.Visibility = ViewStates.Visible;
                    avsMsgText.BringToFront();
                }
                else
                {
                    postCodeContainer.Visibility = ViewStates.Visible;
                    postCodeContainer.BringToFront();
                    avsMsgText.Visibility = ViewStates.Invisible;
                    if (!Volatile.Read(ref ignoreFocus))
                    {
                        postCodeEditText.RequestFocus();   
                    }
                }
            };
        }

        public string GetCountry()
        {
            return (string)countrySpinner.SelectedItem;
        }

        public string GetPostCode()
        {
            return postCodeEditText.Text;
        }

        public void FocusPostCode()
        {
            if (postCodeEditText != null)
            {
                postCodeEditText.RequestFocus();
            }
        }

        public void InhibitFocusOnFirstShowOfCountrySpinner()
        {
            Volatile.Write(ref ignoreFocus, true);
        }
    }
}