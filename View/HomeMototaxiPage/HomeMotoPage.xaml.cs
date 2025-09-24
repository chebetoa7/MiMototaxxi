using Maui.GoogleMaps;
using MiMototaxxi.ViewModel.HomeMototaxi;
using Plugin.Firebase.CloudMessaging;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace MiMototaxxi.View.HomeMototaxiPage
{
    public partial class HomeMotoPage : ContentPage
    {
        public HomeMotoViewModel ViewModel { get; }

        public HomeMotoPage()
        {
            InitializeComponent();
            BindingContext = ViewModel = new HomeMotoViewModel();

            // Inicializar mapa al aparecer la p�gina
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
                else if (e.PropertyName == nameof(ViewModel.FixedPin))
                {
                    UpdateMap();
                }
            };
            CrossFirebaseCloudMessaging.Current.NotificationReceived += (s, notification) =>
            {
                try
                {
                    var dtoNotification = notification.Notification.Body.ToString();
                    // Console.WriteLine($"Notificaci�n recibida: {notification.Notification.Body}");//dtDataNotification[0].ToString()
                    var dtDataNotification = notification.Notification.Data.Values.ToList();
                    Console.WriteLine($"Notificación recibida: {dtDataNotification[3].ToString() ?? "NA"}");
                    //_ = ViewModel.OpenSolicitudViaje(dtoNotification);
                    //OpenSolicitudViaje(dtoNotification);
                    _ = OpenSolicitudViaje(dtDataNotification[3].ToString());
                    ViewModel.IsSolicitudViaje = true;
                }
                catch (Exception exM)
                {
                    Console.WriteLine("Error: CrossFirebaseCluodMessaging" + exM.Message);
                }
            };
        }
        private double ParseCoordinate(string coordinateValue)
        {
            try
            {
                // Normalizar el formato: reemplazar coma por punto
                string normalizedValue = coordinateValue.Trim().Replace(",", ".");
                return Double.Parse(normalizedValue, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing coordinate '{coordinateValue}': {ex.Message}");
                return 0;
            }
        }
        Double latDestino = 0f;
        Double lonDestino = 0f;
        public async Task OpenSolicitudViaje(string datoEntrada)
        {
            string[] partes = datoEntrada.Split('|');

            // Inicializar variables para almacenar los datos
            string ubicacion2 = "";
            string pasajero2 = "";
            string distancia = "";
            string idViaje = "";

            // Recorrer cada parte para extraer la informaci�n
            foreach (string parte in partes)
            {
                if (parte.Contains("Ubicacion:"))
                {
                    ubicacion2 = parte.Replace("Ubicacion:", "").Trim();
                }
                else if (parte.Contains("pasajero:"))
                {
                    pasajero2 = parte.Replace("pasajero:", "").Trim(); // Ojo con la escritura ("pasajero" vs "pasajero")
                }
                else if (parte.Contains("Distancia:"))
                {
                    distancia = parte.Replace("Distancia:", "").Trim().Replace("Km", "");

                }
                else if (parte.Contains("idViaje:"))
                {
                    idViaje = parte.Replace("idViaje:", "").Trim();
                }
                else if (parte.Contains("Lat:"))
                {
                    var ltDestino = parte.Replace("Lat:", "").Trim();
                    double rawLat = ParseCoordinate(ltDestino);

                    if (Math.Abs(rawLat) > 90)
                    {
                        latDestino = rawLat / 1000000.0;
                        Console.WriteLine($"[MICROGRADOS] Latitud convertida: {rawLat} → {latDestino}");
                    }
                    else
                    {
                        latDestino = rawLat;
                        Console.WriteLine($"[GRADOS] Latitud: {latDestino}");
                    }
                }
                else if (parte.Contains("Lon:"))
                {
                    var loDestino = parte.Replace("Lon:", "").Trim();
                    double rawLon = ParseCoordinate(loDestino);

                    if (Math.Abs(rawLon) > 180)
                    {
                        lonDestino = rawLon / 1000000.0;
                        Console.WriteLine($"[MICROGRADOS] Longitud convertida: {rawLon} → {lonDestino}");
                    }
                    else
                    {
                        lonDestino = rawLon;
                        Console.WriteLine($"[GRADOS] Longitud: {lonDestino}");
                    }
                }
                /*else if (parte.Contains("Lat:"))
                {
                    var ltDestino = parte.Replace("Lat:", "").Trim();
                    latDestino = Double.Parse(ltDestino);
                }
                else if (parte.Contains("Lon:"))
                {
                    var loDestino = parte.Replace("Lon:", "").Trim();
                    lonDestino = Double.Parse(loDestino);
                }*/
            }
            // Mostrar resultados (puedes usarlos en tu aplicaci�n MAUI)
            Console.WriteLine($"Ubicaci�n: {ubicacion2}");
            Console.WriteLine($"Pasajero: {pasajero2}");
            Console.WriteLine($"Distancia: {distancia}");
            Console.WriteLine($"ID Viaje: {idViaje}");


            Console.WriteLine($"Valor Lat convertido: {latDestino}");
            Console.WriteLine($"Valor Lon convertido: {lonDestino}");

            /*Console.WriteLine($"Lat: {latDestino}");
            Console.WriteLine($"Lon: {lonDestino}");*/
            var dk = Double.Parse(distancia.Trim().Replace("km", ""));
            var metrosdistance = ConvertirKilometrosAMetros(dk);

            //lblDistance.Text  = Double.Parse(distancia).ToString();
            lblUbi.Text = ubicacion2;
            lblPasaje.Text = pasajero2;
            lblDistance.Text = metrosdistance.ToString() + " metros";
            ViewModel.LatDes = latDestino;
            ViewModel.LonDes = lonDestino;
            ViewModel.IdViaje = idViaje;
        }

        public static double ConvertirKilometrosAMetros(double kilometros)
        {
            if (kilometros < 0)
            {
                throw new ArgumentException("Los kilómetros no pueden ser negativos", nameof(kilometros));
            }

            return kilometros * 1000;
        }
        

        /*protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.StartMovementTracking();
        }*/

        /* protected override void OnDisappearing()
         {
             base.OnDisappearing();
             ViewModel.StopMovementTracking();
         }*/
        private void UpdateMap()
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
                    Distance.FromKilometers(0.3)),
                animate: true);

            // Actualizar pins
            MyMap.Pins.Clear();
            MyMap.Pins.Add(ViewModel.UserPin);

            if (ViewModel.FixedPin != null && !string.IsNullOrEmpty(ViewModel.FixedPin.Label))
            {
                MyMap.Pins.Add(ViewModel.FixedPin);
            }
        }
        private void UpdateMap2()
        {
            if (ViewModel.CurrentPosition != null && ViewModel.UserPin != null)
            {
                // Actualizar posici�n del pin
                ViewModel.UserPin.Position = ViewModel.CurrentPosition;

                // Mover mapa si es necesario
                MyMap.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                        ViewModel.CurrentPosition,
                        Distance.FromKilometers(0.3)), // Zoom cercano
                    animate: true);
            }
        }

        private async void StartMonitoringTapped(object sender, TappedEventArgs e)
        {
            ViewModel.IsVisibleLoading = true;
            ViewModel.IsBtnGoJornada = false;
            ViewModel.IsBtnStopJornada = true;

            _ = ViewModel.StartMovementTracking();
           // _ = ViewModel.StartRealTimeTracking();
            await ViewModel.IniciarJornada();
            await ViewModel.UpdateToken();
            await Task.Delay(TimeSpan.FromSeconds(5));
            ViewModel.IsVisibleLoading = false;
        }

        private void SotpMonitoring_Cliked(object sender, EventArgs e)
        {
            ViewModel.StopMovementTracking();
            ViewModel.IsBtnGoJornada = true;
            ViewModel.IsBtnStopJornada = false;
            _= ViewModel.TerminarJornada();
            
        }

        /*private void UpdateMap()
{
   if (ViewModel.CurrentPosition != null && ViewModel.UserPin != null)
   {
       // 1. Centrar mapa (mantenemos tu ubicaci�n como centro)
       MyMap.MoveToRegion(
           MapSpan.FromCenterAndRadius(
               ViewModel.CurrentPosition,
               Distance.FromKilometers(0.5)),
           animate: true);

       // 2. Actualizar los pins
       MyMap.Pins.Clear();
       MyMap.Pins.Add(ViewModel.UserPin);

       // 3. Agregar punto fijo si existe
       if (ViewModel.FixedPin != null)
       {
           MyMap.Pins.Add(ViewModel.FixedPin);

           // Opcional: Ajustar vista para mostrar ambos puntos
           var positions = new List<Position> {
           ViewModel.CurrentPosition,
           ViewModel.FixedPin.Position
       };
           MyMap.MoveToRegion(MapSpan.FromPositions(positions), animate: true);
       }
   }
}*/
    }
}

//using Maui.GoogleMaps;
//using MiMototaxxi.ViewModel.HomeMototaxi;
//using System.Windows.Input;

//namespace MiMototaxxi.View.HomeMototaxiPage;

//public partial class HomeMotoPage : ContentPage
//{
//    public HomeMotoViewModel ViewModel { get; }
//    private Polyline _routeLine;

//    public HomeMotoPage()
//    {
//        InitializeComponent();
//        BindingContext = ViewModel = new HomeMotoViewModel();

//        // Inicializaci�n del mapa
//        this.Appearing += async (sender, e) =>
//        {
//            await ViewModel.InitializeMap();
//            UpdateMap();
//        };

//        // Suscripci�n a cambios
//        ViewModel.PropertyChanged += (s, e) =>
//        {
//            switch (e.PropertyName)
//            {
//                case nameof(ViewModel.CurrentPosition):
//                case nameof(ViewModel.UserPin):
//                    UpdateMap();
//                    break;
//                case nameof(ViewModel.Points):
//                    UpdateRoute();
//                    break;
//            }
//        };

//        // Suscribirse a cambios en las propiedades
//        /*ViewModel.PropertyChanged += (s, e) =>
//        {
//            if (e.PropertyName == nameof(ViewModel.CurrentPosition) ||
//                e.PropertyName == nameof(ViewModel.UserPin))
//            {
//                UpdateMap();
//            }
//        };*/
//    }

//    private void UpdateMap()
//    {
//        if (ViewModel.CurrentPosition != null)
//        {
//            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(
//                ViewModel.CurrentPosition,
//                Distance.FromKilometers(1)),
//                animate: true);

//            if (ViewModel.UserPin != null)
//            {
//               // MyMap.Pins.Clear();
//                MyMap.Pins.Add(ViewModel.UserPin);

//               // if (ViewModel.RouteLine != null)
//                  //  MyMap.Polylines.Add(ViewModel.RouteLine);
//            }
//        }
//    }
//    private void UpdateRoute()
//    {
//        // Limpiar todas las polil�neas existentes
//        MyMap.Polylines.Clear();

//        // Solo crear ruta si hay al menos 2 puntos
//        if (ViewModel.Points.Count >= 2)
//        {
//            // Crear una NUEVA instancia de Polyline con los puntos actualizados
//            var newPolyline = new Polyline()
//            {
//                StrokeColor = Colors.Blue,
//                StrokeWidth = 5f
//            };

//            // A�adir puntos mediante Add() ya que la colecci�n es mutable
//            foreach (var point in ViewModel.Points)
//            {
//                newPolyline.Positions.Add(point);
//            }

//            MyMap.Polylines.Add(newPolyline);
//        }
//    }

//    private void UpdatePins()
//    {
//        // Mantener solo el pin del usuario
//        var pinsToKeep = MyMap.Pins.Where(p => p == ViewModel.UserPin).ToList();
//        MyMap.Pins.Clear();

//        foreach (var pin in pinsToKeep)
//        {
//            MyMap.Pins.Add(pin);
//        }

//        // Agregar nuevos pins para los puntos adicionales
//        for (int i = 1; i < ViewModel.Points.Count; i++)
//        {
//            MyMap.Pins.Add(new Pin
//            {
//                Position = ViewModel.Points[i],
//                Label = $"Punto {i + 1}",
//                Type = PinType.Place,
//                Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(0, 0, 255)) // Azul s�lido
//            });
//        }
//    }

//}