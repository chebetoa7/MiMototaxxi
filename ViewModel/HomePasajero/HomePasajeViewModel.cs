using CommunityToolkit.Mvvm.ComponentModel;
using Maui.GoogleMaps;
using MiMototaxxi.Model.Usuario;
using MiMototaxxi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Timers;

namespace MiMototaxxi.ViewModel.HomePasajero
{
    public partial class HomePasajeViewModel : BaseViewModel
    {
        #region Vars

        [ObservableProperty]
        private bool _isVisibleLoadingViaje;

        [ObservableProperty]
        private bool _isVisibleViaje;

        [ObservableProperty]
        private string _idMototaxi;

        private readonly UsuarioService _Service;

        private Position _currentPosition;
        public Position CurrentPosition
        {
            get => _currentPosition;
            set => SetProperty(ref _currentPosition, value);
        }
        private Position _currentPositionMoto;
        public Position CurrentPositionMoto
        {
            get => _currentPositionMoto;
            set => SetProperty(ref _currentPositionMoto, value);
        }
        private Pin _userPinMoto;
        public Pin UserPinMoto
        {
            get => _userPinMoto;
            set => SetProperty(ref _userPinMoto, value);
        }
        private Pin _userPin;
        public Pin UserPin
        {
            get => _userPin;
            set => SetProperty(ref _userPin, value);
        }
        private Pin _fixedPin;
        public Pin FixedPin
        {
            get => _fixedPin;
            set => SetProperty(ref _fixedPin, value);
        }
        #endregion
        #region Commands
        public ICommand InitializeMapCommand { get; }
        public ICommand EnviarSolicitudCommand { get; }
        public ICommand AddFixedPointCommand { get; }
        #endregion
        #region Constructor
        public HomePasajeViewModel() 
        {
            _Service = new UsuarioService();
            IsVisibleViaje = true;
            CurrentPosition = new Position(0, 0);
            InitializeMapCommand = new Command(async () => await InitializeMap());
            EnviarSolicitudCommand = new Command(async () => await EnviarSolicitud());
            AddFixedPointCommand = new Command(AddFixedPoint);
        }
        #endregion

        #region Function
        private async Task EnviarSolicitud() 
        {
            try 
            {
                IsVisibleLoadingViaje = true;
                IsVisibleViaje = false;
                var solviaje = new SolicitudViaje
                {
                    nomPasajero = "Pasajero Invitado",
                    idPasajero = "00",
                    lat = CurrentPosition.Latitude,
                    lon = CurrentPosition.Longitude,
                    ubicacion = "Invitado", //ObtenerDireccionDesdeCoordenadas(CurrentPosition.Latitude, CurrentPosition.Longitude).Result ?? "No disponible",
                    fcmToken = "NA"

                };
                var result = await _Service.EnviarSolicitudViaje(solviaje,"70102");

                // Asume que 'result' incluye un ID de viaje (ajusta según tu API real)
                if (!string.IsNullOrEmpty(result?.viajeId))
                {
                    await StartPollingForConfirmation(result.viajeId);
                }

            } catch (Exception exM)
            {
                Console.WriteLine("Error: " + exM.Message);
            }
           // IsVisibleLoadingViaje = false;
        }
        private void AddFixedPoint()
        {
            var fixedPosition = new Position(16.547402, -94.950021);

            FixedPin = new Pin
            {
                Label = "Punto de Servicio", // Label obligatorio
                Position = fixedPosition,
                Type = PinType.Place,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(0,0,200))
            };
        }
        public async Task<string> ObtenerDireccionDesdeCoordenadas(double latitud, double longitud)
        {
            try
            {
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    Console.WriteLine("Permiso de ubicación denegado");
                    return "Permiso no disponible";
                }

                Console.WriteLine("Antes de GetPlacemarksAsync");
                var placemarks = await Geocoding.Default.GetPlacemarksAsync(latitud, longitud);
                Console.WriteLine($"Después de GetPlacemarksAsync. Placemarks count: {placemarks?.ToList().Count ?? 0}"); // Debug
                                                                                                                 // var placemarks = await Geocoding.Default.GetPlacemarksAsync(latitud, longitud);

                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    var direccion = $"{placemark.Thoroughfare} {placemark.SubThoroughfare}, " +
                                   $"{placemark.Locality}, {placemark.AdminArea}, " +
                                   $"{placemark.CountryName}, {placemark.PostalCode}";

                    // Limpiar elementos nulos
                    direccion = direccion.Replace(" ,", "").Replace(", ,", ",").Trim(',', ' ');

                    return direccion;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine($"Geocodificación no soportada: {fnsEx.Message}");
            }
            catch (PermissionException ex)
            {
                Console.WriteLine("Permisos insuficientes: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en geocodificación: {ex.Message}");
            }

            return "Dirección no disponible";
        }
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
                            Label = "Tú (Usuario Pasajero)",
                            Position = CurrentPosition,
                            Type = PinType.Place,
                            Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(0, 0, 255)) // Azul sólido
                            //Icon = BitmapDescriptorFactory.FromBundle("mi_icono"),
                        };
                        /*var idUser = Preferences.Get("IdUsuario", string.Empty);
                        var lct = new Model.Moto.LocationMoto
                        {
                            Lat = location.Latitude,
                            Lon = location.Longitude
                        };*/
                        //var result = await _motoService.updateLocationMotoAsync(lct, idUser ?? "NA");
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
        #endregion

        #region ObtencionLatLonMotoViaje
        private System.Timers.Timer _polllingTimerCheckPositionMoto;//Timer para el intervalo de odtención de lat y lon de moto
        [ObservableProperty]
        private bool _isWaitingForLatLon;

        private async Task StartPollingForDetectionLatLon()
        {
            _isWaitingForLatLon = true;

            // Configurar el timer para ejecutarse cada 10 segundos
            _polllingTimerCheckPositionMoto = new System.Timers.Timer(10000); // 10 segundos en milisegundos
            _polllingTimerCheckPositionMoto.Elapsed += async (sender, e) => await CheckLatitudLongitud();
            _polllingTimerCheckPositionMoto.AutoReset = true;
            _polllingTimerCheckPositionMoto.Enabled = true;

            // Primera verificación inmediata
            await CheckLatitudLongitud();
        }
        private async Task CheckLatitudLongitud() 
        {
            try 
            {
                await SetUpdateLocationMoto();

            }catch(Exception exM) 
            {
                Console.WriteLine($"Error CheckLatitudLongitud: {exM.Message}");
            }
        }
        private async Task SetUpdateLocationMoto()
        {
            try
            {
                var result = await _Service.GetLocationMoto(IdMototaxi);
                var lat = result.lat;
                var lon = result.lon;
                var CurrentPositionMoto_ = new Position(lat, lon);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    UserPinMoto = new Pin
                    {
                        Label = "Mototaxista",
                        Position = CurrentPositionMoto_,
                        Type = PinType.Place,
                        Icon = BitmapDescriptorFactory.FromBundle("mi_icono"),
                    };

                    // Notificar a la página para actualizar el mapa
                    MessagingCenter.Send(this, "UpdateMotoPin", UserPinMoto);
                });
            }
            catch (Exception exM)
            {
                Console.WriteLine("Error, SetUpdateLocation: " + exM.Message);
            }
        }

        /*private async Task SetUpdateLocationMoto()
        {
            try
            {
                var result = await _Service.GetLocationMoto(IdMototaxi);
                var lat = result.lat;
                var lon = result.lon;
                var CurrentPositionMoto_ = new Position(lat, lon);
                UserPinMoto = new Pin
                {
                    Label = "Mototaxista", // Label obligatorio
                    Position = CurrentPositionMoto_,
                    Type = PinType.Place,
                    Icon = BitmapDescriptorFactory.FromBundle("mi_icono"),
                };
            }
            catch (Exception exM)
            {
                Console.WriteLine("Error, SetUpdateLocation: " + exM.Message);
            }
        }*/
        #endregion

        #region Polling_para_confirmacion_de_viaje


        private System.Timers.Timer _pollingTimer;//timer para esperar la confirmacion de viaje
        private string _viajeId; // Guarda el ID del viaje generado al enviar la solicitud

        [ObservableProperty]
        private bool _isWaitingForConfirmation;

        private async Task StartPollingForConfirmation(string viajeId)
        {
            _viajeId = viajeId;
            IsWaitingForConfirmation = true;

            // Configurar el timer para ejecutarse cada 15 segundos
            _pollingTimer = new System.Timers.Timer(15000); // 15 segundos en milisegundos
            _pollingTimer.Elapsed += async (sender, e) => await CheckTripConfirmation();
            _pollingTimer.AutoReset = true;
            _pollingTimer.Enabled = true;

            // Primera verificación inmediata
            await CheckTripConfirmation();
        }

        private async Task CheckTripConfirmation()
        {
            if (string.IsNullOrEmpty(_viajeId)) return;

            try
            {
                var apiUrl = $"https://us-central1-apimototaxi.cloudfunctions.net/api/viajes/confirmarViaje/{_viajeId}";
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = System.Text.Json.JsonSerializer.Deserialize<ConfirmationResponse>(json);

                    if (result?.status == true)
                    {
                        // Viaje confirmado: Detener el polling y navegar a la siguiente pantalla
                        
                        IsWaitingForConfirmation = false;
                        IsVisibleLoadingViaje = false;
                        //await Shell.Current.DisplayAlert("¡Éxito!", "El mototaxista ha aceptado tu viaje", "OK");
                        //App.Current.MainPage.DisplayAlert("¡Éxito!", "El mototaxista ha aceptado tu viaje", "OK");
                        _pollingTimer?.Stop();
                        IdMototaxi = result.IdMoto;
                        await StartPollingForDetectionLatLon();
                        // Opcional: Navegar a pantalla de seguimiento
                        // await Shell.Current.GoToAsync(nameof(TripTrackingPage));
                    }
                }
               // var result = await _Service.VerificaViajeAsync(_viajeId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar confirmación: {ex.Message}");
                // No detenemos el polling ante errores (reintentará en 15 segundos)
            }
        }

        private void StopPolling()
        {
            _pollingTimer?.Stop();
            IsWaitingForConfirmation = false;
        }

        // Modelo para la respuesta de la API
        private class ConfirmationResponse
        {
            public bool status { get; set; }
            public string IdMoto { get; set; }
        }
        #endregion
    }
}
