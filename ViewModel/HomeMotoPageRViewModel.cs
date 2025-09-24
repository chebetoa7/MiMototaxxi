using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maui.GoogleMaps;
using MiMototaxxi.Services.Location;
using MiMototaxxi.Services.Moto;
using Plugin.Firebase.CloudMessaging;

namespace MiMototaxxi.ViewModel;

 public partial class HomeMotoPageRViewModel : BaseViewModel
{
     private readonly IMotoService _motoService;
        private readonly ILocationTrackerService _locationTracker;
        private readonly IDestinationMonitorService _destinationMonitor;
        private CancellationTokenSource _trackingCts;

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
        private string _pasaje;

        [ObservableProperty]
        private string _ubication;

        [ObservableProperty]
        private string _idViaje;

        [ObservableProperty]
        private double _latDes;

        [ObservableProperty]
        private double _lonDes;

        [ObservableProperty]
        private Position _currentPosition;

        [ObservableProperty]
        private Pin _userPin;

        [ObservableProperty]
        private Pin _fixedPin;

        [ObservableProperty]
        private double _currentDistance;

        public HomeMotoPageRViewModel(
            IMotoService motoService,
            ILocationTrackerService locationTracker,
            IDestinationMonitorService destinationMonitor)
        {
            _motoService = motoService;
            _locationTracker = locationTracker;
            _destinationMonitor = destinationMonitor;
            
            InitializeProperties();
            SubscribeToEvents();
        }

        private void InitializeProperties()
        {
            IsBtnGoJornada = true;
            CurrentPosition = new Position(19.4326, -99.1332);
            InitializeMotoPin();
        }

        private void SubscribeToEvents()
        {
            _locationTracker.PositionChanged += OnPositionChanged;
            _destinationMonitor.DistanceChanged += OnDistanceChanged;
            _destinationMonitor.DestinationReached += OnDestinationReached;
        }

        [RelayCommand]
        public async Task InitializeAsync()
        {
            await InitializeMap();
            await CheckPermissions();
        }

        private async Task InitializeMap()
        {
            try
            {
                var position = await _locationTracker.GetCurrentPositionAsync();
                if (position != null)
                {
                    CurrentPosition = position;
                    UserPin.Position = position;
                }
            }
            catch
            {
                SetDefaultLocation();
            }
        }

        private async Task CheckPermissions()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
        }

        private void InitializeMotoPin()
        {
            UserPin = new Pin
            {
                Type = PinType.Place,
                Position = CurrentPosition,
                Icon = BitmapDescriptorFactory.FromBundle("mi_icono"),
                Label = "Tú (Mototaxista)",
                IsDraggable = false
            };
        }

        [RelayCommand]
        private async Task StartJornada()
        {
            IsVisibleLoading = true;
            IsBtnGoJornada = false;
            IsBtnStopJornada = true;

            await StartTracking();
            
            var idUser = Preferences.Get("IdUsuario", string.Empty);
            await _motoService.UpdateStatusAsync("Disponible", idUser ?? "NA");
            await _motoService.UpdateTokenAsync(idUser ?? "NA");
            
            await Task.Delay(TimeSpan.FromSeconds(2));
            IsVisibleLoading = false;
        }

        [RelayCommand]
        private async Task StopJornada()
        {
            StopTracking();
            IsBtnGoJornada = true;
            IsBtnStopJornada = false;
            
            var idUser = Preferences.Get("IdUsuario", string.Empty);
            await _motoService.UpdateStatusAsync("SinServicio", idUser ?? "NA");
        }

        [RelayCommand]
        private async Task AcceptRide()
        {
            IsVisibleLoading = true;
            
            await AddFixedPin();
            
            var rideRequest = new Model.Moto.Viaje.AceptaViaje
            {
                lat = LatDes,
                lon = LonDes,
                CodigoConfirmacion = Generate6DigitCode()
            };
            
            await _motoService.AcceptRideAsync(rideRequest, IdViaje ?? "NA");

            Codigo = rideRequest.CodigoConfirmacion;
            IsSolicitudViaje = false;
            IsVisibleLoading = false;
            
            await StartDestinationMonitoring();
        }

        [RelayCommand]
        private void RejectRide()
        {
            IsSolicitudViaje = false;
        }

        private async Task StartTracking()
        {
            _trackingCts = new CancellationTokenSource();
            await _locationTracker.StartTrackingAsync(_trackingCts.Token);
        }

        private void StopTracking()
        {
            _trackingCts?.Cancel();
            _locationTracker.StopTracking();
            _destinationMonitor.StopMonitoring();
        }

        private async Task StartDestinationMonitoring()
        {
            if (FixedPin != null)
            {
                await _destinationMonitor.StartMonitoringAsync(FixedPin.Position, CurrentPosition);
            }
        }

        private async Task AddFixedPin()
        {
            var fixedPosition = new Position(LatDes, LonDes);
            FixedPin = new Pin
            {
                Label = $"Pasajero: {Pasaje}",
                Position = fixedPosition,
                Type = PinType.Place,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(0, 0, 255))
            };
        }

        private void OnPositionChanged(Position newPosition)
        {
            CurrentPosition = newPosition;
            UserPin.Position = newPosition;
            
            // Actualizar API en segundo plano
            _ = UpdateLocationInBackground(newPosition);
        }

        private async Task UpdateLocationInBackground(Position position)
        {
            try
            {
                var idUser = Preferences.Get("IdUsuario", string.Empty);
                var locationData = new Model.Moto.LocationMoto
                {
                    Lat = position.Latitude,
                    Lon = position.Longitude
                };

                await _motoService.UpdateLocationAsync(locationData, idUser ?? "NA");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error actualizando ubicación: {ex.Message}");
            }
        }

        private void OnDistanceChanged(double distance)
        {
            CurrentDistance = distance;
        }

        private async void OnDestinationReached()
        {
            var status = new Model.Moto.Viaje.StatusModel { estatus = "llego" };
            await _motoService.UpdateViajeStatusAsync(status, IdViaje ?? "NA");
            
            Console.WriteLine("¡Has llegado al destino!");
        }

        public void ProcessNotification(IDictionary<string, string> data)
        {
            if (data.TryGetValue("ubicacion", out var ubicacion)) Ubication = ubicacion;
            if (data.TryGetValue("pasajero", out var pasajero)) Pasaje = pasajero;
            if (data.TryGetValue("idViaje", out var idViaje)) IdViaje = idViaje;
            if (data.TryGetValue("lat", out var latStr)) LatDes = double.Parse(latStr);
            if (data.TryGetValue("lon", out var lonStr)) LonDes = double.Parse(lonStr);

            IsSolicitudViaje = true;
        }

        private string Generate6DigitCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        private void SetDefaultLocation()
        {
            CurrentPosition = new Position(19.4326, -99.1332);
            UserPin.Position = CurrentPosition;
        }
}