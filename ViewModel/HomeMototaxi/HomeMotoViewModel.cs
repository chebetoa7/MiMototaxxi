
using CommunityToolkit.Mvvm.ComponentModel;
//using Firebase.Core;

//using HomeKit;
using Maui.GoogleMaps;
using Microsoft.Maui.Controls;
using MiMototaxxi.Services.Moto;
using Plugin.Firebase.CloudMessaging;



//using Google.Maps.Direction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiMototaxxi.ViewModel.HomeMototaxi
{
    public partial class HomeMotoViewModel : BaseViewModel
    {
        #region Vars
        

        [ObservableProperty]
        private string _codigo;

        [ObservableProperty]
        private bool _isVisibleLoading; 

        [ObservableProperty]
        private bool _isSolicitudViaje; 

        [ObservableProperty]
        private bool _isBtnStopJornada;

        [ObservableProperty]
        private bool _isBtnGoJornada;

        [ObservableProperty]
        private string pasaje;

        [ObservableProperty]
        private string ubication;

        [ObservableProperty]
        private string _idViaje;

        [ObservableProperty]
        private Double _latDes;

        [ObservableProperty]
        private Double _lonDes;

        private readonly MotoService _motoService;

        //Movimiento de mototaxista
        private Position _lastRecordedPosition;
        private const double MovementThresholdMeters = 5;
        private CancellationTokenSource _movementCts;
        #endregion
        #region Properties
        //
        private DestinationMonitor _destinationMonitor;
        private double _currentDistance;

        public double CurrentDistance
        {
            get => _currentDistance;
            set => SetProperty(ref _currentDistance, value);
        }

        //
        private Position _currentPosition;
        public Position CurrentPosition
        {
            get => _currentPosition;
            set => SetProperty(ref _currentPosition, value);
        }

        private Pin _userPin;
        public Pin UserPin
        {
            get => _userPin;
            set => SetProperty(ref _userPin, value);
        }

        //***
        private Pin _fixedPin;
        public Pin FixedPin
        {
            get => _fixedPin;
            set => SetProperty(ref _fixedPin, value);
        }


        #endregion

        #region Commands
        public ICommand InitializeMapCommand { get; }
        public ICommand AddFixedPointCommand { get; }

        public ICommand OpenGoogleMapsCommand { get; }
        public ICommand OpenWazeCommand { get; }
        public ICommand StartMonitoringCommand { get; }
        public ICommand StopMonitoringCommand { get; }

        #endregion

        public HomeMotoViewModel()
        {
            _motoService = new MotoService();
            IsBtnGoJornada = true;
            // Inicializar con valores por defecto
            CurrentPosition = new Position(0, 0);
            //SetDefaultLocation();

            InitializeMapCommand = new Command(async () => await InitializeMap());
           // AddFixedPointCommand = new Command(AddFixedPoint);

            OpenGoogleMapsCommand = new Command(async () => await OpenInGoogleMaps());
            OpenWazeCommand = new Command(async () => await OpenInWaze());

            _destinationMonitor = new DestinationMonitor();
            _destinationMonitor.OnpinUpReached += OnDestinationReachedPinUp;
            _destinationMonitor.OnDistanceChangedPinUp += OnDistanceChangedPinUp;

            StartMonitoringCommand = new Command(async () => await IniciaViaje()); // StartMonitoring());
            StopMonitoringCommand = new Command(StopMonitoringPinUp);

           
        }
        public async Task OpenSolicitudViaje(string datoEntrada)
        {
            string[] partes = datoEntrada.Split('|');

            // Inicializar variables para almacenar los datos
            string ubicacion2 = "";
            string pasajero2 = "";
            string distancia = "";
            string idViaje = "";
            // Recorrer cada parte para extraer la información
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
            }
            // Mostrar resultados (puedes usarlos en tu aplicación MAUI)
            Console.WriteLine($"Ubicación: {ubicacion2}");
            Console.WriteLine($"Pasajero: {pasajero2}");
            Console.WriteLine($"Distancia: {distancia}");
            Console.WriteLine($"ID Viaje: {idViaje}");

            var dk = Double.Parse(distancia.Trim().Replace("km", ""));
            var metrosdistance = ConvertirKilometrosAMetros(dk);


            IsSolicitudViaje = true;
            CurrentDistance = metrosdistance;
            Ubication = ubicacion2;
            Pasaje = pasajero2;
        }
        public static double ConvertirKilometrosAMetros(double kilometros)
        {
            if (kilometros < 0)
            {
                throw new ArgumentException("Los kilómetros no pueden ser negativos", nameof(kilometros));
            }

            return kilometros * 1000;
        }

        private async Task OpenInGoogleMaps()
        {
            if (CurrentPosition == null || FixedPin == null) return;   

            await OpenInGoogleMaps(CurrentPosition, FixedPin.Position, "Punto de Servicio");
        }

        private async Task OpenInWaze()
        {
            if (FixedPin == null) return;

            await OpenInWaze(FixedPin.Position, "Punto de Servicio");
        }

        private void InitializeMotoPin()
        {
            // Validar y crear posición inicial
            Position initialPosition;

            if (CurrentPosition == null ||
                (CurrentPosition.Latitude == 0 && CurrentPosition.Longitude == 0))
            {
                initialPosition = new Position(19.4326, -99.1332); // CDMX como fallback
            }
            else
            {
                initialPosition = CurrentPosition;
            }

            UserPin = new Pin
            {
                
                Type = PinType.Place,
                Position = initialPosition,
                Icon = BitmapDescriptorFactory.FromBundle("mi_icono"),
                IsDraggable = false,
                Anchor = new Point(0.5, 0.5),
                Rotation = 0,
                Label = "Tú (Mototaxista)",
               
            };
        }

        private float GetCurrentBearing()
        {
            // Implementar lógica para obtener el rumbo actual
            // Puedes usar el acelerómetro o diferencias entre posiciones
            return 0; // Valor temporal
        }

       /* public async Task InitializeMap()
        {
            try
            {
                // Inicializar pin primero
                InitializeMotoPin();

                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status == PermissionStatus.Granted)
                {
                    var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.High,
                        Timeout = TimeSpan.FromSeconds(30)
                    });

                    if (location != null)
                    {
                        CurrentPosition = new Position(location.Latitude, location.Longitude);
                        UserPin.Position = CurrentPosition; // Asignar posición inicial
                    }
                    else
                    {
                        SetDefaultLocation();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                SetDefaultLocation();
            }
        }*/

         public async Task InitializeMap()
         {
             try
             {
                SetDefaultLocation();
                 // 1. Verificar permisos
                 var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                 if (status != PermissionStatus.Granted)
                 {
                     status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                 }

                 if (status == PermissionStatus.Granted)
                 {
                     // 2. Obtener ubicación actual
                     var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                     {
                         DesiredAccuracy = GeolocationAccuracy.High, // Mayor precisión para mototaxista
                         Timeout = TimeSpan.FromSeconds(15)
                     });

                     // 3. Configurar el pin del usuario
                     if (location != null)
                     {
                         CurrentPosition = new Position(location.Latitude, location.Longitude);

                         UserPin = new Pin
                         {
                             Label = "Tú (Mototaxista)",
                             Position = CurrentPosition,
                             Type = PinType.Place,
                             Icon = BitmapDescriptorFactory.FromBundle("mi_icono"),
                         };
                         var idUser = Preferences.Get("IdUsuario", string.Empty);
                         var lct = new Model.Moto.LocationMoto
                         {
                             Lat = location.Latitude,
                             Lon = location.Longitude
                         };
                         var result = await _motoService.updateLocationMotoAsync(lct, idUser ?? "NA");
                     }
                     else
                     {
                         SetDefaultLocation();
                     }
                 }
             }
             catch
             {
                 SetDefaultLocation();
             }
         }

        private void SetDefaultLocation()
        {
            CurrentPosition = new Position(19.4326, -99.1332);
            UserPin = new Pin
            {
                Label = "Ubicación predeterminada", // Label obligatorio
                Position = CurrentPosition,
                Type = PinType.Place,
                Icon = BitmapDescriptorFactory.FromBundle("mi_icono"),
            };
        }
        private async Task ActualizaUbiMoto() 
        {
            try 
            {
                var lat = LatDes;
                var lon = LonDes;
                var name = Pasaje;
                AddFixedPinUp(lat, lon, name);
                IsSolicitudViaje = false;
                var idUser = Preferences.Get("IdUsuario", string.Empty);
                var viajemodel = new Model.Moto.Viaje.AceptaViaje
                {
                    lat = lat,
                    lon = lon,
                    CodigoConfirmacion = "NA",
                };
                _= _motoService.AceptaViajeAsync(viajemodel, IdViaje ?? "NA");
            }catch(Exception exM) 
            {
                Console.WriteLine("Error IniciarViaje: " + exM.Message );
            }
        }
        private async Task IniciaViaje() 
        {
            try 
            {
                IsVisibleLoading = true;
                var lat = LatDes;
                var lon = LonDes;
                var name = Pasaje;
                AddFixedPinUp(lat, lon, name);
                
                var idUser = Preferences.Get("IdUsuario", string.Empty);
                var viajemodel = new Model.Moto.Viaje.AceptaViaje
                {
                    lat = lat,
                    lon = lon,
                    CodigoConfirmacion = GenerarCodigo6Digitos(),
                };
                Codigo = viajemodel.CodigoConfirmacion;
                await _motoService.AceptaViajeAsync(viajemodel, IdViaje ?? "NA");
                IsSolicitudViaje = false;
                IsVisibleLoading = false;
            }
            catch(Exception exM) 
            {
                Console.WriteLine("Error IniciarViaje: " + exM.Message );
            }
        }
        public string GenerarCodigo6Digitos()
        {
            Random random = new Random();
            int min = 100000; // Mínimo 6 dígitos
            int max = 999999; // Máximo 6 dígitos
            return random.Next(min, max + 1).ToString();
        }
        private async Task AddFixedPinUp(double lat_, double lon_, string name)
        {
            // Usar Position consistentemente con Maui.GoogleMaps
            var fixedPosition = new Position(lat_, lon_);
            
            FixedPin = new Pin
            {
                Label = $"Pasajero: {name}",
                Position = fixedPosition,
                Type = PinType.Place,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(0, 0, 255))
            };

            // Convertir CurrentPosition a Position (Maui.GoogleMaps)
            var puntoInicial = new Position(CurrentPosition.Latitude, CurrentPosition.Longitude);
            var puntofinal = new Position(lat_, lon_);

            // Crear la Polyline
            var polyline = new Polyline
            {
                StrokeColor = Color.FromHex("#FF0000"),
                StrokeWidth = 10f,
                IsClickable = true
            };

            // Añadir puntos (usando Position)
            polyline.Positions.Add(puntoInicial);
            polyline.Positions.Add(puntofinal);
            await StartMonitoringPinUp();

        }
        /*private void AddFixedPoint(Double lat_, Double lon_, string name)
        {
            var fixedPosition = new Position(lat_, lon_);
            FixedPin = new Pin
            {
                Label = $"Pasajero: {name}", // Label obligatorio
                Position = fixedPosition,
                Type = PinType.Place,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(0, 0, 255))
            };
            var latini = CurrentPosition.Latitude;
            var lonini = CurrentPosition.Longitude;
            // Coordenadas de ejemplo (punto A y punto B)
            Location? puntoInicial = new Location(latini, lonini); // Lima, Perú
            Location? puntofinal = new Location(lat_, lon_); // Punto cercano
             // 2. Crear la Polyline
            var polyline = new Polyline
            {
                StrokeColor = Color.FromHex("#FF0000"),
                StrokeWidth = 10f,
                IsClickable = true
            };
            // 3. Añadir puntos (usando Positions, no Points ni Geopath)
            polyline.Positions.Add(puntoInicial);
            polyline.Positions.Add(puntofinal);
        }*/
        public async Task OpenInWaze(Position destination, string destinationName = "Destino")
        {
            try
            {
                var uri = new Uri($"waze://?ll={destination.Latitude},{destination.Longitude}&navigate=yes");

                // Primero intentamos con Waze directamente
                var canOpen = await Launcher.CanOpenAsync(uri);

                if (!canOpen)
                {
                    // Si Waze no está instalado, abrimos la tienda
                    uri = new Uri($"https://waze.com/ul?ll={destination.Latitude},{destination.Longitude}&navigate=yes");
                }

                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir Waze: {ex.Message}", "OK");
            }
        }

        public async Task OpenInGoogleMaps(Position start, Position end, string destinationName = "Destino")
        {
            try
            {
                var uri = new Uri($"https://www.google.com/maps/dir/?api=1&origin={start.Latitude},{start.Longitude}&destination={end.Latitude},{end.Longitude}&travelmode=driving&dir_action=navigate");

                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir Google Maps: {ex.Message}", "OK");
            }
        }

        //Monitor
        private async Task StartMonitoringPinUp()
        {
            if (FixedPin == null) return;

            await _destinationMonitor.StartMonitoringUp(FixedPin.Position);
        }

        private void StopMonitoringPinUp()
        {
            _destinationMonitor.StopMonitoringPinUp();
            //Se rechasa el viaje codificar
        }

        private void OnDistanceChangedPinUp(double distance)
        {
            CurrentDistance = distance;

            // Puedes actualizar la UI con la distancia restante
            Console.WriteLine($"Distancia al destino: {distance} metros");
        }

        private async void OnDestinationReachedPinUp()
        {
            StopMonitoringPinUp();
            var viajemodel = new Model.Moto.Viaje.StatusModel
            {
                estatus = "llego"
            };
            await _motoService.UpdateViajeStatusAsync(viajemodel, IdViaje ?? "NA");
            /* await Application.Current.MainPage.DisplayAlert(
                 "¡Llegaste!",
                 "Has llegado a tu destino",
                 "OK");*/
            
            Console.WriteLine("El usuario esta por llegar...! ");


            // Aquí puedes registrar la llegada en tu base de datos
        }
        public async Task UpdateToken()
        {
            try {
                var tk = Preferences.Get("TokenFCM", string.Empty);
                var idUser = Preferences.Get("IdUsuario", string.Empty);
                var lctToken = new Model.Moto.MtoToken { fcmToken = tk };
                await _motoService.updateMotoAsync(lctToken, idUser ?? "NA");
            }
            catch(Exception exM) 
            {
                Console.WriteLine("Error, UpdateToken: " + exM.Message);
            }
        }
        #region MetodosMovimientoMototaxi
        /*public async Task StartMovementTracking()
        {
            _movementCts = new CancellationTokenSource();
            _lastRecordedPosition = CurrentPosition;
            

            try
            {
                while (!_movementCts.IsCancellationRequested)
                {
                    var location = await GetCurrentLocationWithRetry();
                    if (location != null)
                    {
                        
                        var newPosition = new Position(location.Latitude, location.Longitude);
                        var distance = CalculateDistance(_lastRecordedPosition, newPosition);

                        if (distance >= MovementThresholdMeters)
                        {
                            var idUser = Preferences.Get("IdUsuario", string.Empty);
                            //var tk = Preferences.Get("TokenFCM", string.Empty);
                            var lct = new Model.Moto.LocationMoto
                            {
                                Lat = location.Latitude,
                                Lon = location.Longitude
                            };
                            
                            await _motoService.updateLocationMotoAsync(lct, idUser ?? "NA");
                            _lastRecordedPosition = newPosition;
                            CurrentPosition = newPosition;
                           
                            Console.WriteLine($"Moto movida a nueva posición: {newPosition}");

                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10), _movementCts.Token);
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Movement tracking stopped");
            }
        }*/
        
        //Se iniciara Jornada y se envia el token actualizado
        public async Task IniciarJornada()
        {
            try
            {
                var idUser = Preferences.Get("IdUsuario", string.Empty);
                var estatusDto = new Model.Moto.DtoEstatus
                {
                    Estatus = "Disponible"
                };
                _ = _motoService.updateMotoAsync(estatusDto, idUser ?? "NA");
            }
            catch (Exception exM)
            {
                Console.WriteLine("Error Iniciar Jornada: " + exM.Message);
            }
        }


        public async Task TerminarJornada()
        {
            try
            {
                var idUser = Preferences.Get("IdUsuario", string.Empty);
                var estatusDto = new Model.Moto.DtoEstatus
                {
                    Estatus = "SinServicio"
                };
                _ = _motoService.updateMotoAsync(estatusDto, idUser ?? "NA");
            }
            catch (Exception exM)
            {
                Console.WriteLine("Error Iniciar Jornada: " + exM.Message);
            }
        }


        /* public async Task StartMovementTracking()
         {
             _movementCts = new CancellationTokenSource();
             _lastRecordedPosition = CurrentPosition;

             try
             {
                 while (!_movementCts.IsCancellationRequested)
                 {
                     var location = await GetCurrentLocationWithRetry();

                     if (location != null && location.Accuracy <= 20) // Solo si la precisión es buena
                     {
                         var newPosition = new Position(location.Latitude, location.Longitude);
                         var distance = CalculateDistance(_lastRecordedPosition, newPosition);
                         double acuary_ = (double)location.Accuracy;
                         var requiredDistance = GetDynamicMovementThreshold(acuary_);

                         if (distance >= requiredDistance)
                         {
                             var idUser = Preferences.Get("IdUsuario", string.Empty);
                             var lct = new Model.Moto.LocationMoto
                             {
                                 Lat = location.Latitude,
                                 Lon = location.Longitude,
                             };

                             await _motoService.updateLocationMotoAsync(lct, idUser ?? "NA");
                             _lastRecordedPosition = newPosition;
                             CurrentPosition = newPosition;
                             Console.WriteLine($"Movimiento detectado: {distance}m (Umbral requerido: {requiredDistance}m)");
                         }
                     }

                     await Task.Delay(TimeSpan.FromSeconds(10), _movementCts.Token);
                 }
             }
             catch (TaskCanceledException)
             {
                 Console.WriteLine("Movement tracking stopped");
             }
         }*/

        /*private double GetDynamicMovementThreshold(double? accuracy)
        {
            throw new NotImplementedException();
        }*/

        /*Nueva forma de actualiza */

        public async Task StartMovementTracking()
        {
            _movementCts = new CancellationTokenSource();

            try
            {
                while (!_movementCts.IsCancellationRequested)
                {
                    var location = await GetCurrentLocationWithRetry();
                    if (location != null)
                    {
                        var newPosition = new Position(location.Latitude, location.Longitude);
                        CurrentPosition = newPosition; // Actualización inmediata del mapa
                        

                        Debug.WriteLine($"🗺️ Mapa actualizado | Lat: {location.Latitude:N6}, Lon: {location.Longitude:N6}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), _movementCts.Token); // Actualización cada 5 segundo
                }
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("Map tracking detenido");
            }
        }

        private async Task<Location> GetCurrentLocationWithRetry()
        {
            try
            {
                using var cts = new CancellationTokenSource(3500); // Timeout de 2 segundos

                var request = new GeolocationRequest(
                    GeolocationAccuracy.Best,
                    TimeSpan.FromSeconds(4));

                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null && location.Accuracy <= 15) // Filtro de precisión
                {
                    return location;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Error de ubicación: {ex.Message}");
            }

            return null;
        }
        public void StopMovementTracking()
        {
            _movementCts?.Cancel();
        }

        private double CalculateDistance(Position pos1, Position pos2)
        {
            // Implementación mejorada de Haversine
            double R = 6371000; // Radio de la Tierra en metros
            var dLat = (pos2.Latitude - pos1.Latitude) * Math.PI / 180;
            var dLon = (pos2.Longitude - pos1.Longitude) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(pos1.Latitude * Math.PI / 180) *
                    Math.Cos(pos2.Latitude * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double GetDynamicMovementThreshold(double accuracy)
        {
            // El umbral mínimo es 5m (MovementThresholdMeters), 
            // pero se ajusta según la precisión del GPS
            // Multiplicamos la precisión por 0.7 para tener un margen
            return Math.Max(MovementThresholdMeters, accuracy * 0.7);
        }

       
        /* 3 private async Task<Location> GetCurrentLocationWithRetry(int maxAttempts = 3)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var request = new GeolocationRequest(
                        GeolocationAccuracy.High, // Cambiado de Best a High para mejor rendimiento
                        TimeSpan.FromSeconds(10));

                    var location = await Geolocation.GetLocationAsync(request);

                    // Validación mejorada de la ubicación
                    if (location != null && IsValidLocation(location))
                    {
                        return location;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Intento {attempt}: {ex.Message}");
                    if (attempt == maxAttempts) throw;
                    await Task.Delay(1000 * attempt); // Retry con backoff
                }
            }
            return null;
        }*/

        private bool IsValidLocation(Location location)
        {
            // Verificar precisión (máximo 50 metros)
            if (location.Accuracy > 50) return false;

            // Verificar que la lectura no sea demasiado antigua
            var timeDiff = DateTime.UtcNow - location.Timestamp.ToUniversalTime();
            if (timeDiff.TotalSeconds > 15) return false;

            // Verificar que no sea un salto imposible
            if (_lastRecordedPosition != null)
            {
                var distance = CalculateDistance(
                    new Position(_lastRecordedPosition.Latitude, _lastRecordedPosition.Longitude),
                    new Position(location.Latitude, location.Longitude));

                // Rechazar movimientos mayores a 500 metros en 15 segundos (120 km/h)
                if (distance > 500 && timeDiff.TotalSeconds < 15) return false;
            }

            return true;
        }

        /* 3 public async Task StartMovementTracking()
        {
            _movementCts = new CancellationTokenSource();
            _lastRecordedPosition = CurrentPosition;
            _lastUpdateTime = DateTime.UtcNow;

            try
            {
                while (!_movementCts.IsCancellationRequested)
                {
                    var locationTask = GetCurrentLocationWithRetry();
                    var location = await locationTask;

                    if (location != null)
                    {
                        var newPosition = new Position(location.Latitude, location.Longitude);
                        var distance = CalculateDistance(_lastRecordedPosition, newPosition);

                        if (distance >= MovementThresholdMeters)
                        {
                            // Actualización inmediata del UI
                            CurrentPosition = newPosition;
                            _lastRecordedPosition = newPosition;

                            // Envío NO BLOQUEANTE al API
                            _ = UpdateLocationInBackground(location);
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(2), _movementCts.Token); // Intervalo reducido
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Monitoreo de movimiento detenido");
            }
        }*/
        /*3 private async Task<Location> GetCurrentLocationWithRetry(int maxAttempts = 3)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    using var cts = new CancellationTokenSource(3000); // Timeout de 3 segundos
                    var request = new GeolocationRequest(
                        GeolocationAccuracy.High,
                        TimeSpan.FromSeconds(5));

                    var location = await Geolocation.GetLocationAsync(request, cts.Token);

                    if (location != null && IsValidLocation(location))
                    {
                        return location;
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("Timeout obteniendo ubicación");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Intento {attempt}: {ex.Message}");
                    if (attempt == maxAttempts) throw;
                    await Task.Delay(500 * attempt); // Espera más corta
                }
            }
            return null;
        }*/

        // Método para actualización en segundo plano
       /*3 private async Task UpdateLocationInBackground(Location location)
        {
            try
            {
                var idUser = Preferences.Get("IdUsuario", string.Empty);
                var lct = new Model.Moto.LocationMoto
                {
                    Lat = location.Latitude,
                    Lon = location.Longitude
                };

                // Usamos ConfigureAwait(false) para no volver al contexto de UI
                await _motoService.updateLocationMotoNoAsync(lct, idUser ?? "NA")
                                .ConfigureAwait(false);

                Console.WriteLine($"✅ Ubicación actualizada en API: {location.Latitude}, {location.Longitude}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al actualizar API: {ex.Message}");
                // Opcional: Reintentar o guardar en cola para enviar luego
            }
        }*/

        /* 2 public async Task StartMovementTracking()
         {
             _movementCts = new CancellationTokenSource();
             _lastRecordedPosition = CurrentPosition;
             _lastUpdateTime = DateTime.UtcNow; // Inicializar aquí también

             try
             {
                 while (!_movementCts.IsCancellationRequested)
                 {
                     var location = await GetCurrentLocationWithRetry();
                     if (location != null)
                     {
                         var newPosition = new Position(location.Latitude, location.Longitude);
                         var distance = CalculateDistance(_lastRecordedPosition, newPosition);

                         if (distance >= MovementThresholdMeters)//si ya avanzo minimo 5 metros
                         {
                             var idUser = Preferences.Get("IdUsuario", string.Empty);
                             //var tk = Preferences.Get("TokenFCM", string.Empty);
                             var lct = new Model.Moto.LocationMoto
                             {
                                 Lat = location.Latitude,
                                 Lon = location.Longitude
                             };

                             await _motoService.updateLocationMotoAsync(lct, idUser ?? "NA");
                             _lastRecordedPosition = newPosition;
                             CurrentPosition = newPosition;

                             Console.WriteLine($"Moto movida a nueva posición: {newPosition}");

                         }

                         await Task.Delay(TimeSpan.FromSeconds(10), _movementCts.Token);
                     }

                 }
             }
             catch (TaskCanceledException)
             {
                 Console.WriteLine("Monitoreo de movimiento detenido");
             }
         }*/

        private bool ShouldUpdatePosition(Position newPosition)
        {
            if (_lastRecordedPosition == null) return true;

            var distance = CalculateDistance(_lastRecordedPosition, newPosition);
            return distance >= MovementThresholdMeters;
        }
        private DateTime _lastUpdateTime = DateTime.UtcNow;
        private DateTime _lastLocationUpdate = DateTime.UtcNow;
        private DateTime _lastFirebaseUpdate = DateTime.UtcNow;
        private double _currentSpeed = 0;   
        private TimeSpan CalculateDynamicDelay(long elapsedMilliseconds)
        {
            const int baseIntervalMs = 3000; // 3 segundos base
            const int minIntervalMs = 1000;  // Mínimo 1 segundo
            const int maxIntervalMs = 10000; // Máximo 10 segundos

            // Si no hay posición previa, usar intervalo base
            if (_lastRecordedPosition == null || CurrentPosition == null)
                return TimeSpan.FromMilliseconds(baseIntervalMs);

            // Calcular velocidad actual (m/s)
            var timeDiffSec = (DateTime.UtcNow - _lastUpdateTime).TotalSeconds;
            var distance = CalculateDistance(_lastRecordedPosition, CurrentPosition);
            var speed = timeDiffSec > 0 ? distance / timeDiffSec : 0;

            // Ajustar intervalo basado en velocidad
            return speed switch
            {
                > 20 => TimeSpan.FromMilliseconds(minIntervalMs),    // >72 km/h: actualizar cada 1s
                > 10 => TimeSpan.FromMilliseconds(baseIntervalMs / 2), // >36 km/h: cada 1.5s
                > 5 => TimeSpan.FromMilliseconds(baseIntervalMs),   // >18 km/h: cada 3s
                _ => TimeSpan.FromMilliseconds(maxIntervalMs)     // Default: cada 10s
            };
        }

        public async Task StartRealTimeTracking()
        {
            _movementCts = new CancellationTokenSource();

            try
            {
                while (!_movementCts.IsCancellationRequested)
                {
                    var startTime = DateTime.UtcNow;

                    // Obtener ubicación con espera máxima de 1 segundo
                    var location = await GetLocationWithTimeout(1000);

                    if (location != null && IsValidLocation(location))
                    {
                        var newPosition = new Position(location.Latitude, location.Longitude);

                        // Calcular velocidad actual
                        CalculateCurrentSpeed(newPosition, startTime);

                        // Actualizar UI inmediatamente
                        CurrentPosition = newPosition;

                        // Condiciones para actualizar Firebase:
                        // 1. Siempre que la precisión sea buena (<= 20m)
                        // 2. O cada 2 segundos como máximo
                        if (location.Accuracy <= 20 || (DateTime.UtcNow - _lastFirebaseUpdate).TotalSeconds >= 2)
                        {
                            await UpdateFirebaseLocation(location);
                            _lastFirebaseUpdate = DateTime.UtcNow;
                        }

                        _lastLocationUpdate = DateTime.UtcNow;
                    }

                    // Intervalo dinámico basado en velocidad (más agresivo)
                    var delay = GetDynamicDelay();
                    await Task.Delay(delay, _movementCts.Token);
                }
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("Tracking detenido");
            }
        }

        private async Task<Location> GetLocationWithTimeout(int timeoutMs)
        {
            try
            {
                var cts = new CancellationTokenSource(timeoutMs);
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(5));

                return await Geolocation.GetLocationAsync(request, cts.Token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo ubicación: {ex.Message}");
                return null;
            }
        }

        private void CalculateCurrentSpeed(Position newPosition, DateTime updateTime)
        {
            if (_lastRecordedPosition != null)
            {
                var timeDiff = (updateTime - _lastLocationUpdate).TotalSeconds;
                if (timeDiff > 0)
                {
                    var distance = CalculateDistance(_lastRecordedPosition, newPosition);
                    _currentSpeed = distance / timeDiff; // m/s
                    Debug.WriteLine($"Velocidad actual: {_currentSpeed * 3.6:N1} km/h");
                }
            }
            _lastRecordedPosition = newPosition;
        }

        private int GetDynamicDelay()
        {
            // Convertir velocidad m/s → km/h (×3.6)
            var speedKmh = _currentSpeed * 3.6;
            return speedKmh switch
            {
                > 40 => 500,   // >40 km/h: actualizar cada 500ms
                > 20 => 1000,  // >20 km/h: cada 1s
                _ => 1500   // Default: cada 1.5s
            };
        }

        private async Task UpdateFirebaseLocation(Location location)
        {
            try
            {
                var idUser = Preferences.Get("IdUsuario", string.Empty);
                var locationData = new Model.Moto.LocationMoto
                {
                    Lat = location.Latitude,
                    Lon = location.Longitude,
                    
                };

                await _motoService.updateLocationMotoAsync(locationData, idUser ?? "NA");
                Debug.WriteLine($"📡 Firebase actualizado | Prec: {location.Accuracy}m");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🔥 Error Firebase: {ex.Message}");
            }
        }
        /*private async Task<Location> GetCurrentLocationWithRetry(int maxAttempts = 3)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var request = new GeolocationRequest(
                        GeolocationAccuracy.Best, // Mayor precisión posible
                        TimeSpan.FromSeconds(15));

                    var location = await Geolocation.GetLocationAsync(request);

                    // Validar que la ubicación sea reciente y precisa
                    if (location != null && location.Accuracy <= 30) // 30 metros o mejor
                    {
                        var timeDiff = DateTime.UtcNow - location.Timestamp.ToUniversalTime();
                        if (timeDiff.TotalSeconds <= 30) // Máximo 30 segundos de antigüedad
                        {
                            return location;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Intento {attempt}: {ex.Message}");
                    if (attempt == maxAttempts) throw;
                    await Task.Delay(1000 * attempt); // Retry con backoff
                }
            }
            return null;
        }*/



        /*private async Task<Location> GetCurrentLocationWithRetry(int maxAttempts = 3)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.High);
                    return await Geolocation.GetLocationAsync(request);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Intento {attempt}: {ex.Message}");
                    if (attempt == maxAttempts) throw;
                    await Task.Delay(1000);
                }
            }
            return null;
        }*/
        #endregion


    }


    //public class HomeMotoViewModel : BaseViewModel
    //{
    //    #region Properties
    //    private Position _currentPosition;
    //    public Position CurrentPosition
    //    {
    //        get => _currentPosition;
    //        set => SetProperty(ref _currentPosition, value);
    //    }

    //    private Pin _userPin;
    //    public Pin UserPin
    //    {
    //        get => _userPin;
    //        set => SetProperty(ref _userPin, value);
    //    }

    //    // Nuevas propiedades para los puntos y polilíneas
    //    private ObservableCollection<Position> _points = new();
    //    public ObservableCollection<Position> Points
    //    {
    //        get => _points;
    //        set => SetProperty(ref _points, value);
    //    }

    //    private Polyline _routeLine;
    //    public Polyline RouteLine
    //    {
    //        get => _routeLine;
    //        set => SetProperty(ref _routeLine, value);
    //    }


    //    private bool _isAddingPoint;
    //    public bool IsAddingPoint
    //    {
    //        get => _isAddingPoint;
    //        set
    //        {
    //            SetProperty(ref _isAddingPoint, value);
    //            OnPropertyChanged(nameof(IsNotAddingPoint));
    //        }
    //    }

    //    public bool IsNotAddingPoint => !IsAddingPoint;


    //    #endregion

    //    #region Commands
    //    public ICommand RelocalizarCommand { get; }
    //    public ICommand InitializeMapCommand { get; }
    //    public ICommand AddPointCommand { get; }

    //    #endregion

    //    public HomeMotoViewModel()
    //    {
    //       // RelocalizarCommand = new Command(async () => await OnCenterMapClicked());
    //        RelocalizarCommand = new Command(async () => await CenterToCurrentLocationWithRetry());

    //        InitializeMapCommand = new Command(async () => await InitializeMap());

    //        AddPointCommand = new Command(async () => await AddPointWithFeedback());
    //    }

    //    #region Methods
    //    //*************
    //    private void AddNewPoint()
    //    {
    //        if (CurrentPosition == null) return;

    //        // Agregar nuevo punto
    //        Points.Add(CurrentPosition);

    //        // Solo actualizar si hay suficientes puntos
    //        if (Points.Count >= 2)
    //        {
    //            UpdateRouteLine();
    //        }
    //    }

    //    private void UpdateRouteLine()
    //    {
    //        if (Points.Count < 2)
    //        {
    //            RouteLine = null;
    //            return;
    //        }

    //        var positions = new Position[Points.Count];
    //        Points.CopyTo(positions, 0);

    //        RouteLine = new Polyline
    //        {
    //            StrokeColor = Colors.Blue,
    //            StrokeWidth = 5f
    //        };

    //        foreach (var position in positions)
    //        {
    //            RouteLine.Positions.Add(position);
    //        }
    //    }

    //    private async Task AddPointWithFeedback()
    //    {
    //        if (CurrentPosition == null || IsAddingPoint) return;

    //        IsAddingPoint = true;

    //        try
    //        {
    //            var manualPosition = new Position(16.552417, -94.944092);
    //            // Agregar el punto
    //            //Points.Add(manualPosition);
    //            UserPin = new Pin
    //            {
    //                Label = "Estás user",
    //                Position = manualPosition,
    //                Type = PinType.Place,
    //                Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(0, 0, 255)) // Azul sólido
    //            };
    //            Points.Add(manualPosition);
    //            // Actualizar la ruta
    //            UpdateRouteLine();

    //            // Feedback visual (opcional)
    //            await Task.Delay(300); // Pequeña pausa para feedback
    //        }
    //        finally
    //        {
    //            IsAddingPoint = false;
    //        }
    //    }
    //    // Cuando se actualiza la posición actual
    //    private void OnCurrentPositionChanged()
    //    {
    //        if (Points.Count == 0)
    //        {
    //            // Primer punto
    //            Points.Add(CurrentPosition);
    //        }
    //    }
    //    //***********

    //    public async Task InitializeMap()
    //    {
    //        try
    //        {
    //            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
    //            if (status != PermissionStatus.Granted)
    //            {
    //                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
    //            }

    //            if (status == PermissionStatus.Granted)
    //            {
    //                var location = await Geolocation.GetLocationAsync(new GeolocationRequest
    //                {
    //                    DesiredAccuracy = GeolocationAccuracy.Medium,
    //                    Timeout = TimeSpan.FromSeconds(30)
    //                });

    //                if (location != null)
    //                {
    //                    CurrentPosition = new Position(location.Latitude, location.Longitude);
    //                    UserPin = new Pin
    //                    {
    //                        Label = "Estás aquí",
    //                        Position = CurrentPosition,
    //                        Type = PinType.Place,
    //                        Icon = BitmapDescriptorFactory.FromBundle("mi_icono"),
    //                        //Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(0, 0, 255)) // Azul sólido
    //                    };
    //                    Points.Add(CurrentPosition);
    //                }
    //                else
    //                {
    //                    SetDefaultLocation();
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Error: {ex.Message}");
    //            SetDefaultLocation();
    //        }
    //    }

    //    private void SetDefaultLocation()
    //    {
    //        CurrentPosition = new Position(19.4326, -99.1332);
    //        UserPin = new Pin
    //        {
    //            Label = "Ubicación predeterminada",
    //            Position = CurrentPosition,
    //            Type = PinType.Place,
    //            Icon = BitmapDescriptorFactory.FromBundle("moto_pin.png")
    //        };
    //    }

    //   /* private async Task OnCenterMapClicked()
    //    {
    //        var location = await Geolocation.GetLocationAsync();
    //        if (location != null)
    //        {
    //            CurrentPosition = new Position(location.Latitude, location.Longitude);
    //            UserPin.Position = CurrentPosition;
    //        }
    //    }*/

    //    // Agrega estas propiedades
    //    private bool _isLoadingLocation;
    //    public bool IsLoadingLocation
    //    {
    //        get => _isLoadingLocation;
    //        set => SetProperty(ref _isLoadingLocation, value);
    //    }

    //    private string _locationStatusMessage;
    //    public string LocationStatusMessage
    //    {
    //        get => _locationStatusMessage;
    //        set => SetProperty(ref _locationStatusMessage, value);
    //    }

    //    private async Task CenterToCurrentLocationWithRetry(int maxRetries = 2)
    //    {
    //        if (IsLoadingLocation) return;

    //        IsLoadingLocation = true;
    //        LocationStatusMessage = "Obteniendo ubicación...";

    //        try
    //        {
    //            for (int attempt = 1; attempt <= maxRetries; attempt++)
    //            {
    //                var location = await GetCurrentLocationWithTimeout(TimeSpan.FromSeconds(10));

    //                if (location != null)
    //                {
    //                    CurrentPosition = new Position(location.Latitude, location.Longitude);
    //                    UserPin.Position = CurrentPosition;
    //                    LocationStatusMessage = "Ubicación actualizada";
    //                    IsLoadingLocation = false;
    //                    return;
    //                }

    //                if (attempt < maxRetries)
    //                {
    //                    LocationStatusMessage = $"Reintentando... ({attempt}/{maxRetries})";
    //                    await Task.Delay(1000);
    //                }
    //            }

    //            LocationStatusMessage = "No se pudo obtener la ubicación";
    //        }
    //        catch (Exception ex)
    //        {
    //            LocationStatusMessage = $"Error: {ex.Message}";
    //        }
    //        finally
    //        {
    //            IsLoadingLocation = false;
    //        }
    //    }

    //    private async Task<Location> GetCurrentLocationWithTimeout(TimeSpan timeout)
    //    {
    //        try
    //        {
    //            var cts = new CancellationTokenSource(timeout);
    //            var request = new GeolocationRequest(GeolocationAccuracy.Medium, timeout);

    //            return await Geolocation.GetLocationAsync(request, cts.Token);
    //        }
    //        catch (OperationCanceledException)
    //        {
    //            LocationStatusMessage = "Tiempo de espera agotado";
    //            return null;
    //        }
    //    }
    //    #endregion
    //}
}
