using MiMototaxxi.ViewModel.UserSelection;
using Plugin.Firebase.CloudMessaging;

namespace MiMototaxxi.View.UserSelection;

public partial class UserSelectionPage : ContentPage
{
    #region Vars
    public readonly UserSelectionViewModel ViewModel;
    #endregion
    public UserSelectionPage()
	{
		InitializeComponent();
        BindingContext = ViewModel = new UserSelectionViewModel();
        //CrossFirebaseCloudMessaging.Current.TokenChanged += (s, token) =>
        //{
        //    Console.WriteLine($"Token FCM actualizado: {token}");
        //    // Envía este token a tu backend aquí
        //};

    }
}