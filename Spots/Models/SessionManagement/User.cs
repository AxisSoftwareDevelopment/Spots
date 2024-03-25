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
    private string? _PhoneNumber;
    private string? _PhoneCountryCode;
    private string? _Description;
    private ImageSource? _ProfilePictureSource;
    #endregion

    #region Public Parameters
    public bool UserDataRetrieved
    {
        get => FirstName.Length > 0;
    }
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
    public string UserID
    {
        get => _UserID ?? "";
        set
        {
            _UserID = value ?? "";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserID)));
        }
    }
    public string FirstName
    {
        get => _FirstName ?? "";
        set
        {
            _FirstName = value ?? "";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
        }
    }
    public string LastName
    {
        get => _LastName ?? ""; 
        set
        {
            _LastName = value ?? "";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
        }
    }
    public DateTimeOffset BirthDate
    {
        get => _BirthDate;
        set
        {
            _BirthDate = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BirthDate)));
        }
    }
    public string Email
    {
        get => _Email ?? "";
        set
        {
            _Email = value ?? "";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
        }
    }
    public string PhoneNumber
    {
        get => _PhoneNumber ?? "";
        set
        {
            _PhoneNumber = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneNumber)));
        }
    }
    public string PhoneCountryCode
    {
        get => _PhoneCountryCode ?? "";
        set
        {
            _PhoneCountryCode = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneCountryCode)));
        }
    }
    public string Description
    {
        get => _Description ?? "";
        set
        {
            _Description = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
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
        PhoneCountryCode = "";
        PhoneNumber = "";
        Description = "";
    }

    public User(string userID, string firstName, string lastName, DateTimeOffset birthDate, string mail, 
        ImageSource? profilePictureSrc = null, string phoneNumber = "", string phoneCountryCode = "", string description = "")
    {
        UserID = userID;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Email = mail;
        ProfilePictureSource = profilePictureSrc ?? ImageSource.FromFile("placeholder_logo.jpg");
        PhoneNumber = phoneNumber;
        PhoneCountryCode = phoneCountryCode;
        Description = description;
    }

    public User(User_Firebase firebaseData)
    {
        UserID = firebaseData.UserID;
        FirstName = firebaseData.FirstName;
        LastName = firebaseData.LastName;
        BirthDate = firebaseData.BirthDate;
        Email = firebaseData.Email;
        UpdateProfilePicture(firebaseData.ProfilePictureAddress);
        PhoneNumber = firebaseData.PhoneNumber;
        PhoneCountryCode = firebaseData.PhoneCountryCode;
        Description = firebaseData.Description;
    }

    public void UpdateUserData(User userData)
    {
        UserID = userData.UserID;
        FirstName = userData.FirstName;
        LastName = userData.LastName;
        BirthDate = userData.BirthDate;
        Email = userData.Email;
        ProfilePictureSource = userData.ProfilePictureSource;
        PhoneNumber = userData.PhoneNumber;
        PhoneCountryCode = userData.PhoneCountryCode;
        Description = userData.Description;
    }

    public async void UpdateProfilePicture(string address)
    {
        if(address.Length > 0)
        {
            Uri imageUri = new( await DatabaseManager.GetImageDownloadLink(address) );

            ProfilePictureSource = ImageSource.FromUri(imageUri);
        }
        else
        {
            ImageSource.FromFile("placeholder_logo.jpg");
        }
    }
}

public class User_Firebase
{
    [FirestoreDocumentId]
    public string UserID { get; set; }

    [FirestoreProperty(nameof(FirstName))]
    public string FirstName { get; set; }

    [FirestoreProperty(nameof(LastName))]
    public string LastName { get; set; }

    [FirestoreProperty(nameof(BirthDate))]
    public DateTimeOffset BirthDate { get; set; }

    [FirestoreProperty(nameof(Email))]
    public string Email { get; set; }

    [FirestoreProperty(nameof(PhoneNumber))]
    public string PhoneNumber { get; set; }

    [FirestoreProperty(nameof(PhoneCountryCode))]
    public string PhoneCountryCode { get; set; }

    [FirestoreProperty(nameof(Description))]
    public string Description { get; set; }

    [FirestoreProperty(nameof(ProfilePictureAddress))]
    public string ProfilePictureAddress { get; set; }

    [FirestoreProperty(nameof(SearchTerms))]
    public IList<string> SearchTerms { get; set; }

    public User_Firebase()
    {
        UserID = "";
        FirstName = "";
        LastName = "";
        BirthDate = DateTimeOffset.Now;
        Email = "";
        PhoneNumber = "";
        PhoneCountryCode = "";
        Description = "";
        ProfilePictureAddress = "";
        SearchTerms = [];
    }

    public User_Firebase(string userID,
        string firstName,
        string lastName,
        DateTimeOffset birthDate,
        string email,
        string phoneNumber,
        string phoneCountryCode,
        string description,
        string profilePictureAddress,
        List<string> searchTerms)
    {
        UserID = userID;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Email = email;
        PhoneNumber = phoneNumber;
        PhoneCountryCode = phoneCountryCode;
        Description = description;
        ProfilePictureAddress = profilePictureAddress;
        SearchTerms = searchTerms;
    }

    public User_Firebase(User userData, string profilePictureAddress)
    {
        UserID = userData.UserID;
        FirstName = userData.FirstName;
        LastName = userData.LastName;
        BirthDate = userData.BirthDate;
        Email = userData.Email;
        PhoneNumber = userData.PhoneNumber;
        PhoneCountryCode = userData.PhoneCountryCode;
        Description = userData.Description;
        ProfilePictureAddress = profilePictureAddress;
        SearchTerms = GenerateSearchTerms(FirstName, LastName);        
    }

    private List<string> GenerateSearchTerms(string firstName, string lastName)
    {
        List<string> retVal = [];

        foreach (string word in firstName.Split(' ').Concat(lastName.Split(' ')))
        {
            string currentTerm = "";
            foreach (char letter in word)
            {
                currentTerm += letter;
                retVal.Add(currentTerm);
            }
        }

        return retVal;
    }
}
