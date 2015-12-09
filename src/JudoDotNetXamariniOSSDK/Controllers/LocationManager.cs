using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if __UNIFIED__
using Foundation;
using UIKit;
using CoreFoundation;
using CoreGraphics;
using CoreLocation;
// Mappings Unified CoreGraphic classes to MonoTouch classes
using RectangleF = global::CoreGraphics.CGRect;
using SizeF = global::CoreGraphics.CGSize;
using PointF = global::CoreGraphics.CGPoint;
#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreLocation;
// Mappings Unified types to MonoTouch types
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

namespace JudoDotNetXamariniOSSDK
{
	internal class LocationManager
    {
        private bool stopped;
        private CLLocation _currentLocationAquired;

        CLLocationManager locationManager;

        LocationManager()
        {
            locationManager = new CLLocationManager();
            locationManager.DesiredAccuracy = CLLocation.AccuracyKilometer; 

            locationManager.LocationsUpdated += (sender, e) => {
	            var loc  = e.Locations;
                if (loc[0] != null) 
                {
                    return;
                }

                if (stopped)
                {
                    return;
                }

                _currentLocationAquired = loc[0];

                locationManager.StopUpdatingLocation();
                stopped = false;
            };
        }

        public CLLocation currentLocationAquired 
        {
            get { return _currentLocationAquired; } 
            set { _currentLocationAquired = value; } 
        }

        void GetCurrentLocation()
        {
            locationManager.StartUpdatingLocation();
        }
 
    }
}