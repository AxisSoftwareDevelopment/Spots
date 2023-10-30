using Plugin.Firebase.Firestore;
using Spots.Models.DatabaseManagement;
using Spots.Models.ResourceManagement;
using System.ComponentModel;

namespace Spots.Models.SessionManagement
{
    public class User : BindableObject, INotifyPropertyChanged
    {
        new public event PropertyChangedEventHandler PropertyChanged;

        #region Private Parameters
        private string _fullName;
        private string _fullPhoneNumber;
        private string _userID;
        private string _firstName;
        private string _lastName;
        private DateTimeOffset _birthDate;
        private string _email;
        private string _profilePictureAddress;
        private string _phoneNumber;
        private string _phoneCountryCode;
        private string _description;
        private ImageSource _profilePictureSource;
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
        public string fullName
        {
            get => _fullName; 
            private set
            {
                _fullName = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(fullName)));
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
        [FirestoreProperty(nameof(firstName))]
        public string firstName
        {
            get => _firstName;
            set
            {
                _firstName = value ?? "";
                fullName = $"{value} {lastName}";
            }
        }
        [FirestoreProperty(nameof(lastName))]
        public string lastName
        {
            get => _lastName; 
            set
            {
                _lastName = value ?? "";
                fullName = $"{firstName} {value}";
            }
        }
        [FirestoreProperty(nameof(birthDate))]
        public DateTimeOffset birthDate
        {
            get => _birthDate;
            set
            {
                _birthDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(birthDate)));
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
                if(_phoneNumber?.Length == 10)
                    fullPhoneNumber = $"+({value}) {_phoneNumber.Substring(0,3)} {_phoneNumber.Substring(3,3)} {_phoneNumber.Substring(6, 4)}";
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
        public User()
        {
            userID = "";
            firstName = "";
            lastName = "";
            birthDate = DateTimeOffset.Now;
            email = "";
            profilePictureAddress = "null";
            phoneCountryCode = "";
            phoneNumber = "";
            description = "";
        }

        public User(string UserID, string FirstName, string LastName, DateTimeOffset BirthDate, string Email, 
            string ProfilePictureAddr = "", ImageSource ProfilePictureSrc = null, string PhoneNumber = "", string PhoneCountryCode = "", string Description = "")
        {
            userID = UserID;
            firstName = FirstName;
            lastName = LastName;
            birthDate = BirthDate;
            email = Email;
            profilePictureAddress = ProfilePictureAddr;
            profilePictureSource = profilePictureAddress.Equals("null") ? 
                ImageSource.FromFile("placeholder_logo.jpg") : ProfilePictureSrc;
            phoneNumber = PhoneNumber;
            phoneCountryCode = PhoneCountryCode;
            description = Description;
        }

        public void UpdateUserData(User userData)
        {
            userID = userData.userID;
            firstName = userData.firstName;
            lastName = userData.lastName;
            birthDate = userData.birthDate;
            email = userData.email;
            profilePictureAddress = userData.profilePictureAddress;
            profilePictureSource = userData.profilePictureSource;
            phoneNumber = userData.phoneNumber;
            phoneCountryCode = userData.phoneCountryCode;
            description = userData.description;
        }

        public async void UpdateProfilePicture(string address)
        {
            profilePictureAddress = address;
            if(!profilePictureAddress.Equals("null"))
            {
                Uri imageUri = new( await DatabaseManager.GetImageDownloadLink(profilePictureAddress) );

                profilePictureSource = ImageSource.FromUri(imageUri);
            }
        }
    }
}
