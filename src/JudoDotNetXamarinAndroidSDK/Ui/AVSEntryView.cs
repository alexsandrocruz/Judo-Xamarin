using System.Threading;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using JudoDotNetXamarinAndroidSDK.Utils;
using Orientation = Android.Widget.Orientation;
using System;
using System.Collections.Generic;
using JudoDotNetXamarin;
using System.ComponentModel;

namespace JudoDotNetXamarinAndroidSDK.Ui
{

    public class CountryArrayAdapter : ArrayAdapter<string>
    {
        private Typeface typeface;

        public CountryArrayAdapter (Context context, int textViewResourceId, string[] objects, Typeface typeface) : base (context, textViewResourceId, objects)
        {
            this.typeface = typeface;
        }


        public override View GetDropDownView (int position, View convertView, ViewGroup parent)
        {
            View row = base.GetDropDownView (position, convertView, parent);
            if (row is TextView) {
                TextView textRow = (TextView)row;
                textRow.Typeface = typeface;
                textRow.SetTextColor (Context.Resources.GetColor (Resource.Color.default_text));
                textRow.TextSize = 18;
            }

            return row;
        }

        public override View GetView (int position, View convertView, ViewGroup parent)
        {
            View row = base.GetView (position, convertView, parent);
            if (row is TextView) {
                TextView textRow = (TextView)row;
                textRow.Typeface = typeface;
                textRow.SetTextColor (Context.Resources.GetColor (Resource.Color.default_text));
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
        private string[] postcodeText;
        private bool ignoreFocus;

        public AVSEntryView (Context context) : base (context)
        {
            Init ();
        }

        public AVSEntryView (Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Init ();
        }

        private void Init ()
        {
            base.RemoveAllViews ();

            Orientation = Orientation.Vertical;

            View view = Inflate (Context, Resource.Layout.avs, null);
            AddView (view);

            //get the arrays of values from Strings
            postcodeText = Resources.GetStringArray (Resource.Array.avs_countries_postcode_label_text);

            countrySpinner = view.FindViewById<Spinner> (Resource.Id.countrySpinner);
            postCodeContainer = view.FindViewById (Resource.Id.postCodeContainer);
            postCodeEditText = view.FindViewById<EditText> (Resource.Id.postCodeEditText);
            postCodeTitleText = view.FindViewById<TextView> (Resource.Id.postCodeTitleText);
            avsMsgText = view.FindViewById<TextView> (Resource.Id.avsMsgText);

            Typeface type = Typefaces.LoadTypefaceFromRaw (Context, Resource.Raw.courier);
            postCodeEditText.Typeface = type;

            List<string> countries = new List<string> ();
            foreach (BillingCountryOptions option in Enum.GetValues(typeof(BillingCountryOptions))) {
                countries.Add (option.ToDescriptionString ());
            }

            // Populate country spinner
            ArrayAdapter<string> dataAdapter = new CountryArrayAdapter (Context,
                                                   Android.Resource.Layout.SimpleSpinnerItem, countries.ToArray (), type);

            dataAdapter.SetDropDownViewResource (Resource.Layout.country_spinner_dropdown_item);
            countrySpinner.Adapter = dataAdapter;

            countrySpinner.ItemSelected += (sender, args) => {
                postCodeTitleText.Text = postcodeText [args.Position];
                postCodeEditText.Hint = postcodeText [args.Position];
                if (args.Position == 3) {
                    postCodeContainer.Visibility = ViewStates.Invisible;
                    avsMsgText.Visibility = ViewStates.Visible;
                    avsMsgText.BringToFront ();
                } else {
                    postCodeContainer.Visibility = ViewStates.Visible;
                    postCodeContainer.BringToFront ();
                    avsMsgText.Visibility = ViewStates.Invisible;
                    if (!Volatile.Read (ref ignoreFocus)) {
                        postCodeEditText.RequestFocus ();   
                    }
                }
            };
        }

        public BillingCountryOptions GetCountry ()
        {
            if (countrySpinner.SelectedItem == null) {
                return BillingCountryOptions.BillingCountryOptionUK;
            }
            var country = BillingCountryOptions.BillingCountryOptionUK;
            switch (countrySpinner.SelectedItem.ToString ()) {
            case "UK":
                country = BillingCountryOptions.BillingCountryOptionUK;
                break;
            case"USA":
                country = BillingCountryOptions.BillingCountryOptionUSA;

                break;
            case "Can":
                country = BillingCountryOptions.BillingCountryOptionCanada;

                break;
            case "other":
                country = BillingCountryOptions.BillingCountryOptionOther;
 
                break;
         
               
            }
            return country;
        }

        public void RestoreState (int country, string postCode)
        {
            countrySpinner.SetSelection (country);
            postCodeEditText.Text = postCode;
        }

        public string GetPostCode ()
        {
			
            return postCodeEditText.Text;
        }

        public void FocusPostCode ()
        {
            if (postCodeEditText != null) {
                postCodeEditText.RequestFocus ();
            }
        }

        public void InhibitFocusOnFirstShowOfCountrySpinner ()
        {
            Volatile.Write (ref ignoreFocus, true);
        }
    }
}