using System;
using System.ComponentModel;
using JudoDotNetXamarin;

namespace JudoDotNetXamarinAndroidSDK
{
    internal static class EnumExtensions
    {
        public static string ToDescriptionString (this Enum val)
        {
            EnumDescriptionAttribute[] attributes = (EnumDescriptionAttribute[])val.GetType ().GetField (val.ToString ()).GetCustomAttributes (typeof(EnumDescriptionAttribute), false);
            return attributes.Length > 0 ? attributes [0].Description : string.Empty;
        }
    }
}

