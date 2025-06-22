using MiMototaxxi.ViewModel.Mototaxxi;

namespace MiMototaxxi.View.Mototaxxi;

public partial class MotoTaxiLoginPage : ContentPage
{
    #region Vars
    public readonly MotoTaxiLoginViewModel ViewModel;
    #endregion
    public MotoTaxiLoginPage()
	{
		InitializeComponent();
        BindingContext = ViewModel = new MotoTaxiLoginViewModel();
    }
}