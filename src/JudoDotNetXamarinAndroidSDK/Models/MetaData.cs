using System;
using System.Collections.Generic;
using Android.OS;
using Java.Interop;
using Newtonsoft.Json;
using Object = Java.Lang.Object;

namespace JudoDotNetXamarinSDK.Models
{
    public class MetaData : Object, IParcelable
    {
        public Dictionary<string, string> Metadata { get; set; }
            
        [ExportField("CREATOR")]
        public static MetaDataCreator InitializeCreator()
        {
            return new MetaDataCreator();
        }

        public MetaData() { }

        public MetaData(Dictionary<string, string> metadata)
        {
            Metadata = metadata;
        }

        public MetaData(Parcel inParcel)
        {
            //There is a string in the parcel
            if (inParcel.ReadInt() > 0)
            {
                Metadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(inParcel.ReadString());
            }
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
            if (Metadata != null)
            {
                var metaDataString = JsonConvert.SerializeObject(Metadata);
    
                dest.WriteInt(metaDataString.Length);
                dest.WriteString(metaDataString);
            }
            else
            {
                dest.WriteInt(0);
            }
            
        }
    }

    public class MetaDataCreator : Java.Lang.Object, IParcelableCreator
    {
        public Object CreateFromParcel(Parcel source)
        {
            return new MetaData(source);
        }

        public Object[] NewArray(int size)
        {
            return new MetaData[size];
        }
    }
}