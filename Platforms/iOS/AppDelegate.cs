using Foundation;
using Maui;

namespace MiMototaxxi;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	//protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    protected override MauiApp CreateMauiApp()
    {
        // Inicializa Google Maps con tu API Key (¡Reemplaza "TU_API_KEY_IOS"!)
        MauiGoogleMaps.Init("AIzaSyBumjBZeGegoQraomAZjiGklIFiSJrPztQ"); 

        return MauiProgram.CreateMauiApp();
    }

}
