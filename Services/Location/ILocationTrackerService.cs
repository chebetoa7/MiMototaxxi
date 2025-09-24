using Maui.GoogleMaps;


namespace MiMototaxxi.Services.Moto
{
    public interface ILocationTrackerService
    {
        event Action<Position> PositionChanged;
        bool IsTracking { get; }
        
        Task StartTrackingAsync(CancellationToken cancellationToken);
        void StopTracking();
        Task<Position> GetCurrentPositionAsync();
    }
}
