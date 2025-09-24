using Maui.GoogleMaps;
using Timer = System.Timers.Timer;

namespace MiMototaxxi.Services.Location
{
    public class DestinationMonitorService : IDestinationMonitorService
    {
        public event Action DestinationReached;
        public event Action<double> DistanceChanged;
        public bool IsMonitoring { get; private set; }
        public double CurrentDistance { get; private set; }
        
        private Position _destination;
        private Position _currentPosition;
        private Timer _monitoringTimer;
        private const double DestinationThresholdMeters = 20;

        public async Task StartMonitoringAsync(Position destination, Position startPosition)
        {
            if (IsMonitoring) return;
            
            _destination = destination;
            _currentPosition = startPosition;
            IsMonitoring = true;

            _monitoringTimer = new Timer(3000); // Verificar cada 3 segundos
            _monitoringTimer.Elapsed += async (s, e) => await CheckDistanceAsync();
            _monitoringTimer.Start();

            // Verificación inicial
            await CheckDistanceAsync();
        }

        public void StopMonitoring()
        {
            _monitoringTimer?.Stop();
            _monitoringTimer?.Dispose();
            IsMonitoring = false;
        }

        private async Task CheckDistanceAsync()
        {
            if (_currentPosition == null || _destination == null) return;

            var distance = CalculateDistance(_currentPosition, _destination);
            CurrentDistance = distance;
            DistanceChanged?.Invoke(distance);

            if (distance <= DestinationThresholdMeters)
            {
                StopMonitoring();
                DestinationReached?.Invoke();
            }
        }

        public void UpdateCurrentPosition(Position newPosition)
        {
            _currentPosition = newPosition;
        }

        public double CalculateDistance(Position position1, Position position2)
        {
            double R = 6371000;
            var dLat = (position2.Latitude - position1.Latitude) * Math.PI / 180;
            var dLon = (position2.Longitude - position1.Longitude) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(position1.Latitude * Math.PI / 180) *
                    Math.Cos(position2.Latitude * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}
