using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MiMototaxxi.Services.Moto
{
    public class ApiServiceMoto : IApiServiceMoto
    {
        private readonly HttpClient _httpClient;

        public ApiServiceMoto()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://us-central1-apimototaxi.cloudfunctions.net/");
        }

        // ✅ Implementación CORRECTA de UpdateLocationAsync
        public async Task<bool> UpdateMotoAsync(Object data, string motoId)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };

                string json = JsonConvert.SerializeObject(data, settings);
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

        /*
        public async Task<bool> UpdateTokenAsync(Object data, string userId)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };
                // Serializar el objeto Usuario a JSON
                string json = JsonConvert.SerializeObject(data, settings);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Hacer la solicitud POST
                var response = await _httpClient.PutAsync("api/mototaxis/location/" + id, content);

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Registro de token exitoso: " + responseData);
                    return responseData;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Registro de token response bug: " + errorResponse);
                    throw new Exception($"Error al actualizar usuario: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Registro de token Error: " + ex.Message);
                throw new Exception($"Error en la solicitud HTTP: {ex.Message}");
            }
        }*/

    }
}
