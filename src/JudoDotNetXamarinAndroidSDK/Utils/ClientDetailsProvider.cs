using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Systems;
using Android.Telephony;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;
using String = System.String;

namespace JudoDotNetXamarinSDK.Utils
{
    public static class ClientDetailsProvider
    {
        public static ClientDetails GetClientDetails(Context context)
        {
            if (!JudoSDKManager.Configuration.IsFraudMonitoringSignals)
            {
                return null;
            }

            var connectivityManager =
                context.GetSystemService(Context.ConnectivityService).JavaCast<ConnectivityManager>();
            var telephonyManager = context.GetSystemService(Context.TelephonyService).JavaCast<TelephonyManager>();

            var clientDetails = new ClientDetails();

            clientDetails.OS = "android " + Build.VERSION.SdkInt;
            clientDetails.DeviceId = new DeviceUuidFactory(context).GetDeviceUuid();
            clientDetails.DeviceModel = Build.Model;
            clientDetails.Serial = Build.Serial;

            clientDetails.CultureLocale = Locale.Default.ISO3Country;

            try
            {
                clientDetails.IsRoaming = connectivityManager != null && connectivityManager.ActiveNetworkInfo.IsRoaming;
            }
            catch (SecurityException e)
            {
                Log.Warn("Not enough permissions to read ActiveNetworkInfo", e);
            }

            

            if (telephonyManager != null)
            {
                clientDetails.NetworkName = telephonyManager.NetworkOperatorName != String.Empty ? telephonyManager.NetworkOperatorName : telephonyManager.SimOperatorName;
            }

            RootCheck rootCheck = new RootCheck(context);
            clientDetails.Root = rootCheck.BuildRootCheckDetails();

            clientDetails.SSLPinningEnabled = JudoSDKManager.Configuration.IsSSLPinningEnabled;

            return clientDetails;
        }
    }

    public class ClientDetails
    {
        public string OS { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceId { get; set; }
        public bool IsRoaming { get; set; }
        public string NetworkName { get; set; }
        public string CultureLocale { get; set; }
        public string Root { get; set; }
        public string Serial { get; set; }
        public bool SSLPinningEnabled { get; set; }
    }
}