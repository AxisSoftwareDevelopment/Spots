using Plugin.Firebase.Firestore;
using System.ComponentModel;

namespace Spots;
public class Client : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

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
    private FirebaseLocation? _LastLocation;
    private int? _FollowersCount;
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FirstName)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
        }
    }
    public string LastName
    {
        get => _LastName ?? ""; 
        set
        {
            _LastName = value ?? "";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastName)));
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullPhoneNumber)));
        }
    }
    public string PhoneCountryCode
    {
        get => _PhoneCountryCode ?? "";
        set
        {
            _PhoneCountryCode = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneCountryCode)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullPhoneNumber)));
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
    public int FollowersCount
    {
        get => _FollowersCount ?? 0;
        set
        {
            _FollowersCount = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FollowersCount)));
        }
    }
    public FirebaseLocation? LastLocation
    {
        get => _LastLocation;
        set
        {
            if (value == null)
            {
                _LastLocation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastLocation)));
            }
        }
    }
    #endregion
    public Client()
    {
        UserID = "";
        FirstName = "";
        LastName = "";
        BirthDate = DateTimeOffset.Now;
        Email = "";
        PhoneCountryCode = "";
        PhoneNumber = "";
        Description = "";
        FollowersCount = 0;
        LastLocation = null;
    }

    public Client(string userID, string firstName, string lastName, DateTimeOffset birthDate, string mail, 
        ImageSource? profilePictureSrc = null, string phoneNumber = "", string phoneCountryCode = "", string description = "",
        int followersCount = 0, FirebaseLocation? lastLocation = null)
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
        FollowersCount = followersCount;
        LastLocation = lastLocation;
    }

    public Client(Client_Firebase firebaseData, ImageSource profilePictureSrc)
    {
        UserID = firebaseData.UserID;
        FirstName = firebaseData.FirstName;
        LastName = firebaseData.LastName;
        BirthDate = firebaseData.BirthDate;
        Email = firebaseData.Email;
        ProfilePictureSource = profilePictureSrc;
        PhoneNumber = firebaseData.PhoneNumber;
        PhoneCountryCode = firebaseData.PhoneCountryCode;
        Description = firebaseData.Description;
        LastLocation = (firebaseData.LastLocation.Address.Length == 0
            && firebaseData.LastLocation.Latitude == 0
            && firebaseData.LastLocation.Longitude == 0)
            ? null : firebaseData.LastLocation;
        FollowersCount = firebaseData.FollowersCount;
    }

    public void UpdateUserData(Client userData)
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
        FollowersCount = userData.FollowersCount;
        LastLocation = userData.LastLocation;
    }

    public async Task UpdateProfilePicture(string address)
    {
        if(address.Length > 0)
        {
            string downloadAddress = await DatabaseManager.GetImageDownloadLink(address);
            Uri imageUri = new( downloadAddress );

            ProfilePictureSource = ImageSource.FromUri(imageUri);
        }
        else
        {
            ImageSource.FromFile("placeholder_logo.jpg");
        }
    }

    public async Task OpenClientView(INavigation? navigation)
    {
        if (navigation != null)
        {
            List<string> followers = await DatabaseManager.FetchFollowers(UserID);
            bool bFollowing = UserID != SessionManager.CurrentSession?.Client?.UserID
                && followers.Contains(SessionManager.CurrentSession?.Client?.UserID ?? "NULL");

            await navigation.PushAsync(new CP_UserProfile(this, bFollowing));
        }
    }
}

public class Client_Firebase
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

    [FirestoreProperty(nameof(FollowersCount))]
    public int FollowersCount { get; set; }

    [FirestoreProperty(nameof(LastLocation))]
    public FirebaseLocation LastLocation { get; set; }

    [FirestoreProperty(nameof(ClientID_ForSearch))]
    public IList<string> ClientID_ForSearch { get; set; }


    public Client_Firebase()
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
        FollowersCount = 0;
        ClientID_ForSearch = [];
        LastLocation = new("", 0, 0);
    }

    public Client_Firebase(string userID,
        string firstName,
        string lastName,
        DateTimeOffset birthDate,
        string email,
        string phoneNumber,
        string phoneCountryCode,
        string description,
        string profilePictureAddress,
        List<string> searchTerms,
        FirebaseLocation lastLocation,
        int followersCount)
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
        FollowersCount = followersCount;
        LastLocation = lastLocation;
        ClientID_ForSearch = [userID];
    }

    public Client_Firebase(Client userData, string profilePictureAddress)
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
        FollowersCount = userData.FollowersCount;
        LastLocation = userData.LastLocation ?? new("", 0, 0);
        ClientID_ForSearch = [userData.UserID];
    }

    private static List<string> GenerateSearchTerms(string firstName, string lastName)
    {
        List<string> retVal = [];
        List<string> composedTerms = [];

        foreach (string word in firstName.Split(' ').Concat(lastName.Split(' ')))
        {
            string currentTerm = "";
            foreach (char letter in word)
            {
                currentTerm += char.ToUpper(letter);
                retVal.Add(currentTerm);

                AttachComposedStrings(ref retVal, composedTerms.Concat([currentTerm]).ToList());
                foreach (string term in composedTerms)
                {
                    retVal.Add(term + " " + currentTerm);
                }
            }
            composedTerms.Add(currentTerm);
        }

        return retVal.Concat(composedTerms).ToList();
    }

    private static void AttachComposedStrings(ref List<string> retVal, List<string> composedTermsLeft, string currentTerm = "")
    {
        if(composedTermsLeft.Count > 0)
        {
            foreach (string term in composedTermsLeft)
            {
                AttachComposedStrings(ref retVal, composedTermsLeft.Where(compTerm => !compTerm.Equals(term)).ToList(), currentTerm + " " + term);
            }
        }
        else
        {
            retVal.Add(currentTerm.Trim());
        }
    }

    public async Task<ImageSource> GetImageSource()
    {
        if (ProfilePictureAddress.Length > 0)
        {
            string downloadAddress = await DatabaseManager.GetImageDownloadLink(ProfilePictureAddress);
            Uri imageUri = new(downloadAddress);

            return ImageSource.FromUri(imageUri);
        }
        else
        {
            return ImageSource.FromFile("placeholder_logo.jpg");
        }
    }
}

public class FollowRegister
{
    public string RegisterID { get; set; }
    public string FollowedID { get; set; }
    public string FollowerID { get; set; }

    public FollowRegister()
    {
        RegisterID = string.Empty;
        FollowedID = string.Empty;
        FollowerID = string.Empty;
    }

    public FollowRegister(FollowRegister_Firebase register)
    {
        RegisterID = register.RegisterID;
        FollowedID = register.FollowedID;
        FollowerID = register.FollowerID;
    }
}

public class FollowRegister_Firebase
{
    [FirestoreDocumentId]
    public string RegisterID { get; set; }
    [FirestoreProperty(nameof(FollowedID))]
    public string FollowedID { get; set; }
    [FirestoreProperty(nameof(FollowerID))]
    public string FollowerID { get; set; }

    public FollowRegister_Firebase()
    {
        RegisterID = string.Empty;
        FollowedID = string.Empty;
        FollowerID = string.Empty;
    }

    public FollowRegister_Firebase(string folloer, string followed, string id = "")
    {
        RegisterID = id;
        FollowerID = folloer;
        FollowedID = followed;
    }

    public FollowRegister_Firebase(FollowRegister register)
    {
        RegisterID = register.RegisterID;
        FollowedID = register.FollowedID;
        FollowerID = register.FollowerID;
    }
}
