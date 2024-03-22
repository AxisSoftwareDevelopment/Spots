using Plugin.Firebase.Firestore;
using Spots;
using System.ComponentModel;

namespace Spots;
public class User : BindableObject, INotifyPropertyChanged
{
    new public event PropertyChangedEventHandler? PropertyChanged;

    #region Private Parameters
    private string? _UserID;
    private string? _FirstName;
    private string? _LastName;
    private DateTimeOffset _BirthDate;
    private string? _Email;
    private string? _ProfilePictureAddress;
    private string? _PhoneNumber;
    private string? _PhoneCountryCode;
    private string? _Description;
    private ImageSource? _ProfilePictureSource;
    #endregion

    #region Public Parameters
    public bool bUserDataRetrieved = false;
    public ImageSource ProfilePictureSource
    {
        get => _ProfilePictureSource ?? ImageSource.FromFile("placeholder_logo.jpg");
        set
        {
            _ProfilePictureSource = value ?? ImageSource.FromFile("placeholder_logo.jpg");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfilePictureSource)));
        }
    }
    public string FullPhoneNumber
    {
        get => (PhoneNumber?.Length < 10 || PhoneCountryCode?.Length < 2) ? "+ -- --- --- ----" : $"+({_PhoneCountryCode}) {PhoneNumber?[..3]} {PhoneNumber?.Substring(3, 3)} {PhoneNumber?.Substring(6, 4)}";
    }
    public string FullName
    {
        get => $"{FirstName} {LastName}"; 
    }
    [FirestoreDocumentId]
    public string UserID
    {
        get => _UserID ?? "";
        set
        {
            _UserID = value ?? "";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserID)));
        }
    }
    [FirestoreProperty(nameof(FirstName))]
    public string FirstName
    {
        get => _FirstName ?? "";
        set
        {
            _FirstName = value ?? "";
        }
    }
    [FirestoreProperty(nameof(LastName))]
    public string LastName
    {
        get => _LastName ?? ""; 
        set
        {
            _LastName = value ?? "";
        }
    }
    [FirestoreProperty(nameof(BirthDate))]
    public DateTimeOffset BirthDate
    {
        get => _BirthDate;
        set
        {
            _BirthDate = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BirthDate)));
        }
    }
    [FirestoreProperty(nameof(Email))]
    public string Email
    {
        get => _Email ?? "";
        set
        {
            _Email = value ?? "";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
        }
    }
    [FirestoreProperty(nameof(PhoneNumber))]
    public string PhoneNumber
    {
        get => _PhoneNumber ?? "";
        set
        {
            _PhoneNumber = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneNumber)));
        }
    }
    [FirestoreProperty(nameof(PhoneCountryCode))]
    public string PhoneCountryCode
    {
        get => _PhoneCountryCode ?? "";
        set
        {
            _PhoneCountryCode = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneCountryCode)));
        }
    }
    [FirestoreProperty(nameof(Description))]
    public string Description
    {
        get => _Description ?? "";
        set
        {
            _Description = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
        }
    }
    [FirestoreProperty(nameof(ProfilePictureAddress))]
    public string ProfilePictureAddress
    {
        get => _ProfilePictureAddress ?? "";
        set
        {
            _ProfilePictureAddress = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfilePictureAddress)));
        }
    }
    #endregion
    public User()
    {
        UserID = "";
        FirstName = "";
        LastName = "";
        BirthDate = DateTimeOffset.Now;
        Email = "";
        ProfilePictureAddress = "";
        PhoneCountryCode = "";
        PhoneNumber = "";
        Description = "";
    }

    public User(string userID, string firstName, string lastName, DateTimeOffset birthDate, string mail, 
        string profilePictureAddr = "", ImageSource? profilePictureSrc = null, string phoneNumber = "", string phoneCountryCode = "", string description = "")
    {
        UserID = userID;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Email = mail;
        ProfilePictureAddress = profilePictureAddr;
        ProfilePictureSource = profilePictureSrc ?? ImageSource.FromFile("placeholder_logo.jpg");
        PhoneNumber = phoneNumber;
        PhoneCountryCode = phoneCountryCode;
        Description = description;
    }

    public void UpdateUserData(User userData)
    {
        UserID = userData.UserID;
        FirstName = userData.FirstName;
        LastName = userData.LastName;
        BirthDate = userData.BirthDate;
        Email = userData.Email;
        ProfilePictureAddress = userData.ProfilePictureAddress;
        ProfilePictureSource = userData.ProfilePictureSource;
        PhoneNumber = userData.PhoneNumber;
        PhoneCountryCode = userData.PhoneCountryCode;
        Description = userData.Description;
    }

    public async void UpdateProfilePicture(string address)
    {
        ProfilePictureAddress = address;
        if(ProfilePictureAddress.Length > 0)
        {
            Uri imageUri = new( await DatabaseManager.GetImageDownloadLink(ProfilePictureAddress) );

            ProfilePictureSource = ImageSource.FromUri(imageUri);
        }
    }
}
