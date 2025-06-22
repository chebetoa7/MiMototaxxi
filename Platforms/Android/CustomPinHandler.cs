using Android.Gms.Maps.Model;
using MiMototaxxi.Model.Map;
using Microsoft.Maui.Maps.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMapPin = Microsoft.Maui.Maps.IMapPin;
using Android.Content;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace MiMototaxxi.Platforms.Android
{
    public class CustomPinHandler : IMapPinHandler, IElementHandler
    {
        private IMauiContext _mauiContext;
        private Context _context;
        private IMapPin _virtualView;

        public CustomPinHandler()
        {
            PlatformView = new MarkerOptions();
        }
        public void UpdateMapPin(IMapPin pin, MarkerOptions marker)
        {
            if (pin == null) return;

            // Verificación adicional de ubicación
            if (pin.Location == null)
                throw new ArgumentNullException("Pin location cannot be null");

            if (pin is CustomPinMap customPin)
            {
                // Conversión segura a LatLng
                var latLng = new LatLng(pin.Location.Latitude, pin.Location.Longitude);

                marker.SetPosition(latLng);
                marker.SetTitle(pin.Label);
                marker.SetSnippet(pin.Address);

                if (!string.IsNullOrEmpty(customPin.IconName))
                {
                    var resourceId = _context.Resources?.GetIdentifier(
                        customPin.IconName.Replace(".png", ""),
                        "drawable",
                        _context.PackageName
                    ) ?? 0;

                    if (resourceId != 0)
                    {
                        marker.SetIcon(BitmapDescriptorFactory.FromResource(resourceId));
                    }
                }
            }
        }

        public MarkerOptions PlatformView { get; }
        public object NativeView => null;

        public IMauiContext MauiContext => _mauiContext;

        public IMapPin VirtualView => _virtualView;

        object? IElementHandler.PlatformView => PlatformView;

        IElement? IElementHandler.VirtualView => VirtualView;

        public void SetMauiContext(IMauiContext mauiContext)
        {
            _mauiContext = mauiContext;
            _context = mauiContext.Context;
        }


        public void DisconnectHandler()
        {
            _context = null;
            _mauiContext = null;
            _virtualView = null;
        }

        public void Invoke(string command, object args)
        {
            // No necesario para este handler
        }

        public void SetVirtualView(IElement view)
        {
            if (view is IMapPin pin)
            {
                _virtualView = pin;
            }
        }

        public void UpdateValue(string property)
        {
            // Implementación básica requerida
            if (property == "IconName" && _virtualView is CustomPinMap pin)
            {
                UpdateMapPin(pin, PlatformView);
            }
        }
    }
}
