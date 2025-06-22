using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Microsoft.Maui.Maps.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMototaxxi.Platforms.Android
{
    internal class CustomInfoWindowClickListener(CustomMapHandler mapHandler) : Java.Lang.Object, GoogleMap.IOnInfoWindowClickListener
    {

        public void OnInfoWindowClick(Marker marker)
        {
            var pin = mapHandler.Markers.FirstOrDefault(x => x.marker.Id == marker.Id);
            pin.pin?.SendInfoWindowClick();
        }

    }
}
