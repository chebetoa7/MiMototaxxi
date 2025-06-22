using MiMototaxxi.ViewModel.Pasajero;

namespace MiMototaxxi.View.Pasajero;

public partial class PasajeroLoginPage : ContentPage
{
    #region Vars
    public readonly PasajeroLoginViewModel ViewModel;
    #endregion
    public PasajeroLoginPage()
	{
		InitializeComponent();
        BindingContext = ViewModel = new PasajeroLoginViewModel();
    }
   
    
}