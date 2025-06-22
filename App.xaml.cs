
using MiMototaxxi.Model.Config;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.CloudMessaging.EventArgs;
using System.Text.Json;

namespace MiMototaxxi
{
    public partial class App : Application
    {
        private bool _firebaseInitialized = false;
        public App()
        {
            InitializeComponent();

            // 1. Inicialización temprana de Firebase
            InitializeFirebase();

           /* CrossFirebaseCloudMessaging.Current.TokenChanged += (s, token) => 
            {
                Console.WriteLine($"🔥 Token FCM: {token}");
                GuardaTokenPreference(token.Token);
                // Aquí envía el token a tu backend
            };


            // Fuerza generación de token después de 5 segundos
            _ = Task.Run(async () => 
            {
                await Task.Delay(500);
                try 
                {
                    var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                    Console.WriteLine($"Token obtenido manualmente: {token}");
                    GuardaTokenPreference(token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener token: {ex.Message}");
                }
            });*/
            MainPage = new AppShell();
        }
        private async void OnTokenChanged(object sender, FCMTokenChangedEventArgs e)
        {
            Console.WriteLine("OnTokenChanged");
            Console.WriteLine($"🔥 TokenChanged recibido! Nuevo token: {e.Token}");
            await SecureStorage.SetAsync("FCM_Token", e.Token);
            await SendTokenToServer(e.Token); // Implementa este método
        }
        private async Task GetAndStoreFcmToken()
        {
            try
            {
                Console.WriteLine("GetAndStoreFcmToken");
                var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    Console.WriteLine($"Token obtenido: {token}");
                    await SecureStorage.SetAsync("FCM_Token", token);
                    await SendTokenToServer(token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 Error obteniendo token: {ex.Message}");
            }
        }
        private async Task SendTokenToServer(string token)
        {
            // Implementa tu lógica para enviar el token al backend
            Console.WriteLine($"Enviando token al servidor: {token}");
            // await yourApiService.UpdateDeviceToken(token);
            GuardaTokenPreference(token);
        }

        protected override async void OnStart()
        {
            base.OnStart();
            // Verificación periódica del token
            await GetAndStoreFcmToken();
        }
        private async void InitializeFirebase()
        {
            try
            {
                if (!_firebaseInitialized)
                {
                    // 2. Configuración esencial para Android
#if ANDROID
                    Firebase.FirebaseApp.InitializeApp(Platform.CurrentActivity);
#endif
                    // 3. Registrar el evento ANTES de cualquier operación con FCM
                    //CrossFirebaseCloudMessaging.Current.TokenChanged += OnTokenChanged;

                    // 4. Forzar la obtención del token inicial
                    await Task.Delay(1000); // Pequeño delay para asegurar inicialización
                    await GetAndStoreFcmToken();

                    _firebaseInitialized = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 Error inicializando Firebase: {ex.Message}");
            }
        }

        private void GuardaTokenPreference(string token) 
       {
            try 
            {
                if (!string.IsNullOrEmpty(token))
                {
                    Console.WriteLine($"Nuevo token: {token}" );
                    var tkdivice = Preferences.Get("TokenFCM", string.Empty);
                    Console.WriteLine($"Nuevo anterior token: {tkdivice}" );
                    if (tkdivice != "")
                    {
                        if (token != tkdivice)
                        {

                            var tokenguardado = Preferences.Get("TokenFCM", string.Empty);
                            if (tokenguardado == "")
                            {
                                Console.WriteLine("Se actualizo el token");
                                Preferences.Remove("TokenFCM");                            // Guardar en Preferences
                                Preferences.Set("TokenFCM", token);
                            }
                        }

                    }
                    else
                    {
                        Console.WriteLine($"Primera vez generando el token:{token}.");
                        //SecureStorage.SetAsync("TokenFCM", token);
                         Preferences.Set("TokenFCM", token);
                        
                    }
                   
                }
            }
            catch (Exception exM) 
            {
                Console.WriteLine("Error, GuardarTokenPreference");
            }
       }

        /*public static void SaveConfig(ConfigApp config)
        {
            try
            {
                // Serializar el objeto a JSON
                var json = JsonSerializer.Serialize(config);

                // Guardar el JSON en Preferences
                Preferences.Set("AppConfig", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar configuración: {ex.Message}");
            }
        }*/

    }

}
