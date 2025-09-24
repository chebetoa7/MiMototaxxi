using Microsoft.Maui.Controls;
using Maui.GoogleMaps;
using MiMototaxxi.ViewModel.HomeMototaxi;
using Plugin.Firebase.CloudMessaging;
using MiMototaxxi.ViewModel;
using MiMototaxxi.Services.Moto;

namespace MiMototaxxi;

public partial class HomeMotoPageR : ContentPage
{
    public HomeMotoPageRViewModel ViewModel { get; }

        public HomeMotoPageR( HomeMotoPageRViewModel viewModel )
        {
            InitializeComponent();
             BindingContext = ViewModel = viewModel;

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            this.Appearing += async (s, e) => await ViewModel.InitializeAsync();
            
            CrossFirebaseCloudMessaging.Current.NotificationReceived += (s, e) =>
            {
                ViewModel.ProcessNotification(e.Notification.Data);
            };

            ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.CurrentPosition) ||
                    e.PropertyName == nameof(ViewModel.UserPin) ||
                    e.PropertyName == nameof(ViewModel.FixedPin))
                {
                    UpdateMap();
                }
            };
        }

        private void UpdateMap()
        {
            if (ViewModel.CurrentPosition == null) return;

            MyMap.Pins.Clear();
            MyMap.Pins.Add(ViewModel.UserPin);

            if (ViewModel.FixedPin != null)
            {
                MyMap.Pins.Add(ViewModel.FixedPin);
            }

            MyMap.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                    ViewModel.CurrentPosition,
                    Distance.FromKilometers(0.3)),
                animate: true);
        }

        private void StartJornadaTapped(object sender, TappedEventArgs e)
            => ViewModel.StartJornadaCommand.Execute(null);

        private void StopJornadaClicked(object sender, EventArgs e)
            => ViewModel.StopJornadaCommand.Execute(null);
}