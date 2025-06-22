using MiMototaxxi.ViewModel.Registre;

namespace MiMototaxxi.View.Registre;

public partial class RegistrePage : ContentPage
{
    #region Vars
    public readonly RegistreViewModel ViewModel;
    #endregion
    public RegistrePage(string typeUser)
	{
		InitializeComponent();
        BindingContext = ViewModel = new RegistreViewModel(typeUser);
    }
}