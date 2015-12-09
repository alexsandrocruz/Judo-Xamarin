using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace JudoDotNetXamarinSDK.Utils
{
    public static class UiUtils
    {
        public static int ToPixels(Context context, float dips)
        {
            DisplayMetrics metrics = new DisplayMetrics();
            var wm = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            wm.DefaultDisplay.GetMetrics(metrics);
            return (int) (dips*((float) metrics.DensityDpi/160)); // px = dp * (dpi / 160)
        }

        public static int ConvertCountryToISO(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                if (country.Equals("uk", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 826;
                }
                else
                {
                    if (country.Equals("usa", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return 840;
                    }
                    else
                    {
                        if (country.Equals("canada", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 124;
                        }
                    }
                }
            }

            //default
            return 0;
        }
    }
}