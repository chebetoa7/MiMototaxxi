using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace MiMototaxxi;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Configuración para mejor precisión de ubicación
        var locationManager = (LocationManager)GetSystemService(LocationService);
        if (!locationManager.IsProviderEnabled(LocationManager.GpsProvider))
        {
            StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
        }

        // Configuración de precisión de ubicación

        // Solicitar permisos de ubicación en primer plano
        if (ContextCompat.CheckSelfPermission(this,
            Android.Manifest.Permission.AccessFineLocation) != Permission.Granted)
        {
            ActivityCompat.RequestPermissions(this,
                new[] { Android.Manifest.Permission.AccessFineLocation }, 1);
        }

        //Permiso para firebase messaging
        Firebase.FirebaseApp.InitializeApp(this);
    }
}

