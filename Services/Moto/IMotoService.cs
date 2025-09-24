using MiMototaxxi.Model.Moto;
using MiMototaxxi.Model.Moto.Viaje;


public interface IMotoService
    {
        Task<bool> UpdateLocationAsync(LocationMoto location, string motoId);
        Task<bool> UpdateMotoAsync(DtoEstatus status, string userId);
        Task<bool> UpdateTokenAsync(MtoToken token, string userId);
        Task<bool> AcceptRideAsync(AceptaViaje ride, string rideId);
        Task<bool> UpdateViajeStatusAsync(StatusModel status, string rideId);
        Task<bool> UpdateStatusAsync(string status, string userId);
        Task<bool> UpdateTokenAsync(string userId);
        Task<string> RegistrarMotoAsync(Moto moto);
}

