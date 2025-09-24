using MiMototaxxi.Model;
using MiMototaxxi.Model.Moto;
using MiMototaxxi.Model.Moto.Viaje;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMototaxxi.Services.Moto
{
    public class MotoService : IMotoService
    {
        private readonly HttpClient _httpClient;

        public MotoService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://us-central1-apimototaxi.cloudfunctions.net/");
        }

        // ✅ Implementación CORRECTA de UpdateLocationAsync
        public async Task<bool> UpdateLocationAsync(LocationMoto location, string motoId)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };

                string json = JsonConvert.SerializeObject(location, settings);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/mototaxis/location/{motoId}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error actualizando ubicación: {ex.Message}");
                return false;
            }
        }

        

        // ✅ Implementación CORRECTA de AcceptRideAsync
        public async Task<bool> AcceptRideAsync(AceptaViaje ride, string rideId)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };

                string json = JsonConvert.SerializeObject(ride, settings);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/viajes/pinUp/{rideId}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error aceptando viaje: {ex.Message}");
                return false;
            }
        }

        // ✅ Implementación CORRECTA de UpdateViajeStatusAsync
        public async Task<bool> UpdateViajeStatusAsync(StatusModel status, string rideId)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };

                string json = JsonConvert.SerializeObject(status, settings);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/viajes/{rideId}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error actualizando estado de viaje: {ex.Message}");
                return false;
            }
        }

        // ✅ Métodos restantes de la interfaz (implementación básica)
        public async Task<bool> UpdateMotoAsync(DtoEstatus status, string userId)
        {
            // Implementar lógica real aquí
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateTokenAsync(MtoToken token, string userId)
        {
            // Implementar lógica real aquí
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateStatusAsync(string status, string userId)
        {
            // Implementar lógica real aquí
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateTokenAsync(string userId)
        {
            // Implementar lógica real aquí
            return await Task.FromResult(true);
        }

        // ✅ Método ya existente que sí coincide
        public async Task<string> RegistrarMotoAsync(Model.Moto.Moto moto)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };

                string json = JsonConvert.SerializeObject(moto, settings);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/mototaxis", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al registrar moto: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud HTTP: {ex.Message}");
            }
        }

        


    }
}
