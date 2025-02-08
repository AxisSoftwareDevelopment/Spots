using Plugin.Firebase.Firestore;
using Spots.Database;
using Spots.Notifications;
using Spots.ResourceManager;
using Spots.Utilities;

namespace Spots.Models
{
    public static class Notification
    {
        public const string NOTIFICATION_TYPE_TABLEINVITE = "TABLE_INVITE";
    }

    public class Notification_TableInvite : INotification
    {
        public string NotificationID { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public string Type { get; set; }
        public string OwnerID { get; set; }
        public IList<string> Metadata { get; set; } = ["", "", "", ""];
        public string Metadata_TableID
        {
            get => Metadata[0];
            set => Metadata[0] = value;
        }

        public string Metadata_SenderName
        {
            get => Metadata[1];
            set => Metadata[1] = value;
        }

        public string Metadata_TableName
        {
            get => Metadata[2];
            set => Metadata[2] = value;
        }

        public string Metadata_ReceiverName
        {
            get => Metadata[3];
            set => Metadata[3] = value;
        }

        public string Title => GetTitle();

        public string Body => GetBody();

        public Notification_TableInvite(string notificationID, DateTimeOffset creationDate, string type, string ownerID, string tableID, string senderName, string tableName, string receiverName)
        {
            NotificationID = notificationID;
            CreationDate = creationDate;
            Type = type;
            OwnerID = ownerID;
            Metadata_TableID = tableID;
            Metadata_SenderName = senderName;
            Metadata_TableName = tableName;
            Metadata_ReceiverName = receiverName;
        }

        public Notification_TableInvite(INotification notification)
        {
            NotificationID = notification.NotificationID;
            CreationDate = notification.CreationDate;
            Type = notification.Type;
            OwnerID = notification.OwnerID;
            Metadata_TableID = notification.Metadata.Count >= 1 ? notification.Metadata[0] : "";
            Metadata_SenderName = notification.Metadata.Count >= 2 ? notification.Metadata[1] : "";
            Metadata_TableName = notification.Metadata.Count >= 3 ? notification.Metadata[2] : "";
            Metadata_ReceiverName = notification.Metadata.Count >= 4 ? notification.Metadata[3] : "";
        }

        public async Task<bool> OnClickedEvent()
        {
            bool retVal = true;

            // Action
            string[] popUpLables = ["lbl_TableInvitation", "txt_TableInvitationAcceptanceConfirmation", "lbl_Ok", "lbl_Decline"];
            popUpLables = ResourceManagement.GetStringResources(Application.Current?.Resources, popUpLables);
            string formatedText = string.Format(popUpLables[1], Metadata_SenderName, Metadata_TableName);
            if (await UserInterface.DisplayPopPup_Choice(popUpLables[0], formatedText, popUpLables[2], popUpLables[3]))
            {
                await DatabaseManager.Transaction_AddTableMember(OwnerID, Metadata_TableID);
            }
            // Remove itself from DB
            await DatabaseManager.DeleteNotificationData(this);
            await NotificationsManager.Handler.UpdateNotifications();

            return retVal;
        }

        private string GetTitle()
        {
            string[] titleLable = ["lbl_UserInvitedYouToTable"];
            titleLable = ResourceManagement.GetStringResources(Application.Current?.Resources, titleLable);
            string formatedTitle = string.Format(titleLable[0], Metadata_SenderName);
            return formatedTitle;
        }

        private string GetBody()
        {
            string[] bodyLable = ["txt_UserInvitedYouToTable"];
            bodyLable = ResourceManagement.GetStringResources(Application.Current?.Resources, bodyLable);
            string formatedBody = string.Format(bodyLable[0], [Metadata_SenderName, Metadata_TableName]);
            return formatedBody;
        }
    }

    public interface INotification
    {
        public string NotificationID { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public string Type { get; set; }
        public string OwnerID { get; set; }
        public IList<string> Metadata { get; set; }
        public string Title { get; }
        public string Body { get; }

        //In this method, The notification should take action and remove itself from the DB if appropiate.
        public Task<bool> OnClickedEvent();
    }

    public class Notification_Firebase : INotification
    {
        [FirestoreDocumentId]
        public string NotificationID { get; set; }

        [FirestoreProperty(nameof(CreationDate))]
        public DateTimeOffset CreationDate { get; set; }

        [FirestoreProperty(nameof(Type))]
        public string Type { get; set; }

        [FirestoreProperty(nameof(OwnerID))]
        public string OwnerID { get; set; }

        [FirestoreProperty(nameof(Metadata))]
        public IList<string> Metadata { get; set; }

        public string Title => "";

        public string Body => "";

        public Notification_Firebase()
        {
            NotificationID = "";
            CreationDate = DateTimeOffset.Now;
            Type = "";
            OwnerID = "";
            Metadata = [];
        }

        public Notification_Firebase(INotification notification)
        {
            NotificationID = notification.NotificationID;
            CreationDate = notification.CreationDate;
            Type = notification.Type;
            OwnerID = notification.OwnerID;
            Metadata = notification.Metadata;
        }

        public Task<bool> OnClickedEvent()
        {
            throw new NotImplementedException();
        }
    }
}
