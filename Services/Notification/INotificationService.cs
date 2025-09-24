namespace MiMototaxxi.Services.Notification
{
    public interface INotificationService
    {
        Task<string> GetTokenAsync();
        void SubscribeToTopics(string[] topics);
        void UnsubscribeFromTopics(string[] topics);
    }
}
