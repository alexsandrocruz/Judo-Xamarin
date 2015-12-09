using System;
using System.Net.NetworkInformation;
using JudoShieldXamarin;


#if __UNIFIED__
using Foundation;
using UIKit;
using CoreFoundation;
using CoreGraphics;
using ObjCRuntime;
// Mappings Unified CoreGraphic classes to MonoTouch classes
using RectangleF = global::CoreGraphics.CGRect;
using SizeF = global::CoreGraphics.CGSize;
using PointF = global::CoreGraphics.CGPoint;
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

namespace JudoDotNetXamariniOSSDK.Utils
{
    internal static class ClientDetailsProvider
    {
        public static ClientDetails GetClientDetails()
        {
            if (!JudoSDKManager.RiskSignals)
            {
                return null;
            }

            var clientDetails = new ClientDetails
            {
                OS = "iOS " + UIDevice.CurrentDevice.SystemVersion,
                DeviceId = GetDeviceMacAddress(),
                DeviceModel = UIDevice.CurrentDevice.Model,
				Serial = JudoShield.GetDeviceIdentifier(),
                CultureLocale = NSLocale.CurrentLocale.CountryCode,
                SslPinningEnabled = JudoSDKManager.SSLPinningEnabled
            };


            return clientDetails;
        }

        private static string GetDeviceMacAddress()
        {
            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    var address = netInterface.GetPhysicalAddress();
                    return BitConverter.ToString(address.GetAddressBytes());

                }
            }

            return "NoMac";
        }
    }

	internal class ClientDetails
    {
        public string OS { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceId { get; set; }
        public string CultureLocale { get; set; }
        public string Serial { get; set; }
        public bool SslPinningEnabled { get; set; }
        // couldn't find the direct API support for following things there are some work around with native APIs
        public bool IsRoaming { get; set; }
        public string NetworkName { get; set; }
    }
}