using Plugin.Firebase.Firestore;
using System.ComponentModel;

namespace Spots;
public class Spot : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    #region Private Parameters
    private string? _SpotID;
    private string? _brandName;
    private string? _businessName;
    private string? _email;
    private ImageSource? _profilePictureSource;
    private string? _phoneNumber;
    private string? _phoneCountryCode;
    private string? _description;
    private FirebaseLocation? _location;
    private List<string>? _praisers;
    #endregion

    #region Public Parameters
    public string SpotID
    {
        get => _SpotID ?? "";
        set
        {
            _SpotID = value ?? "";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SpotID)));
        }
    }
    public ImageSource ProfilePictureSource
    {
        get => _profilePictureSource ?? ImageSource.FromFile("placeholder_logo.jpg");
        set
        {
            _profilePictureSource = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfilePictureSource)));
        }
    }
    public string FullPhoneNumber
    {
        get => (PhoneNumber?.Length < 10 || PhoneCountryCode.Length < 2) ? "+ -- --- --- ----" : $"+({_phoneCountryCode}) {PhoneNumber?[..3]} {PhoneNumber?.Substring(3, 3)} {PhoneNumber?.Substring(6, 4)}";
    }
    public string FullName
    {
        get => BrandName + " - " + SpotName;
    }
    public string BrandName
    {
        get => _brandName ?? "";
        set
        {
            _brandName = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BrandName)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
        }
    }
    public string SpotName
    {
        get => _businessName ?? "";
        set
        {
            _businessName = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SpotName)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
        }
    }
    public string Email
    {
        get => _email ?? "";
        set
        {
            _email = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
        }
    }
    public string PhoneNumber
    {
        get => _phoneNumber ?? "";
        set
        {
            _phoneNumber = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneNumber)));
        }
    }
    public string PhoneCountryCode
    {
        get => _phoneCountryCode ?? "";
        set
        {
            _phoneCountryCode = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneCountryCode)));
        }
    }
    public string Description
    {
        get => _description ?? "";
        set
        {
            _description = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
        }
    }
    public FirebaseLocation Location
    {
        get => _location ?? new FirebaseLocation();
        set
        {
            _location = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Location)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Geolocation)));
        }
    }
    public Location Geolocation
    {
        get => new(Location.Latitude, Location.Longitude);
        set
        {
            Location = new FirebaseLocation(Location.Address,
                value.Latitude,
                value.Longitude);
        }
    }
    public List<string> Praisers
    {
        get => _praisers ?? [];
        set
        {
            _praisers = value ?? [];
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Praisers)));
        }
    }
    #endregion

    public Spot()
    {
        BrandName = "";
        SpotName = "";
        Email = "";
        PhoneCountryCode = "";
        PhoneNumber = "";
        Description = "";
        Location = new FirebaseLocation();
        Praisers = [];
    }

    public Spot(string userID, string brandName, string businessName, string email, string profilePictureAddress = "", ImageSource? profilePictureSource = null,
        string phoneNumber = "", string phoneCountryCode = "", string description = "", FirebaseLocation? location = null, List<string>? praisers = null)
    {
        BrandName = brandName;
        SpotName = businessName;
        Email = email;
        ProfilePictureSource = profilePictureSource ?? ImageSource.FromFile("placeholder_logo.jpg");
        PhoneNumber = phoneNumber;
        PhoneCountryCode = phoneCountryCode;
        Description = description;
        Location = location ?? new FirebaseLocation();
        Praisers = praisers ?? [];
    }

    public Spot(Spot_Firebase spotData, ImageSource profilePictureSource)
    {
        BrandName = spotData.BrandName;
        SpotName = spotData.SpotName;
        Email = spotData.Email;
        ProfilePictureSource = profilePictureSource;
        PhoneNumber = spotData.PhoneNumber;
        PhoneCountryCode = spotData.PhoneCountryCode;
        Description = spotData.Description;
        Location = spotData.Location;
        Praisers = spotData.Praisers.ToList();
    }

    public void UpdateUserData(Spot userData)
    {
        BrandName = userData.BrandName;
        SpotName = userData.SpotName;
        Email = userData.Email;
        ProfilePictureSource = userData.ProfilePictureSource;
        PhoneNumber = userData.PhoneNumber;
        PhoneCountryCode = userData.PhoneCountryCode;
        Description = userData.Description;
        Location = userData.Location;
        Praisers = userData.Praisers;
    }

    public async Task UpdateProfilePicture(string firebaseAddress)
    {
        if (firebaseAddress.Length > 0)
        {
            string downloadAddress = await DatabaseManager.GetImageDownloadLink(firebaseAddress);
            Uri imageUri = new(downloadAddress);

            ProfilePictureSource = ImageSource.FromUri(imageUri);
        }
        else
        {
            ProfilePictureSource = ImageSource.FromFile("placeholder_logo.jpg");
        }
    }
}

public class Spot_Firebase
{
    [FirestoreDocumentId]
    public string SpotID { get; set; }
    [FirestoreProperty(nameof(BrandName))]
    public string BrandName { get; set; }
    [FirestoreProperty(nameof(SpotName))]
    public string SpotName { get; set; }
    [FirestoreProperty(nameof(Email))]
    public string Email { get; set; }
    [FirestoreProperty(nameof(PhoneNumber))]
    public string PhoneNumber { get; set; }
    [FirestoreProperty(nameof(PhoneCountryCode))]
    public string PhoneCountryCode { get; set; }
    [FirestoreProperty(nameof(Description))]
    public string Description { get; set; }
    [FirestoreProperty(nameof(Location))]
    public FirebaseLocation Location { get; set; }
    [FirestoreProperty(nameof(ProfilePictureAddress))]
    public string ProfilePictureAddress { get; set; }
    [FirestoreProperty(nameof(Praisers))]
    public IList<string> Praisers { get; set; }
    [FirestoreProperty(nameof(SearchTerms))]
    public IList<string> SearchTerms { get; set; }

    public Spot_Firebase(string spotID,
        string brandName,
        string businessName,
        string email,
        string phoneNumber,
        string phoneCountryCode,
        string description,
        FirebaseLocation location,
        string profilePictureAddress,
        List<string> praises,
        List<string> searchTerms)
    {
        SpotID = spotID;
        BrandName = brandName;
        SpotName = businessName;
        Email = email;
        PhoneNumber = phoneNumber;
        PhoneCountryCode = phoneCountryCode;
        Description = description;
        Location = location;
        ProfilePictureAddress = profilePictureAddress;
        Praisers = praises;
        SearchTerms = searchTerms;
    }

    public Spot_Firebase()
    {
        SpotID = "";
        BrandName = "";
        SpotName = "";
        Email = "";
        PhoneNumber = "";
        PhoneCountryCode = "";
        Description = "";
        Location = new();
        ProfilePictureAddress = "";
        Praisers = [];
        SearchTerms = [];
    }

    public Spot_Firebase(Spot spotData, string profilePictureAddress)
    {
        SpotID = spotData.SpotID;
        BrandName = spotData.BrandName;
        SpotName = spotData.SpotName;
        Email = spotData.Email;
        PhoneNumber = spotData.PhoneNumber;
        PhoneCountryCode = spotData.PhoneCountryCode;
        Description = spotData.Description;
        Location = spotData.Location;
        ProfilePictureAddress = profilePictureAddress;
        Praisers = spotData.Praisers;
        SearchTerms = GenerateSearchTerms(BrandName, SpotName);
    }

    private List<string> GenerateSearchTerms(string brandName, string spotName)
    {
        List<string> retVal = [];
        List<string> composedTerms = [];

        foreach (string word in brandName.Split(' ').Concat(spotName.Split(' ')))
        {
            string currentTerm = "";
            foreach (char letter in word)
            {
                currentTerm += char.ToUpper(letter);
                retVal.Add(currentTerm);

                foreach (string term in composedTerms)
                {
                    retVal.Add(term + " " + currentTerm);
                }
            }
            composedTerms.Add(currentTerm);
        }

        return retVal.Concat(composedTerms).ToList();
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