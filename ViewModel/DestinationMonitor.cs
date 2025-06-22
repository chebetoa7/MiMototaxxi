using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Maui.GoogleMaps;

namespace MiMototaxxi.ViewModel
{

    public class DestinationMonitor
    {
        //Client PinUp
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(10);
        private readonly double _arrivalThresholdMetersPinUp = 30;
        private CancellationTokenSource _ctspinUp;
        private Position _pinUp;
        public event Action OnpinUpReached;
        public event Action<double> OnDistanceChangedPinUp;

        //Destination
         private readonly double _arrivalThresholdMetersDestination = 25; // 40 metros de radio
        private CancellationTokenSource _ctsDestination;
        private Position _destination;
        public event Action OnDestinationReachedDestination;
        public event Action<double> OnDistanceChangedDestination;

        public async Task StartMonitoringUp(Position PinUp)
        {
            _pinUp = PinUp;
            _ctspinUp = new CancellationTokenSource();

            try
            {
                while (!_ctspinUp.IsCancellationRequested)
                {
                    var location = await GetCurrentLocation();
                    if (location != null)
                    {
                        var currentPosition = new Position(location.Latitude, location.Longitude);
                        var distance = CalculateDistance(currentPosition, _pinUp);

                        OnDistanceChangedPinUp?.Invoke(distance);

                        if (distance <= _arrivalThresholdMetersPinUp)
                        {
                            OnpinUpReached?.Invoke();
                            break;
                        }
                    }

                    await Task.Delay(_updateInterval, _ctspinUp.Token);
                }
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("Monitoring stopped");
            }
        }

        public async Task StartMonitoring(Position destination)
        {
            _destination = destination;
            _ctsDestination = new CancellationTokenSource();

            try
            {
                while (!_ctsDestination.IsCancellationRequested)
                {
                    var location = await GetCurrentLocation();
                    if (location != null)
                    {
                        var currentPosition = new Position(location.Latitude, location.Longitude);
                        var distance = CalculateDistance(currentPosition, _destination);

                        OnDistanceChangedDestination?.Invoke(distance);

                        if (distance <= _arrivalThresholdMetersDestination)
                        {
                            OnDestinationReachedDestination?.Invoke();
                            break;
                        }
                    }

                    await Task.Delay(_updateInterval, _ctsDestination.Token);
                }
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("Monitoring stopped");
            }
        }

        public void StopMonitoringDestination()
        {
            _ctsDestination?.Cancel();
        }
        public void StopMonitoringPinUp()
        {
            _ctspinUp?.Cancel();
        }

        private async Task<Location> GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                return await Geolocation.GetLocationAsync(request);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting location: {ex.Message}");
                return null;
            }
        }

        private double CalculateDistance(Position start, Position end)
        {
            // Fórmula Haversine para calcular distancia en metros
            double lat1 = start.Latitude * Math.PI / 180;
            double lon1 = start.Longitude * Math.PI / 180;
            double lat2 = end.Latitude * Math.PI / 180;
            double lon2 = end.Longitude * Math.PI / 180;

            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return 6371000 * c; // Radio de la Tierra en metros
        }
    }
}
