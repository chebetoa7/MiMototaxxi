using CommunityToolkit.Mvvm.ComponentModel;
using MiMototaxxi.Model.Credential;
using MiMototaxxi.Model.Usuario;
using MiMototaxxi.Services;
using MiMototaxxi.View.HomeMototaxiPage;
using MiMototaxxi.View.Registre;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiMototaxxi.ViewModel.Mototaxxi
{
    public partial class MotoTaxiLoginViewModel : BaseViewModel
    {
        #region Vars
        private readonly UsuarioService _usuarioService;

        [ObservableProperty]
        private bool _isVisible;

        [ObservableProperty]
        private string _phone;

        [ObservableProperty]
        private string _pass;

        #endregion
        #region Command
        public ICommand RegistroMotoTaxiCommand { get; set; }
        public ICommand ValidarCommand { get; set; }
        #endregion
        #region Constructor
        public MotoTaxiLoginViewModel()
        {
            _usuarioService = new UsuarioService();
            RegistroMotoTaxiCommand = new Command(async () => await FuntionRegistroMotoTaxistaCommand());
            ValidarCommand = new Command(async () => await FuntionValidarCommand());
        }


        #endregion

        #region Funtion
        private async Task FuntionValidarCommand()
        {
            try
            {
                var dataValidation = new validation
                {
                    phone = "+52"+Phone,
                    password =Pass
                };
                IsVisible = true;
                var result = await _usuarioService.ValidarUsuarioAsync(dataValidation);
                // Intenta deserializar como una respuesta exitosa
                var usuarioResponse = JsonConvert.DeserializeObject<UsuarioResponse>(result);

                if (usuarioResponse != null && usuarioResponse.Message == "Credenciales válidas")
                {
                    // Respuesta exitosa
                    var idUser = Preferences.Get("IdUsuario", string.Empty);
                    if (idUser == "")
                    {
                        // Guardar en Preferences
                        Preferences.Set("IdUsuario",usuarioResponse.IdUsuario);

                    }
                    else
                    {
                        //Eliminar usuario y volver a guardar
                        Preferences.Remove("IdUsuario");
                        // Guardar en Preferences
                        Preferences.Set("IdUsuario", usuarioResponse.IdUsuario);
                    }
                        Console.WriteLine("Usuario válido: " + usuarioResponse.Usuario.Name);
                    await Shell.Current.DisplayAlert("Mensaje", "Bienvenido: " + usuarioResponse.Usuario.Name, "OK");
                    IsVisible = false;
                    // Navega a la página de inicio
                    var page = new HomeMotoPage();
                    await NavigateAsync(page);
                }
                else
                {
                    // Intenta deserializar como una respuesta de error
                    var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(result);

                    if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Error))
                    {
                        // Respuesta de error
                        Console.WriteLine("Error: " + errorResponse.Error);
                        IsVisible = false;
                        await Shell.Current.DisplayAlert("Error", errorResponse.Error, "OK");
                    }
                    else
                    {
                        IsVisible = false;
                        // Respuesta inesperada
                        Console.WriteLine("Respuesta: " + result);
                        await Shell.Current.DisplayAlert("Respuesta", result, "OK");
                    }
                }

                /*var page = new HomeMotoPage();
                await NavigateAsync(page);*/
            }
            catch (Exception exM)
            {
                Console.WriteLine($"Error: FuntionRegistroMotoTaxistaCommand -> {exM.Message} ");
                await Shell.Current.DisplayAlert("Error", "Contraseña incorrecta", "OK");
                IsVisible = false;
            }
        }
        private async Task FuntionRegistroMotoTaxistaCommand()
        {
            try
            {
                var page = new RegistrePage("Mototaxi");
                await NavigateAsync(page);
            }
            catch (Exception exM)
            {
                Console.WriteLine($"Error: FuntionRegistroMotoTaxistaCommand -> {exM.Message} ");
            }
        }
        #endregion
    }
}
