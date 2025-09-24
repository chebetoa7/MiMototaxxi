using Maui.GoogleMaps;
using Microsoft.Maui.Devices.Sensors;
using MiMototaxxi.Services.Moto;
using System.Diagnostics;

namespace MiMototaxxi.Services.LocationTracking
{
    public class LocationTrackerService : ILocationTrackerService
    {
        public bool IsTracking { get; private set; } 

        public event Action<Position> PositionChanged;
        
        private CancellationTokenSource _trackingCts;
 
        public async Task StartTrackingAsync(CancellationToken cancellationToken)
        {
            if (IsTracking) return;
            
            IsTracking = true; // ✅ Ahora puedes asignar valor
            _trackingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            await TrackLocationContinuously(_trackingCts.Token);
        }

        public void StopTracking()
        {
            IsTracking = false;
            _trackingCts?.Cancel();
        }

        public async Task<Position> GetCurrentPositionAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                var location = await Geolocation.Default.GetLocationAsync(request);
                
                if (location != null)
                {
                    return new Position(location.Latitude, location.Longitude);
                }
                
                // En lugar de null, retornar una posición por defecto o lanzar una excepción
                Debug.WriteLine("No se pudo obtener la ubicación, retornando posición por defecto");
                return new Position(0, 0); // O una posición por defecto específica
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Debug.WriteLine($"GPS no soportado: {fnsEx.Message}");
                return new Position(0, 0);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Debug.WriteLine($"GPS deshabilitado: {fneEx.Message}");
                return new Position(0, 0);
            }
            catch (PermissionException pEx)
            {
                Debug.WriteLine($"Permisos denegados: {pEx.Message}");
                return new Position(0, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo ubicación: {ex.Message}");
                return new Position(0, 0);
            }
        }

        private async Task TrackLocationContinuously(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && IsTracking)
            {
                try
                {
                    var position = await GetCurrentPositionAsync();
                    PositionChanged?.Invoke(position);
                    
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Tracking cancelado
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error en tracking continuo: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                }
            }
        }
    }
}
