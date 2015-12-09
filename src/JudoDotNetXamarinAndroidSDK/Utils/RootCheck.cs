using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Java.IO;
using Java.Util;
using Java.Interop;

namespace JudoDotNetXamarinAndroidSDK.Utils
{
    public class RootCheck
    {
        private const char LINE = '\n';
        private readonly Context context;
        private readonly StringBuilder logBuilder;
        PackageManager packageManager;

        private readonly String[] knownRootAppsPackages = new [] { "com.noshufou.android.su",
            "eu.chainfire.supersu", "com.koushikdutta.superuser"
        };

        private readonly String[] knownDangerousAppsPackages = new[] { "com.koushikdutta.rommanager",
            "com.dimonvideo.luckypatcher", "com.chelpus.lackypatch"
        };

        private readonly String[] pathsArray = new[] { "/sbin/", "/system/bin/", "/system/xbin/",
            "/data/local/xbin/", "/data/local/bin/", "/system/sd/xbin/",
            "/system/bin/failsafe/", "/data/local/"
        };

        private readonly Dictionary<string, string> dangerousProps = new Dictionary<string, string> {
            { "ro.debuggable", "1" },
            { "ro.secure", "0" }
        };

        public RootCheck ()
        {
            logBuilder = new StringBuilder ();
            context = Android.App.Application.Context;
            packageManager = context.PackageManager; 
        }

        public string BuildRootCheckDetails ()
        {
            bool rooted = IsRooted ();
            if (!rooted) {
                logBuilder.Append ("root not detected");
            }

            return logBuilder.ToString ();
        }

        public bool IsRooted ()
        {
            bool rootManagment = DetectRootManagementApps ();
            bool potentiallyDangerousApps = DetectPotentiallyDangerousApps ();
            bool suBinary = CheckForBinary ("su");
            bool busyboxBinary = CheckForBinary ("busybox");
            bool dangerousProps = CheckForDangerousProps ();
            bool rwSystem = CheckForRWSystem ();
            bool testKeys = DetectTestKeys ();

            return rootManagment || potentiallyDangerousApps || suBinary || busyboxBinary || dangerousProps ||
            rwSystem || testKeys;
        }

        public bool DetectTestKeys ()
        {
            string buildTags = Build.Tags;

            return !String.IsNullOrWhiteSpace (buildTags) && buildTags.Contains ("test-keys");
        }

        public bool DetectRootManagementApps ()
        {
            bool result = false;

            foreach (var packageName in knownRootAppsPackages) {
                try {
                    packageManager.GetPackageInfo (packageName, 0);
                    logBuilder.AppendFormat ("{0} ROOT management app detected!{1}", packageName, LINE);
                    result = true;
                } catch (PackageManager.NameNotFoundException) {
                    // Exception thrown, package is not installed into the system
                    continue;
                    ;
                }
            }

            return result;
        }

        public bool DetectPotentiallyDangerousApps ()
        {
            bool result = false;

            foreach (var packageName in knownDangerousAppsPackages) {
                try {
                    packageManager.GetPackageInfo (packageName, 0);
                    logBuilder.AppendFormat ("{0} potentially dangerous app detected!{1}", packageName, LINE);
                    result = true;
                } catch (PackageManager.NameNotFoundException) {
                    // Exception thrown, package is not installed into the system
                    continue;
                    ;
                }
            }

            return result;
        }

        public bool CheckForBinary (string filename)
        {
            bool result = false;

            foreach (var path in pathsArray) {
                var completePath = path + filename;
                File f = new File (completePath);
                bool fileExists = f.Exists ();
                if (fileExists) {
                    logBuilder.AppendFormat ("{0} binary detected!{1}", completePath, LINE);
                    result = true;
                }
            }

            return result;
        }

        private string[] propsReader ()
        {
            System.IO.Stream inputStream = null;
            try {
                inputStream = Java.Lang.Runtime.GetRuntime ().Exec ("getprop").InputStream;
            } catch (Exception) {
                //Normal behaviour    
            }

            var propval = string.Empty;

            try {
                propval = new Scanner (inputStream).UseDelimiter ("\\A").Next ();
            } catch (NoSuchElementException) {
                //Normal behaviour    
            }

            return propval.Split ('\n');
        }

        private string[] MountReader ()
        {
            System.IO.Stream inputStream = null;
            try {
                inputStream = Java.Lang.Runtime.GetRuntime ().Exec ("mount").InputStream;
            } catch (Exception) {
                //Normal behaviour    
            }

            var propval = string.Empty;

            try {
                propval = new Scanner (inputStream).UseDelimiter ("\\A").Next ();
            } catch (NoSuchElementException) {
                //Normal behaviour    
            }

            return propval.Split ('\n');
        }

        public bool CheckForDangerousProps ()
        {
            bool result = false;

            string[] lines = propsReader ();
            foreach (var line in lines) {
                foreach (var key in dangerousProps.Keys) {
                    if (line.Contains (key)) {
                        string badValue;
                        dangerousProps.TryGetValue (key, out badValue);
                        badValue = "[" + badValue + "]";
                        if (line.Contains (badValue)) {
                            logBuilder.AppendFormat ("{0} = {1} detected!{2}", key, badValue, LINE);
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        public bool CheckForRWSystem ()
        {
            bool result = false;

            string[] lines = MountReader ();

            foreach (var line in lines) {
                if (line.Contains ("/system") && line.Contains (" rw,")) {
                    logBuilder.AppendFormat ("System partition mounted with rw permissions!{0}", LINE);
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}