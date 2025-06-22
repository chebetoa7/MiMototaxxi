using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMototaxxi.ViewModel
{
    public partial class BaseViewModel : ObservableObject
    {
        public async Task<bool> NavigateAsync(Page NextPage)
        {
            await Shell.Current.Navigation.PushAsync(NextPage);
            return true;
        }
        public async Task<bool> CloseNavigationAsync()
        {
            await Shell.Current.Navigation.PopAsync();
            return true;
        }

    }
}
