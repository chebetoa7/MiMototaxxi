using CommunityToolkit.Mvvm.ComponentModel;
using MiMototaxxi.View.HomePasajeroPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiMototaxxi.ViewModel.Pasajero
{
    public partial class PasajeroLoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool _isVisible;

        #region Command
        public ICommand InvitadoCommand { get; set; }
        #endregion
        public PasajeroLoginViewModel() 
        {
            InvitadoCommand = new Command(async () => await FuncitonInvitadoCommand());
        }

        private async Task FuncitonInvitadoCommand()
        {
            try 
            {
                var page = new HomePasajePage();
                await NavigateAsync(page);
            }
            catch(Exception ex) 
            {
                Console.WriteLine("error: FunctionInviatadoCommand: " + ex.Message);
            }
        }
    }
}
