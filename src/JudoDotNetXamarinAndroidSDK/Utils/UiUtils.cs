using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using JudoDotNetXamarin;
using JudoDotNetXamarinAndroidSDK.Ui;
using JudoPayDotNet.Models;

namespace JudoDotNetXamarinAndroidSDK.Utils
{
    public static class UiUtils
    {
        public static int ToPixels (Context context, float dips)
        {
            DisplayMetrics metrics = new DisplayMetrics ();
            var wm = context.GetSystemService (Context.WindowService).JavaCast<IWindowManager> ();
            wm.DefaultDisplay.GetMetrics (metrics);
            return (int)(dips * ((float)metrics.DensityDpi / 160)); // px = dp * (dpi / 160)
        }

        public static int ConvertCountryToISO (string country)
        {
            if (string.IsNullOrWhiteSpace (country)) {
                if (country.Equals ("uk", StringComparison.InvariantCultureIgnoreCase)) {
                    return 826;
                } else {
                    if (country.Equals ("usa", StringComparison.InvariantCultureIgnoreCase)) {
                        return 840;
                    } else {
                        if (country.Equals ("canada", StringComparison.InvariantCultureIgnoreCase)) {
                            return 124;
                        }
                    }
                }
            }

            //default
            return 0;
        }

        public static void GatherCardDetails (CardEntryView cardEntryView, AVSEntryView avsEntryView, StartDateIssueNumberEntryView startDateEntryView, out CardViewModel viewModel)
        {
            var cardNumber = cardEntryView.GetCardNumber ();
            var expiryDate = cardEntryView.GetCardExpiry ();
            var cv2 = cardEntryView.GetCardCV2 ();
            BillingCountryOptions country = BillingCountryOptions.BillingCountryOptionUK;
            CardAddressModel cardAddress = new CardAddressModel ();

            if (JudoSDKManager.AVSEnabled) {
                country = avsEntryView.GetCountry ();
                cardAddress.PostCode = avsEntryView.GetPostCode ();
            }

            string startDate = null;
            string issueNumber = null;

            if (JudoSDKManager.MaestroAccepted) {
                issueNumber = startDateEntryView.GetIssueNumber ();
                startDate = startDateEntryView.GetStartDate ();
            }


            var cardPayment = new CardViewModel () {
                CardNumber = cardNumber,
                CountryCode = country.GetISOCode (),
                CV2 = cv2,
                ExpireDate = expiryDate,
                IssueNumber = issueNumber,
                StartDate = startDate,
                PostCode = cardAddress.PostCode,    
            };

            viewModel = cardPayment;
        }
    }
}