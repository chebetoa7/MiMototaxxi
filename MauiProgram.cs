
using Microsoft.Extensions.Logging;
using Maui.GoogleMaps;
using MiMototaxxi.Services;
using MiMototaxxi.View.HomeMototaxiPage;
using MiMototaxxi.View.HomePasajeroPage;
using MiMototaxxi.View.Mototaxxi;
using MiMototaxxi.View.Pasajero;
using MiMototaxxi.View.Registre;
using MiMototaxxi.View.UserSelection;
using MiMototaxxi.ViewModel.HomeMototaxi;
using MiMototaxxi.ViewModel.HomePasajero;
using MiMototaxxi.ViewModel.Mototaxxi;
using MiMototaxxi.ViewModel.Pasajero;
using MiMototaxxi.ViewModel.Registre;
using MiMototaxxi.ViewModel.UserSelection;
using Maui.GoogleMaps.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Core;
using Plugin.Firebase.CloudMessaging;
using MiMototaxxi.Services.Moto;
using MiMototaxxi.Services.Location;
using MiMototaxxi.ViewModel;
using MiMototaxxi.Services.LocationTracking;
using MiMototaxxi.Services.Notification;







#if IOS
using Plugin.Firebase.Core.Platforms.iOS;
#elif ANDROID
using Plugin.Firebase.Core.Platforms.Android;
#endif


namespace MiMototaxxi;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
        builder
			.UseMauiApp<App>()
            //.RegisterFirebaseServices()
            #if ANDROID
            .UseGoogleMaps()
            #elif IOS
                   // builder.UseGoogleMaps(Variables.GOOGLE_MAPS_IOS_API_KEY);
            #endif
            .UseMauiMaps()
           
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
        //Servicios
        builder.Services.AddSingleton<UsuarioService>();
        builder.Services.AddSingleton<MotoService>();
        builder.Services.AddSingleton<ApiServiceMoto>();

        builder.Services.AddSingleton<IMotoService, MotoService>();
        builder.Services.AddSingleton<ILocationTrackerService, LocationTrackerService>();
        builder.Services.AddSingleton<IDestinationMonitorService, DestinationMonitorService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();

        //ViewModels
        builder.Services.AddTransient<UserSelectionViewModel>();
        builder.Services.AddTransient<MotoTaxiLoginViewModel>();
        builder.Services.AddTransient<PasajeroLoginViewModel>();
        builder.Services.AddTransient<RegistreViewModel>();
        builder.Services.AddTransient<RegistreMototaxiViewModel>();
        builder.Services.AddTransient<HomeMotoViewModel>();
        builder.Services.AddTransient<HomePasajeViewModel>();

        builder.Services.AddTransient<HomeMotoPageRViewModel>();
        builder.Services.AddTransient<HomeMotoPageR>();


        //Views
        builder.Services.AddTransient<MotoTaxiLoginPage>();
        builder.Services.AddTransient<PasajeroLoginPage>();
        builder.Services.AddTransient<UserSelectionPage>();

        builder.Services.AddTransient<RegistreMototaxiPage>();
        builder.Services.AddTransient<RegistrePage>();
		builder.Services.AddTransient<HomeMotoPage>();
		builder.Services.AddTransient<HomePasajePage>();


#if DEBUG
        builder.Logging.AddDebug();
#endif


		return builder.Build();
	}
    private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events => {
#if IOS
            events.AddiOS(iOS => iOS.WillFinishLaunching((_,__) => {
                CrossFirebase.Initialize();
                return false;
            }));
#elif ANDROID
            events.AddAndroid(android => android.OnCreate((activity, _) =>
                CrossFirebase.Initialize(activity)));
#endif
        });
        
        return builder;
    }

}
