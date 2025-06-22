//using Android.Graphics.Drawables;
using Maui.GoogleMaps;
using MiMototaxxi.ViewModel.HomeMototaxi;
using MiMototaxxi.ViewModel.HomePasajero;
using MiMototaxxi.ViewModel.Pasajero;

namespace MiMototaxxi.View.HomePasajeroPage;

public partial class HomePasajePage : ContentPage
{
    #region Vars
    public readonly HomePasajeViewModel ViewModel;
    #endregion
    public HomePasajePage()
	{
		InitializeComponent();
        BindingContext = ViewModel = new HomePasajeViewModel();
        // Inicializar mapa al aparecer la página
        this.Appearing += async (sender, e) =>
        {
            await ViewModel.InitializeMap();
            UpdateMap();
        };

        // Actualizar  PropertyChanged
        ViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ViewModel.CurrentPosition))
            {
                UpdateMap();
            }
        };
    }
    private async void UpdateMap()
    {
        if (ViewModel?.CurrentPosition == null) return;

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            // Actualizar pins (código existente)
            if (ViewModel.UserPin == null)
            {
                ViewModel.UserPin = new Pin
                {
                    Label = "Tu ubicación",
                    Position = ViewModel.CurrentPosition,
                    Type = PinType.Place,
                };
            }

            if (ViewModel.UserPinMoto != null)
            {
                ViewModel.UserPinMoto.Icon = BitmapDescriptorFactory.FromBundle("mi_icono");
            }

            MyMap.Pins.Clear();
            MyMap.Pins.Add(ViewModel.UserPin);

            if (ViewModel.UserPinMoto != null)
            {
                MyMap.Pins.Add(ViewModel.UserPinMoto);
                AdjustMapToFitPins();
            }
            else
            {
                // Vista por defecto si solo hay un pin
                MyMap.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                        ViewModel.CurrentPosition,
                        Distance.FromKilometers(1.5)));
            }
        });
    }

    private void AdjustMapToFitPins()
    {
        try
        {
            // 1. Calcular el rectángulo que contiene ambos pins
            var positions = new List<Position>
        {
            ViewModel.CurrentPosition,
            ViewModel.UserPinMoto.Position
        };

            var minLat = positions.Min(p => p.Latitude);
            var maxLat = positions.Max(p => p.Latitude);
            var minLon = positions.Min(p => p.Longitude);
            var maxLon = positions.Max(p => p.Longitude);

            // 2. Añadir un margen del 40% alrededor de los puntos
            var latMargin = (maxLat - minLat) * 0.8;
            var lonMargin = (maxLon - minLon) * 0.8;

            minLat -= latMargin;
            maxLat += latMargin;
            minLon -= lonMargin;
            maxLon += lonMargin;

            // 3. Crear una lista de posiciones para los bordes
            var boundingBox = new List<Position>
        {
            new Position(minLat, minLon),
            new Position(minLat, maxLon),
            new Position(maxLat, minLon),
            new Position(maxLat, maxLon)
        };

            // 4. Mover el mapa para mostrar toda el área
            MyMap.MoveToRegion(MapSpan.FromPositions(boundingBox));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error ajustando mapa: {ex.Message}");

            // Fallback: Zoom por distancia
            var distance = CalculateDistance(ViewModel.CurrentPosition, ViewModel.UserPinMoto.Position);
            var radius = Math.Max(distance * 1.8, 1600); // Mínimo 1km

            MyMap.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                    new Position(
                        (ViewModel.CurrentPosition.Latitude + ViewModel.UserPinMoto.Position.Latitude) / 2,
                        (ViewModel.CurrentPosition.Longitude + ViewModel.UserPinMoto.Position.Longitude) / 2),
                    Distance.FromMeters(radius)));
        }
    }
    private double CalculateDistance(Position pos1, Position pos2)
    {
        // Fórmula Haversine mejorada
        const double R = 6371000; // Radio de la Tierra en metros
        var dLat = ToRadians(pos2.Latitude - pos1.Latitude);
        var dLon = ToRadians(pos2.Longitude - pos1.Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(pos1.Latitude)) *
                Math.Cos(ToRadians(pos2.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
    /*
    private async void UpdateMap()
    {
        if (ViewModel?.CurrentPosition == null) return;

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            // 1. Actualizar los pins (tu código existente)
            if (ViewModel.UserPin == null)
            {
                ViewModel.UserPin = new Pin
                {
                    Label = "Tu ubicación",
                    Position = ViewModel.CurrentPosition,
                    Type = PinType.Place
                };
            }
            else
            {
                ViewModel.UserPin.Position = ViewModel.CurrentPosition;
            }

            MyMap.Pins.Clear();
            MyMap.Pins.Add(ViewModel.UserPin);

            if (ViewModel.UserPinMoto != null)
            {
                MyMap.Pins.Add(ViewModel.UserPinMoto);
            }

            // 2. Calcular la región óptima
            if (ViewModel.UserPinMoto != null)
            {
                var positions = new List<Position>
            {
                ViewModel.CurrentPosition,
                ViewModel.UserPinMoto.Position
            };

                // Calcular los límites
                var minLat = positions.Min(p => p.Latitude);
                var maxLat = positions.Max(p => p.Latitude);
                var minLon = positions.Min(p => p.Longitude);
                var maxLon = positions.Max(p => p.Longitude);

                // Calcular centro
                var center = new Position(
                    (minLat + maxLat) / 2,
                    (minLon + maxLon) / 2
                );

                // Calcular distancia aproximada en grados
                var latDelta = (maxLat - minLat) * 1.5; // Margen adicional
                var lonDelta = (maxLon - minLon) * 1.5;

                // Asegurar un mínimo de zoom
                latDelta = Math.Max(latDelta, 0.01); // ~1km
                lonDelta = Math.Max(lonDelta, 0.01);

                // Crear región
                var span = new MapSpan(center, latDelta, lonDelta);
                MyMap.MoveToRegion(span);
            }
            else
            {
                // Vista por defecto si solo hay un pin
                MyMap.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                        ViewModel.CurrentPosition,
                        Distance.FromKilometers(1)));
            }
        });
    }
    */
    /* private async void UpdateMap()
     {
         if (ViewModel?.CurrentPosition == null) return;

         await MainThread.InvokeOnMainThreadAsync(() =>
         {
             // Actualizar pin del usuario
             if (ViewModel.UserPin == null || string.IsNullOrEmpty(ViewModel.UserPin.Label))
             {
                 ViewModel.UserPin = new Pin
                 {
                     Label = "Ubicación actual",
                     Position = ViewModel.CurrentPosition,
                     Type = PinType.Place
                 };
             }
             else
             {
                 ViewModel.UserPin.Position = ViewModel.CurrentPosition;
             }

             // Actualizar pins en el mapa
             MyMap.Pins.Clear();
             MyMap.Pins.Add(ViewModel.UserPin);

             // Agregar pin del mototaxista si existe
             if (ViewModel.UserPinMoto != null)
             {
                 MyMap.Pins.Add(ViewModel.UserPinMoto);
             }

             // Mover el mapa
             if (ViewModel.UserPinMoto != null)
             {
                 // Centrar para mostrar ambos pins
                 var positions = new List<Position>
             {
                 ViewModel.CurrentPosition,
                 ViewModel.UserPinMoto.Position
             };
                 MyMap.MoveToRegion(MapSpan.FromPositions(positions));
             }
             else
             {
                 // Solo centrar en el usuario
                 MyMap.MoveToRegion(
                     MapSpan.FromCenterAndRadius(
                         ViewModel.CurrentPosition,
                         Distance.FromKilometers(0.5)),
                     animate: true);
             }
             if (ViewModel.UserPinMoto != null)
             {
                 // Opción 2: Zoom fijo que muestra bien ambos puntos
                 var positions = new List<Position>
                 {
                     ViewModel.CurrentPosition,
                     ViewModel.UserPinMoto.Position
                 };

                 // Ajustamos el padding para mejor visualización
                 MyMap.MoveToRegion(
                     MapSpan.FromPositions(positions).WithZoom(200),
                     animate: true);
             }
             else
             {
                 MyMap.MoveToRegion(
                     MapSpan.FromCenterAndRadius(
                         ViewModel.CurrentPosition,
                         Distance.FromKilometers(1)),
                     animate: true);
             }
         });
     }*/

    /*private void UpdateMap()
    {
        if (ViewModel?.CurrentPosition == null) return;

        // Asegurarnos que el UserPin existe y tiene Label
        if (ViewModel.UserPin == null || string.IsNullOrEmpty(ViewModel.UserPin.Label))
        {
            ViewModel.UserPin = new Pin
            {
                Label = "Ubicación actual",
                Position = ViewModel.CurrentPosition,
                Type = PinType.Place
            };
        }
        else
        {
            ViewModel.UserPin.Position = ViewModel.CurrentPosition;
        }

        // Mover el mapa
        MyMap.MoveToRegion(
            MapSpan.FromCenterAndRadius(
                ViewModel.CurrentPosition,
                Distance.FromKilometers(0.2)),
            animate: true);

        // Actualizar pins
        MyMap.Pins.Clear();
        MyMap.Pins.Add(ViewModel.UserPin);
    }*/

    [Obsolete]
    protected override void OnAppearing()
    {
        base.OnAppearing();

        MessagingCenter.Subscribe<HomePasajeViewModel, Pin>(this, "UpdateMotoPin", async (sender, pin) =>
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                // Eliminar el pin anterior si existe
                var existingPin = MyMap.Pins.FirstOrDefault(p => p.Label == "Mototaxista");
                if (existingPin != null)
                {
                    MyMap.Pins.Remove(existingPin);
                }

                // Agregar el nuevo pin
                MyMap.Pins.Add(pin);

                // Centrar el mapa para mostrar ambos pins
                var positions = new List<Position>
            {
                ViewModel.CurrentPosition,
                pin.Position
            };
                MyMap.MoveToRegion(MapSpan.FromPositions(positions));
            });
        });

        // Resto del código...
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<HomePasajeViewModel>(this, "UpdateMotoPin");
    }

}