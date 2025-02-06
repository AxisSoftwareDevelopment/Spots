using Spots.CloudMessaging;

namespace Spots.Notifications
{
    public static class NotificationsManager
    {
        public static Task SendTableInvitation(string FCMToken, string Sender, string TableName)
        {
            return CloudMessagingManager.TriggerNotificationViaTokensAsync([FCMToken], $"Table Invitation", $"{Sender} invites you to join the {TableName} table.");
        }
    }
}
