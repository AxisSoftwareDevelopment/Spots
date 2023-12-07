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
        private string _sFullName;
        private string _sFullPhoneNumber;
        private string _sUserID;
        private string _sFirstName;
        private string _sLastName;
        private DateTimeOffset _dtBirthDate;
        private string _sEmail;
        private string _sProfilePictureAddress;
        private string _sPhoneNumber;
        private string _sPhoneCountryCode;
        private string _sDescription;
        private ImageSource _imProfilePictureSource;
        #endregion

        #region Public Parameters
        public bool bUserDataRetrieved = false;
        public ImageSource imProfilePictureSource
        {
            get => _sProfilePictureAddress.Equals("null") ?
                ImageSource.FromFile("placeholder_logo.jpg") : _imProfilePictureSource;
            set
            {
                _imProfilePictureSource = value ?? null;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(imProfilePictureSource)));
            }
        }
        public string sFullPhoneNumber
        {
            get => _sFullPhoneNumber?.Length > 0 ? _sFullPhoneNumber : "+ -- --- --- ----";
            private set
            {
                _sFullPhoneNumber = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(sFullPhoneNumber)));
            }
        }
        public string sFullName
        {
            get => _sFullName; 
            private set
            {
                _sFullName = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(sFullName)));
            }
        }
        [FirestoreDocumentId]
        public string sUserID
        {
            get => _sUserID;
            set
            {
                _sUserID = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(sUserID)));
            }
        }
        [FirestoreProperty(nameof(sFirstName))]
        public string sFirstName
        {
            get => _sFirstName;
            set
            {
                _sFirstName = value ?? "";
                sFullName = $"{value} {sLastName}";
            }
        }
        [FirestoreProperty(nameof(sLastName))]
        public string sLastName
        {
            get => _sLastName; 
            set
            {
                _sLastName = value ?? "";
                sFullName = $"{sFirstName} {value}";
            }
        }
        [FirestoreProperty(nameof(dtBirthDate))]
        public DateTimeOffset dtBirthDate
        {
            get => _dtBirthDate;
            set
            {
                _dtBirthDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(dtBirthDate)));
            }
        }
        [FirestoreProperty(nameof(sEmail))]
        public string sEmail
        {
            get => _sEmail;
            set
            {
                _sEmail = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(sEmail)));
            }
        }
        [FirestoreProperty(nameof(sPhoneNumber))]
        public string sPhoneNumber
        {
            get => _sPhoneNumber;
            set
            {
                _sPhoneNumber = value ?? "null";
                if (value?.Length == 10)
                    sFullPhoneNumber = $"+({_sPhoneCountryCode}) {value?.Substring(0, 3)} {value?.Substring(3, 3)} {value?.Substring(6, 4)}";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(sPhoneNumber)));
            }
        }
        [FirestoreProperty(nameof(sPhoneCountryCode))]
        public string sPhoneCountryCode
        {
            get => _sPhoneCountryCode;
            set
            {
                _sPhoneCountryCode = value ?? "null";
                if(_sPhoneNumber?.Length == 10)
                    sFullPhoneNumber = $"+({value}) {_sPhoneNumber.Substring(0,3)} {_sPhoneNumber.Substring(3,3)} {_sPhoneNumber.Substring(6, 4)}";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(sPhoneCountryCode)));
            }
        }
        [FirestoreProperty(nameof(sDescription))]
        public string sDescription
        {
            get => _sDescription;
            set
            {
                _sDescription = value ?? "null";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(sDescription)));
            }
        }
        [FirestoreProperty(nameof(sProfilePictureAddress))]
        public string sProfilePictureAddress
        {
            get => _sProfilePictureAddress;
            set
            {
                _sProfilePictureAddress = value ?? "null";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(sProfilePictureAddress)));
            }
        }
        #endregion
        public User()
        {
            sUserID = "";
            sFirstName = "";
            sLastName = "";
            dtBirthDate = DateTimeOffset.Now;
            sEmail = "";
            sProfilePictureAddress = "null";
            sPhoneCountryCode = "";
            sPhoneNumber = "";
            sDescription = "";
        }

        public User(string UserID, string FirstName, string LastName, DateTimeOffset BirthDate, string Email, 
            string ProfilePictureAddr = "", ImageSource ProfilePictureSrc = null, string PhoneNumber = "", string PhoneCountryCode = "", string Description = "")
        {
            sUserID = UserID;
            sFirstName = FirstName;
            sLastName = LastName;
            dtBirthDate = BirthDate;
            sEmail = Email;
            sProfilePictureAddress = ProfilePictureAddr;
            imProfilePictureSource = sProfilePictureAddress.Equals("null") ? 
                ImageSource.FromFile("placeholder_logo.jpg") : ProfilePictureSrc;
            sPhoneNumber = PhoneNumber;
            sPhoneCountryCode = PhoneCountryCode;
            sDescription = Description;
        }

        public void UpdateUserData(User userData)
        {
            sUserID = userData.sUserID;
            sFirstName = userData.sFirstName;
            sLastName = userData.sLastName;
            dtBirthDate = userData.dtBirthDate;
            sEmail = userData.sEmail;
            sProfilePictureAddress = userData.sProfilePictureAddress;
            imProfilePictureSource = userData.imProfilePictureSource;
            sPhoneNumber = userData.sPhoneNumber;
            sPhoneCountryCode = userData.sPhoneCountryCode;
            sDescription = userData.sDescription;
        }

        public async void UpdateProfilePicture(string address)
        {
            sProfilePictureAddress = address;
            if(!sProfilePictureAddress.Equals("null"))
            {
                Uri imageUri = new( await DatabaseManager.GetImageDownloadLink(sProfilePictureAddress) );

                imProfilePictureSource = ImageSource.FromUri(imageUri);
            }
        }
    }
}
