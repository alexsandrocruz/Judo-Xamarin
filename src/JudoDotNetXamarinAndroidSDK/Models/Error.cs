using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using Android.Content.Res;
using Android.OS;
using Java.Interop;
using Java.IO;
using Java.Lang;
using JudoPayDotNet.Errors;
using Newtonsoft.Json;
using Exception = System.Exception;

namespace JudoDotNetXamarinSDK.Models
{
    public class Error : Object, IParcelable
    {
        public Exception Exception { get; set; }
        public JudoApiErrorModel ApiError { get; set; }

        public Error(Parcel parcel)
        {
            var exceptionByteLength = parcel.ReadInt();
            if (exceptionByteLength > 0)
            {
                byte[] exceptionBytes = new byte[exceptionByteLength];
                parcel.ReadByteArray(exceptionBytes);

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream(exceptionBytes))
                {
                    Exception = binaryFormatter.Deserialize(stream) as Exception;
                }    
            }

            var apiErrorByteLength = parcel.ReadInt();

            if (apiErrorByteLength > 0)
            {
                var judpApiErrorModel = parcel.ReadString();
                ApiError = JsonConvert.DeserializeObject<JudoApiErrorModel>(judpApiErrorModel) ;
            }
        }

        [ExportField("CREATOR")]
        public static ExceptionCreator InitializeCreator()
        {
            return new ExceptionCreator();
        }

        public Error(Exception exception, JudoApiErrorModel apiError)
        {
            //ToDo : format the exception 
            Exception = exception;
            ApiError = apiError;
        }


        public void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {

            if (Exception != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    binaryFormatter.Serialize(stream, Exception);

                    var exceptionArray = stream.ToArray();
                    var exceptionByteLength = exceptionArray.Length;

                    dest.WriteInt(exceptionByteLength);
                    dest.WriteByteArray(exceptionArray);
                }    
            }
            else
            {
                dest.WriteInt(0);
            }


            if (ApiError != null)
            {
                var apiErrorString = JsonConvert.SerializeObject(ApiError);
                dest.WriteInt(apiErrorString.Length);
                dest.WriteString(apiErrorString);
            }
            else
            {
                dest.WriteInt(0);
            }
            
        }

        public int DescribeContents()
        {
            return 0;
        }

    }

    public class ExceptionCreator : Java.Lang.Object, IParcelableCreator
    {
        public Object CreateFromParcel(Parcel source)
        {
            return new Error(source);
        }

        public Object[] NewArray(int size)
        {
            return new Error[size];
        }
    }
}