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
    public class MotoService
    {
        private readonly HttpClient _httpClient;
        public MotoService() 
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://us-central1-apimototaxi.cloudfunctions.net/"); // URL de tu API
        }

        public async Task<string> RegistrarMotoAsync(Model.Moto.Moto moto)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };
                // Serializar el objeto Usuario a JSON
                string json = JsonConvert.SerializeObject(moto, settings);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Hacer la solicitud POST
                var response = await _httpClient.PostAsync("api/mototaxis", content);

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al registrar usuario: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud HTTP: {ex.Message}");
            }
        }

        public async Task<string> AceptaViajeAsync(Model.Moto.Viaje.AceptaViaje viaje, string id)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };
                // Serializar el objeto Usuario a JSON
                string json = JsonConvert.SerializeObject(viaje, settings);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Hacer la solicitud POST
                var response = await _httpClient.PutAsync($"api/viajes/pinUp/{id}", content);

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al registrar usuario: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud HTTP: {ex.Message}");
            }
        }
        
        public async Task<string> updateLocationMotoNoAsync(Model.Moto.LocationMoto locationMoto, string id)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii 
                };
                // Serializar el objeto Usuario a JSON
                string json = JsonConvert.SerializeObject(locationMoto, settings);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var cts = new CancellationTokenSource(5000);
                // Hacer la solicitud POST
                var response = await _httpClient.PutAsync(
                    $"api/mototaxis/location/{id}",
                    content,
                    cts.Token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
                // Verificar si la solicitud fue exitosa

            }
            catch (Exception ex)
            {
                Console.WriteLine("Registro de token Error: " + ex.Message);
                return string.Empty;
            }
        }
        public async Task<string> updateLocationMotoAsync(Model.Moto.LocationMoto locationMoto, string id)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };
                // Serializar el objeto Usuario a JSON
                string json = JsonConvert.SerializeObject(locationMoto, settings);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Hacer la solicitud POST
                var response = await _httpClient.PutAsync("api/mototaxis/location/" + id, content);

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("Registro de token exitoso: " + responseData);
                    return responseData;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("Registro de token response bug: " + errorResponse);
                    throw new Exception($"Error al actualizar usuario: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Registro de token Error: " + ex.Message);
                throw new Exception($"Error en la solicitud HTTP: {ex.Message}");
            }
        }
        public async Task<string> updateMotoAsync(object data, string id)
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
        }

        //Api para modificar el status del viaje
        public async Task<string> UpdateViajeStatusAsync(StatusModel status, string id)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };
                // Serializar el objeto Usuario a JSON
                string json = JsonConvert.SerializeObject(status, settings);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Hacer la solicitud POST
                var response = await _httpClient.PutAsync($"api/viajes/"+id, content);

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al registrar usuario: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud HTTP: {ex.Message}");
            }
        }
    }
}
