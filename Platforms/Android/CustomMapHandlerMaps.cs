using Android.Gms.Maps.Model;
using Android.Gms.Maps;
using Microsoft.Maui.Maps.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Microsoft.Maui.Maps;
using MiMototaxxi.Model;
using Microsoft.Maui.Platform;

namespace MiMototaxxi.Platforms.Android
{
    public class CustomMapHandlerMaps : MapHandler
    {
        protected override void ConnectHandler(MapView platformView)
        {
            base.ConnectHandler(platformView);
            platformView.GetMapAsync((IOnMapReadyCallback)this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
           /* if (Element?.Pins.Count > 0)
            {
                foreach (var pin in Element.Pins)
                {
                    if (pin is CustomPin customPinMap)
                    {
                        var markerOptions = new MarkerOptions();
                        markerOptions.SetPosition(new LatLng(customPin.Position.Latitude, customPin.Position.Longitude));
                        markerOptions.SetTitle(customPin.Label);
                        markerOptions.SetSnippet(customPin.Address);

                        // Cargar el ícono personalizado
                        var icon = BitmapDescriptorFactory.FromAsset(customPin.IconUrl);
                        markerOptions.SetIcon(icon);

                        googleMap.AddMarker(markerOptions);
                    }
                }
            }*/
        }

    }
}
