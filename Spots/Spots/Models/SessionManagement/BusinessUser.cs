using Plugin.Firebase.Firestore;
using System.ComponentModel;

namespace Spots.Models.SessionManagement
{
    public class BusinessUser : BindableObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
        private string _location;
        #endregion

        #region Public Parameters
        public bool userDataRetrieved = false;
        public ImageSource profilePictureSource
        {
            get => _profilePictureAddress.Equals("null") ?
                ImageSource.FromFile("placeholder_logo.jpg") : _profilePictureSource;
            set
            {
                _profilePictureSource = value ?? null;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(profilePictureSource)));
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
        public string userID
        {
            get => _userID;
            set
            {
                _userID = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(userID)));
            }
        }
        [FirestoreProperty(nameof(brandName))]
        public string brandName
        {
            get => _brandName;
            set
            {
                _brandName = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(brandName)));
            }
        }
        [FirestoreProperty(nameof(businessName))]
        public string businessName
        {
            get => _businessName;
            set
            {
                _businessName = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(businessName)));
            }
        }
        [FirestoreProperty(nameof(email))]
        public string email
        {
            get => _email;
            set
            {
                _email = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(email)));
            }
        }
        [FirestoreProperty(nameof(phoneNumber))]
        public string phoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value ?? "null";
                if (value?.Length == 10)
                    fullPhoneNumber = $"+({_phoneCountryCode}) {value?.Substring(0, 3)} {value?.Substring(3, 3)} {value?.Substring(6, 4)}";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(phoneNumber)));
            }
        }
        [FirestoreProperty(nameof(phoneCountryCode))]
        public string phoneCountryCode
        {
            get => _phoneCountryCode;
            set
            {
                _phoneCountryCode = value ?? "null";
                if (_phoneNumber?.Length == 10)
                    fullPhoneNumber = $"+({value}) {_phoneNumber.Substring(0, 3)} {_phoneNumber.Substring(3, 3)} {_phoneNumber.Substring(6, 4)}";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(phoneCountryCode)));
            }
        }
        [FirestoreProperty(nameof(description))]
        public string description
        {
            get => _description;
            set
            {
                _description = value ?? "null";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(description)));
            }
        }
        [FirestoreProperty(nameof(location))]
        public string location
        {
            get => _location;
            set
            {
                _location = value ?? "null";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(location)));
            }
        }
        [FirestoreProperty(nameof(profilePictureAddress))]
        public string profilePictureAddress
        {
            get => _profilePictureAddress;
            set
            {
                _profilePictureAddress = value ?? "null";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(profilePictureAddress)));
            }
        }
        #endregion

        public BusinessUser()
        {
            userID = "";
            brandName = "";
            businessName = "";
            email = "";
            profilePictureAddress = "null";
            phoneCountryCode = "";
            phoneNumber = "";
            description = "";
            location = "";
        }

        public BusinessUser(string UserID, string BrandName, string BusinessName, string Email, string ProfilePictureAddress = "", ImageSource ProfilePictureSource = null,
            string PhoneNumber = "", string PhoneCountryCode = "", string Description = "", string bLocation = "")
        {
            userID = UserID;
            brandName = BrandName;
            businessName = BusinessName;
            email = Email;
            profilePictureAddress = ProfilePictureAddress;
            profilePictureSource = ProfilePictureSource;
            phoneNumber = PhoneNumber;
            phoneCountryCode = PhoneCountryCode;
            description = Description;
            location = bLocation;
        }

        public void UpdateUserData(BusinessUser userData)
        {
            userID = userData.userID;
            brandName = userData.brandName;
            businessName = userData.businessName;
            email = userData.email;
            profilePictureAddress = userData.profilePictureAddress;
            profilePictureSource = userData.profilePictureSource;
            phoneNumber = userData.phoneNumber;
            phoneCountryCode = userData.phoneCountryCode;
            description = userData.description;
            location = userData.location;
        }
    }
}
