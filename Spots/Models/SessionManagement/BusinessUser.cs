using Plugin.Firebase.Firestore;
using System.ComponentModel;
using Spots.Models.DatabaseManagement;

namespace Spots.Models.SessionManagement
{
    public class BusinessUser : BindableObject, INotifyPropertyChanged
    {
        new public event PropertyChangedEventHandler PropertyChanged;

        #region Private Parameters
        private string _fullPhoneNumber;
        private string _brandName;
        private string _businessName;
        private string _userID;
        private string _email;
        private string _profilePictureAddress;
        private ImageSource _profilePictureSource;
        private string _phoneNumber;
        private string _phoneCountryCode;
        private string _description;
        private FirebaseLocation _location;
        private List<string> _praises;
        #endregion

        #region Public Parameters
        public bool userDataRetrieved = false;
        public ImageSource ProfilePictureSource
        {
            get => _profilePictureAddress.Equals("null") ?
                ImageSource.FromFile("placeholder_logo.jpg") : _profilePictureSource;
            set
            {
                _profilePictureSource = value ?? null;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfilePictureSource)));
            }
        }
        public string fullPhoneNumber
        {
            get => _fullPhoneNumber?.Length > 0 ? _fullPhoneNumber : "+ -- --- --- ----";
            private set
            {
                _fullPhoneNumber = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(fullPhoneNumber)));
            }
        }
        [FirestoreDocumentId]
        public string UserID
        {
            get => _userID;
            set
            {
                _userID = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserID)));
            }
        }
        [FirestoreProperty(nameof(BrandName))]
        public string BrandName
        {
            get => _brandName;
            set
            {
                _brandName = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BrandName)));
            }
        }
        [FirestoreProperty(nameof(BusinessName))]
        public string BusinessName
        {
            get => _businessName;
            set
            {
                _businessName = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BusinessName)));
            }
        }
        [FirestoreProperty(nameof(Email))]
        public string Email
        {
            get => _email;
            set
            {
                _email = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
            }
        }
        [FirestoreProperty(nameof(PhoneNumber))]
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value ?? "null";
                if (value?.Length == 10)
                    fullPhoneNumber = $"+({_phoneCountryCode}) {value?.Substring(0, 3)} {value?.Substring(3, 3)} {value?.Substring(6, 4)}";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneNumber)));
            }
        }
        [FirestoreProperty(nameof(PhoneCountryCode))]
        public string PhoneCountryCode
        {
            get => _phoneCountryCode;
            set
            {
                _phoneCountryCode = value ?? "null";
                if (_phoneNumber?.Length == 10)
                    fullPhoneNumber = $"+({value}) {_phoneNumber.Substring(0, 3)} {_phoneNumber.Substring(3, 3)} {_phoneNumber.Substring(6, 4)}";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneCountryCode)));
            }
        }
        [FirestoreProperty(nameof(Description))]
        public string Description
        {
            get => _description;
            set
            {
                _description = value ?? "null";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
            }
        }
        [FirestoreProperty(nameof(Location))]
        public FirebaseLocation Location
        {
            get => _location;
            set
            {
                _location = value ?? new FirebaseLocation();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Location)));
            }
        }
        [FirestoreProperty(nameof(ProfilePictureAddress))]
        public string ProfilePictureAddress
        {
            get => _profilePictureAddress;
            set
            {
                _profilePictureAddress = value ?? "null";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfilePictureAddress)));
            }
        }

        public List<string> Praises
        {
            get => _praises;
            set
            {
                _praises = value ?? new List<string>();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Praises)));
            }
        }
        #endregion

        public BusinessUser()
        {
            UserID = "";
            BrandName = "";
            BusinessName = "";
            Email = "";
            ProfilePictureAddress = "null";
            PhoneCountryCode = "";
            PhoneNumber = "";
            Description = "";
            Location = new FirebaseLocation();
            Praises = new List<string>();
        }

        public BusinessUser(string userID, string brandName, string businessName, string email, string profilePictureAddress = "", ImageSource profilePictureSource = null,
            string phoneNumber = "", string phoneCountryCode = "", string description = "", FirebaseLocation location = null, List<string> praises = null)
        {
            UserID = userID;
            BrandName = brandName;
            BusinessName = businessName;
            Email = email;
            ProfilePictureAddress = profilePictureAddress;
            ProfilePictureSource = profilePictureSource;
            PhoneNumber = phoneNumber;
            PhoneCountryCode = phoneCountryCode;
            Description = description;
            Location = location ?? new FirebaseLocation();
            Praises = praises ?? new List<string>();

        }

        public void UpdateUserData(BusinessUser userData)
        {
            UserID = userData.UserID;
            BrandName = userData.BrandName;
            BusinessName = userData.BusinessName;
            Email = userData.Email;
            ProfilePictureAddress = userData.ProfilePictureAddress;
            ProfilePictureSource = userData.ProfilePictureSource;
            PhoneNumber = userData.PhoneNumber;
            PhoneCountryCode = userData.PhoneCountryCode;
            Description = userData.Description;
            Location = userData.Location;
            Praises = userData.Praises;
        }
    }

    public class FirebaseLocation : IFirestoreObject
    {
        [FirestoreProperty(nameof(Address))]
        public string Address { get; set; }

        [FirestoreProperty(nameof(Latitude))]
        public double Latitude { get; set; }

        [FirestoreProperty(nameof(Longitude))]
        public double Longitude { get; set; }

        public FirebaseLocation()
        {
            Address = "";
            Latitude = 0;
            Longitude = 0;
        }

        public FirebaseLocation(string addr, double lat, double lng)
        {
            Address = addr;
            Latitude = lat;
            Longitude = lng;
        }
    }
}
