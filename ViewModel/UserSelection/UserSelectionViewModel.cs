using MiMototaxxi.View.Mototaxxi;
using MiMototaxxi.View.Pasajero;
using Plugin.Firebase.CloudMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiMototaxxi.ViewModel.UserSelection
{
    public class UserSelectionViewModel : BaseViewModel
    {
        #region Command
        public ICommand GoUserMototaxiCommand { get; set; }
        public ICommand GoUserPasajeroCommand { get; set; }
        #endregion

        #region Constructor
        public UserSelectionViewModel()
        {
            // En algún lugar de tu código (ej: al iniciar la app)
          
            GoUserMototaxiCommand = new Command(async () => await FuntionGoMototaxiCommand());
            GoUserPasajeroCommand = new Command(async () => await FuntionGoPasajeroCommand());
        }
        #endregion

        #region Funtion
        private async Task FuntionGoMototaxiCommand()
        {
            try
            {
                Token();
                var page = new MotoTaxiLoginPage();
                await NavigateAsync(page);
            }
            catch (Exception exM)
            {
                Console.WriteLine($"Error: FuntionGoMototaxiCommand -> {exM.Message} ");
            }
        }
        private async void Token()
        {
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            Console.WriteLine($"FCM token: {token}");
        }
        private async Task FuntionGoPasajeroCommand()
        {
            try
            {
                Token();
                var page = new PasajeroLoginPage(); 
                await NavigateAsync(page);
            }
            catch (Exception exM)
            {
                Console.WriteLine($"Error: FuntionGoPasajeroCommand -> {exM.Message} ");
            }
        }
        #endregion
    }
}
