using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMototaxxi.Platforms.Android
{
    class CustomMarkerClickListener(CustomMapHandler mapHandler) : Java.Lang.Object, GoogleMap.IOnMarkerClickListener
    {
        public bool OnMarkerClick(Marker marker)
        {
            var pin = mapHandler.Markers.FirstOrDefault(x => x.marker.Id == marker.Id);
            pin.pin?.SendMarkerClick();
            marker.ShowInfoWindow();
            return true;
        }
    }
}
