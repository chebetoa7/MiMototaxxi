using Plugin.Firebase.CloudMessaging;
using System.Diagnostics;

namespace MiMototaxxi.Services.Notification
{
    public class NotificationService : INotificationService
    {
        public async Task<string> GetTokenAsync()
        {
            try
            {
                return await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo token FCM: {ex.Message}");
                return string.Empty;
            }
        }

        public async void SubscribeToTopics(string[] topics)
        {
            try
            {
                foreach (var topic in topics)
                {
                    await CrossFirebaseCloudMessaging.Current.SubscribeToTopicAsync(topic);
                    Debug.WriteLine($"Suscrito al topic: {topic}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error suscribiendo a topics: {ex.Message}");
            }
        }

        public async void UnsubscribeFromTopics(string[] topics)
        {
            try
            {
                foreach (var topic in topics)
                {
                    await CrossFirebaseCloudMessaging.Current.UnsubscribeFromTopicAsync(topic);
                    Console.WriteLine($"Desuscrito del topic: {topic}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error desuscribiendo de topics: {ex.Message}");
            }
        }
    }
}
