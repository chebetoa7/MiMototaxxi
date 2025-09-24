using Maui.GoogleMaps;

public interface IDestinationMonitorService
{
    event Action DestinationReached;
    event Action<double> DistanceChanged;
    bool IsMonitoring { get; }
    double CurrentDistance { get; }
    
    Task StartMonitoringAsync(Position destination, Position startPosition);
    void StopMonitoring();
    double CalculateDistance(Position position1, Position position2);
}