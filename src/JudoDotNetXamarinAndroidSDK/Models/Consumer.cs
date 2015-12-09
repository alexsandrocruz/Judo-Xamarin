using System;
using Android.OS;
using Java.Interop;
using Object = Java.Lang.Object;

namespace JudoDotNetXamarinSDK.Models
{
    public class Consumer : Object, IParcelable
    {
        public string ConsumerToken { get; set; }
        public string YourConsumerReference { get; set; }

        [ExportField("CREATOR")]
        public static ConsumerCreator InitializeCreator()
        {
            return new ConsumerCreator();
        }

        public Consumer() { }

        public Consumer(string consumerReference, string consumerToken = null)
        {
            YourConsumerReference = consumerReference;
            ConsumerToken = consumerToken;
        }

        public Consumer(Parcel inParcel)
        {
            ConsumerToken = inParcel.ReadString();
            YourConsumerReference = inParcel.ReadString();
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
            dest.WriteString(ConsumerToken);
            dest.WriteString(YourConsumerReference);
        }

        public override string ToString()
        {
            return String.Format(@"Consumer{{consumerToken='{0}', yourConsumerReference='{1}'}}", ConsumerToken,
                YourConsumerReference);
        }
    }

    public class ConsumerCreator : Java.Lang.Object, IParcelableCreator
    {
        public Object CreateFromParcel(Parcel source)
        {
            return new Consumer(source);
        }

        public Object[] NewArray(int size)
        {
            return new Consumer[size];
        }
    }
}