using Plugin.Firebase.Firestore;

using Spots.Database;

namespace Spots.Models
{
    public class Table
    {
        public List<string> SittingMembers {  get; set; }
        public string TableID { get; set; }
        public ImageSource TablePictureSource { get; set; }
        public string TableName { get; set; }
        public string Description { get; set; }
        public List<string> TableMembers { get; set; }
        public FirebaseLocation Location { get; set; }
        public int OnlineCount
        {
            get
            {
                return SittingMembers.Count;
            }
        }

        public string TableStateColor
        {
            get
            {
                return OnlineCount > 0 ? "Green" : "Red";
            }
        }

        public Table()
        {
            TableID = "";
            SittingMembers = [];
            TablePictureSource = ImageSource.FromFile("placeholder_logo.jpg");
            TableName = "";
            Description = "";
            TableMembers = [];
            Location = new();
        }

        public Table(Table_Firebase table, ImageSource tablePictureSource)
        {
            TableID = table.TableID;
            SittingMembers = [.. table.OnlineMembers];
            TablePictureSource = tablePictureSource;
            TableName = table.TableName;
            Description = table.Description;
            TableMembers = [.. table.TableMembers];
            Location = table.Location;
        }

        public async Task UpdateTablePicture(string address)
        {
            if (address.Length > 0)
            {
                string downloadAddress = await DatabaseManager.GetImageDownloadLink(address);
                Uri imageUri = new(downloadAddress);

                TablePictureSource = ImageSource.FromUri(imageUri);
            }
            else
            {
                ImageSource.FromFile("placeholder_logo.jpg");
            }
        }
    }

    public class Table_Firebase
    {
        [FirestoreDocumentId]
        public string TableID { get; set; }

        [FirestoreProperty(nameof(OnlineMembers))]
        public IList<string> OnlineMembers { get; set; }

        [FirestoreProperty(nameof(TablePictureAddress))]
        public string TablePictureAddress { get; set; }

        [FirestoreProperty(nameof(TableName))]
        public string TableName { get; set; }

        [FirestoreProperty(nameof(Description))]
        public string Description { get; set; }

        [FirestoreProperty(nameof(TableMembers))]
        public IList<string> TableMembers { get; set; }

        [FirestoreProperty(nameof(Location))]
        public FirebaseLocation Location { get; set; }

        public Table_Firebase()
        {
            TableID = string.Empty;
            OnlineMembers = [];
            TablePictureAddress = string.Empty;
            TableName = string.Empty;
            Description = string.Empty;
            TableMembers = [];
            Location = new();
        }

        public Table_Firebase(Table table, string tablePictureAddress)
        {
            TableID = table.TableID;
            OnlineMembers = table.SittingMembers;
            TablePictureAddress = tablePictureAddress;
            TableName = table.TableName;
            Description = table.Description;
            TableMembers = table.TableMembers;
            Location = table.Location;
        }

        public async Task<ImageSource> GetImageSource()
        {
            if (TablePictureAddress.Length > 0)
            {
                string downloadAddress = await DatabaseManager.GetImageDownloadLink(TablePictureAddress);
                Uri imageUri = new(downloadAddress);

                return ImageSource.FromUri(imageUri);
            }
            else
            {
                return ImageSource.FromFile("placeholder_logo.jpg");
            }
        }
    }
}
