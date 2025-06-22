using MiMototaxxi.Model.Credential;
using MiMototaxxi.Model.Moto.Response.Viaje;
using MiMototaxxi.Model.SMS;
using MiMototaxxi.Model.Usuario;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MiMototaxxi.Services
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        public UsuarioService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://us-central1-apimototaxi.cloudfunctions.net/"); // URL de tu API
        }

        public async Task<string> VerificaViajeAsync(string idViaje)
        {
            try
            {
                // Hacer la solicitud POST
                var response = await _httpClient.GetAsync($"api/viajes/confirmarViaje/{idViaje}");

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

        public async Task<ResponseSolicitudViaje> EnviarSolicitudViaje(SolicitudViaje datoViaje, string region)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };
                // Serializar el objeto viaje
                string json = JsonConvert.SerializeObject(datoViaje, settings);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                // Hacer la solicitud POST
                var response = await _httpClient.PostAsync("api/mototaxis/viaje/" + region + "?" + "apikeymaps=" + "NA", content);

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var resultModel = System.Text.Json.JsonSerializer.Deserialize<ResponseSolicitudViaje>(responseData);
                    return resultModel;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al enviar solicitud: {errorResponse}");
                }
            }
            catch (Exception exM)
            {
                throw new Exception($"Error al enviar solicitud: {exM.Message}");
            }
        }

        public async Task<LocationResponse> GetLocationMoto(string idMoto)
        {
            try
            {

                // Hacer la solicitud GET
                var response = await _httpClient.GetAsync("api/mototaxis/locationMoto/" + idMoto);

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var resultModel = System.Text.Json.JsonSerializer.Deserialize<LocationResponse>(responseData);
                    return resultModel;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al enviar solicitud: {errorResponse}");
                }
            }
            catch (Exception exM)
            {
                throw new Exception($"Error al enviar solicitud: {exM.Message}");
            }
        }
        

        public async Task<LocationResponse> GetMotoxita(string idMoto)
        {
            try
            {

                // Hacer la solicitud GET
                var response = await _httpClient.GetAsync("api/mototaxis/locationMoto/" + idMoto);

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var resultModel = System.Text.Json.JsonSerializer.Deserialize<LocationResponse>(responseData);
                    return resultModel;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al enviar solicitud: {errorResponse}");
                }
            }
            catch (Exception exM)
            {
                throw new Exception($"Error al enviar solicitud: {exM.Message}");
            }
        }
        

        public async Task<string> RegistrarUsuarioAsync(Usuario usuario)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };
                // Serializar el objeto Usuario a JSON
                string json = JsonConvert.SerializeObject(usuario, settings);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Hacer la solicitud POST
                var response = await _httpClient.PostAsync("api/usuarios", content);

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
        public async Task<string> ValidarCodigoAsync(sms mensaje)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };
                // Serializar el objeto Usuario a JSON
                // var json = JsonSerializer.Serialize(usuario);
                string json = JsonConvert.SerializeObject(mensaje, settings);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Hacer la solicitud POST
                var response = await _httpClient.PostAsync("api/usuarios/validar-codigo", content);

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al validar usuario: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud HTTP: {ex.Message}");
            }
        }
        public async Task<string> ValidarUsuarioAsync(validation login)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                };
                // Serializar el objeto Usuario a JSON
                string json = JsonConvert.SerializeObject(login, settings);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Hacer la solicitud POST
                var response = await _httpClient.PostAsync("api/usuarios/valida-phone-lg", content);

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error en login usuario: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud HTTP: {ex.Message}");
            }
        }

    }
}
