using Android.App;
using Android.Runtime;
using Plugin.Firebase.CloudMessaging; // Añade este using

namespace MiMototaxxi;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	public override void OnCreate()
	{
		base.OnCreate();
		// Solución definitiva para carga de clases
		try {
			var firebaseApp = Firebase.FirebaseApp.InitializeApp(this);
			
			// Carga explícita de la clase (SOLUCIÓN CLAVE)
			//Java.Lang.Class.ForName("plugin.firebase.cloudmessaging.PlatformFirebaseMessagingService");
			
			Console.WriteLine($"Firebase inicializado: {firebaseApp.Name}");
		} catch (Exception ex) {
			Console.WriteLine($"Error inicializando Firebase: {ex}");
		}
		/*var firebase = Firebase.FirebaseApp.InitializeApp(this);
        Console.WriteLine($"Firebase initialized: {firebase?.Name ?? "FAILED"}");*/
    }

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
