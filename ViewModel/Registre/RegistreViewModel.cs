using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiMototaxxi.Model.Moto;
using MiMototaxxi.Model.SMS;
using MiMototaxxi.Model.Usuario;
using MiMototaxxi.Services;
using MiMototaxxi.Services.Moto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiMototaxxi.ViewModel.Registre
{
    public partial class RegistreViewModel : BaseViewModel
    {
        #region Vars
        private readonly UsuarioService _usuarioService;
        private readonly MotoService _motoService;

        [ObservableProperty]
        private bool _isVisible;

        [ObservableProperty]
        private bool _isVisibleloading;

        [ObservableProperty]
        private bool _isVisibleButton;

        [ObservableProperty]
        private bool _isVisibleRegistreMoto;

        [ObservableProperty]
        private bool _isVisibleRegistroUsuario;

        [RelayCommand]
        private void ToggleVisibility()
        {
            IsVisible = !IsVisible;
        }

        [ObservableProperty]
        private string _codigoActiva;

        [ObservableProperty]
        private string _password1;

        [ObservableProperty]
        private string _password2;

        // Propiedades para enlazar con la interfaz
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private string phone;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string sex;

        [ObservableProperty]
        private int age;

        [ObservableProperty]
        private string briday;

        [ObservableProperty]
        private string typeUser;

        [ObservableProperty]
        private string statusTable;

        [ObservableProperty]
        private string dateRegistre;

        [ObservableProperty]
        private string cp;

        [ObservableProperty]
        private string metadatos;

        [ObservableProperty]
        private int estrellas;

        [ObservableProperty]
        private string pass;

        [ObservableProperty]
        private string fcmToken;

        //valida codigo
        [ObservableProperty]
        private string id_ca;

        //Propiedades para agegar moto
        [ObservableProperty]
        private string marca;

        [ObservableProperty]
        private string modelo;

        [ObservableProperty]
        private string anio;

        [ObservableProperty]
        private string color;

        [ObservableProperty]
        private string des;
        //end moto



        #endregion
        #region Command
        // Comando para manejar la selección de edad
        public ICommand SelectAgeCommand { get; }
        public ICommand RegistrarUsuarioCommand { get; }
        public ICommand RegistrarMotoCommand { get; }
        
        public ICommand ValidaUsuarioCommand { get; }

        #endregion

        #region Properties

        // Propiedad para almacenar la edad seleccionada
        [ObservableProperty]
        private string selectedAge;

        // Lista de edades para el Picker
        public ObservableCollection<string> Ages { get; set; } = new ObservableCollection<string>();

        #endregion

        #region Constructor
        public RegistreViewModel(string typeUser) 
        {
            IsVisibleButton = true;
            IsVisibleRegistroUsuario = true;
            this.typeUser = typeUser;
            _usuarioService = new UsuarioService();
            _motoService = new MotoService();
            RegistrarUsuarioCommand = new Command(async () => await RegistrarUsuario());
            RegistrarMotoCommand = new Command(async () => await RegistrarMoto());
            ValidaUsuarioCommand = new Command(async () => await ValidarCodigoSMS()); 
            // Llenar la lista de edades (17 a 59)
            for (int age = 17; age <= 59; age++)
            {
                Ages.Add(age.ToString());
            }

            // Inicializar el comando
            SelectAgeCommand = new Command(OnSelectAge);
        }
        #endregion

        #region Methods

        // Método para manejar la selección de edad
        private void OnSelectAge()
        {
            if (!string.IsNullOrEmpty(selectedAge))
            {
                // Aquí puedes agregar la lógica que deseas ejecutar con la edad seleccionada
                Console.WriteLine($"Edad seleccionada: {selectedAge}");
            }
        }

        //Validar Codigo
        [RelayCommand]
        private async Task ValidarCodigoSMS() 
        {
            string codigoIngresado = CodigoActiva;
            string pass_ = Password1;
            try 
            {
                var sms_ = new sms
                {
                    Id = id_ca,
                    codigoActivacion = codigoIngresado,
                    password = pass_,

                };
                IsVisibleloading = true;
                IsVisibleButton = false;
                var result = await _usuarioService.ValidarCodigoAsync(sms_);
                IsVisibleloading = false;
                IsVisibleButton = true;
                IsVisible = false;
                // Mostrar el resultado
                await Shell.Current.DisplayAlert("Éxito", "Usuario validado exitosamente", "OK");
               await CloseNavigationAsync();

            }
            catch(Exception exM) 
            {
                Console.WriteLine("Error: ValidarCodigoSMS, " + exM.Message);
                IsVisibleloading = false;
                IsVisibleButton = true;
                await Application.Current.MainPage.DisplayAlert("Error","Opps! Ocurrio Algo inesperado","OK");
            }
        }

        //Método para agregar nuevo usuario
        string idUsuarioNew = "";
        private async Task RegistrarUsuario()
        {
            string tokenfcm = "NA"; 
            try
            {
                var _telefono = "+52" + phone;
                var tokenguardado = Preferences.Get("TokenFCM", string.Empty);
                if (!string.IsNullOrEmpty(tokenguardado))
                {
                    // octener token
                    tokenfcm = tokenguardado;
                   
                }
                // Crear un nuevo usuario
                var usuario = new Usuario
                {
                    Id = "US" + Guid.NewGuid().ToString(), // Generar un ID único
                    Name = name,
                    LastName = lastName,
                    Phone = _telefono,
                    Email = email,
                    Sex = string.IsNullOrEmpty(Sex) ? "N/A" : Sex,
                    Age = Int32.Parse(selectedAge),
                    Briday = string.IsNullOrEmpty(briday) ? "N/A" : briday,
                    TypeUser = string.IsNullOrEmpty(typeUser) ? "Pasajero" : typeUser,
                    StatusTable = string.IsNullOrEmpty(StatusTable) ? "Activo" : statusTable,
                    DateRegistre = DateTime.Now.ToString(),
                    CP = cp,
                    Metadatos = string.IsNullOrEmpty(Metadatos) ? "N/A" : metadatos,
                    Estrellas = Estrellas == null ? 0 : estrellas,
                    pass = string.IsNullOrEmpty(pass) ? "NA" : pass,
                    fcmToken = tokenfcm,
                };
                idUsuarioNew = usuario.Id;

                // Registrar el usuario
                var resultado = await _usuarioService.RegistrarUsuarioAsync(usuario);
                IsVisibleRegistroUsuario = false;

                id_ca = usuario.Id;

                // Mostrar el resultado
                await Shell.Current.DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");

               // IsVisible = true;
                IsVisibleRegistreMoto = true;

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al registrar usuario: {ex.Message}", "OK");
            }
        }

        //Método para agregar nuevo moto
        private async Task RegistrarMoto()
        {
            string tokenfcm = "NA";
            try
            {
                
                IsVisibleloading = true;
                var tokenguardado = Preferences.Get("TokenFCM", string.Empty);
                if (!string.IsNullOrEmpty(tokenguardado))
                {
                    // octener token
                    tokenfcm = tokenguardado;

                }
                // Crear un nuevo usuario
                var moto = new Moto
                {
                    Id = "MT" + Guid.NewGuid().ToString(), // Generar un ID único
                    Operador = string.IsNullOrEmpty(name) ? "N/A" : name,
                    Marca = string.IsNullOrEmpty(marca) ? "N/A" : marca,
                    Modelo = string.IsNullOrEmpty(modelo) ? "N/A" : modelo,
                    Color = string.IsNullOrEmpty(color) ? "N/A" : color,
                    Age = Age,
                    Caracteristicas = string.IsNullOrEmpty(des) ? "N/A" : des,
                    StatusTable = string.IsNullOrEmpty(StatusTable) ? "Activo" : statusTable,
                    Estatus = "Descanso",
                    IdUsuario = idUsuarioNew,
                    DateRegistre = DateTime.Now.ToString(),
                    CP = cp,
                    Lat = "NA",
                    Lon = "NA",
                    Dir = "NA",
                    Distancias = "NA",
                    Estrellas = "0",
                    Metadatos = string.IsNullOrEmpty(Metadatos) ? "N/A" : metadatos,
                    fcmToken = string.IsNullOrEmpty(tokenfcm) ? "NA" : tokenfcm,
                };

                // Registrar el usuario
                var resultado = await _motoService.RegistrarMotoAsync(moto);

                IsVisibleloading = false;
               
                // Mostrar el resultado
                await Shell.Current.DisplayAlert("Éxito", " Moto registrado correctamente", "OK");

                IsVisibleRegistreMoto = false;
                IsVisible = true;
                

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al registrar usuario: {ex.Message}", "OK");
            }
            IsVisibleloading = false;
        }

        #endregion
    }
}
