using System;
using System.Collections.Generic;
using System.IO;
using Android.Content;
using Android.Graphics;
using Android.Util;

namespace JudoDotNetXamarinAndroidSDK.Utils
{
    public static class Typefaces
    {
        private static readonly string TAG = "Typefaces";
        private static readonly Dictionary<string, Typeface> cache = new Dictionary<string, Typeface> ();

        public static Typeface Get (Context context, string assetPath)
        {
            lock (cache) {
                if (!cache.ContainsKey (assetPath)) {
                    try {
                        Typeface t = Typeface.CreateFromAsset (context.Assets, assetPath);
                        cache [assetPath] = t;
                    } catch (Exception e) {
                        Log.Error (JudoSDKManager.DEBUG_TAG,
                            "Could not get typeface '" + assetPath + "' because " + e.Message);
                        return null;
                    }
                }

                return cache [assetPath];
            }
        }

        public static Typeface LoadTypefaceFromRaw (Context context, int resource)
        {
            Typeface tf = null;

            Stream inputStream = context.Resources.OpenRawResource (resource);
            string path = context.FilesDir + "/judo_fonts";

            if (!Directory.Exists (path)) {
                try {
                    Directory.CreateDirectory (path);
                } catch (Exception) {
                    return null;
                }
            }

            string outPath = path + "/courier.ttf";

            if (File.Exists (outPath)) {
                tf = Typeface.CreateFromFile (outPath);
            } else {
                try {
                    byte[] buffer = new byte[inputStream.Length];
                    BufferedStream bufferedStream = new BufferedStream (File.Open (outPath, FileMode.CreateNew));
                    int length = 0;
                    while ((length = inputStream.Read (buffer, 0, buffer.Length)) > 0) {
                        bufferedStream.Write (buffer, 0, length);
                    }
                    bufferedStream.Close ();
                    tf = Typeface.CreateFromFile (outPath);
                } catch (Exception) {
                    return null;
                }
            }
            return tf;
        }
    }
}