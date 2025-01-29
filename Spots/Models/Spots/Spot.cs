using Plugin.Firebase.Firestore;
using System.ComponentModel;

using Spots.Database;

namespace Spots.Models;
public class Spot : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    #region Private Parameters
    private string? _SpotID;
    private string? _brandName;
    private string? _businessName;
    private ImageSource? _profilePictureSource;
    private string? _phoneNumber;
    private string? _phoneCountryCode;
    private string? _description;
    private FirebaseLocation? _location;
    private int? _praiseCount;
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
    public int PraiseCount
    {
        get => _praiseCount ?? 0;
        set
        {
            _praiseCount = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PraiseCount)));
        }
    }
    #endregion

    public Spot()
    {
        BrandName = "";
        SpotName = "";
        PhoneCountryCode = "";
        PhoneNumber = "";
        Description = "";
        Location = new FirebaseLocation();
        PraiseCount = 0;
    }

    public Spot(string userID, string brandName, string businessName, ImageSource? profilePictureSource = null,
        string phoneNumber = "", string phoneCountryCode = "", string description = "", FirebaseLocation? location = null, int? praiseCount = null)
    {
        SpotID = userID;
        BrandName = brandName;
        SpotName = businessName;
        ProfilePictureSource = profilePictureSource ?? ImageSource.FromFile("placeholder_logo.jpg");
        PhoneNumber = phoneNumber;
        PhoneCountryCode = phoneCountryCode;
        Description = description;
        Location = location ?? new FirebaseLocation();
        PraiseCount = praiseCount ?? 0;
    }

    public Spot(Spot_Firebase spotData, ImageSource profilePictureSource)
    {
        SpotID = spotData.SpotID;
        BrandName = spotData.BrandName;
        SpotName = spotData.SpotName;
        ProfilePictureSource = profilePictureSource;
        PhoneNumber = spotData.PhoneNumber;
        PhoneCountryCode = spotData.PhoneCountryCode;
        Description = spotData.Description;
        Location = spotData.Location;
        PraiseCount = spotData.PraiseCount;
    }

    public void UpdateUserData(Spot userData)
    {
        BrandName = userData.BrandName;
        SpotName = userData.SpotName;
        ProfilePictureSource = userData.ProfilePictureSource;
        PhoneNumber = userData.PhoneNumber;
        PhoneCountryCode = userData.PhoneCountryCode;
        Description = userData.Description;
        Location = userData.Location;
        PraiseCount = userData.PraiseCount;
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
    [FirestoreProperty(nameof(PraiseCount))]
    public int PraiseCount { get; set; }
    [FirestoreProperty(nameof(SearchTerms))]
    public IList<string> SearchTerms { get; set; }

    public Spot_Firebase(string spotID,
        string brandName,
        string businessName,
        string phoneNumber,
        string phoneCountryCode,
        string description,
        FirebaseLocation location,
        string profilePictureAddress,
        int praiseCount,
        List<string> searchTerms)
    {
        SpotID = spotID;
        BrandName = brandName;
        SpotName = businessName;
        PhoneNumber = phoneNumber;
        PhoneCountryCode = phoneCountryCode;
        Description = description;
        Location = location;
        ProfilePictureAddress = profilePictureAddress;
        PraiseCount = praiseCount;
        SearchTerms = searchTerms;
    }

    public Spot_Firebase()
    {
        SpotID = "";
        BrandName = "";
        SpotName = "";
        PhoneNumber = "";
        PhoneCountryCode = "";
        Description = "";
        Location = new();
        ProfilePictureAddress = "";
        PraiseCount = 0;
        SearchTerms = [];
    }

    public Spot_Firebase(Spot spotData, string profilePictureAddress)
    {
        SpotID = spotData.SpotID;
        BrandName = spotData.BrandName;
        SpotName = spotData.SpotName;
        PhoneNumber = spotData.PhoneNumber;
        PhoneCountryCode = spotData.PhoneCountryCode;
        Description = spotData.Description;
        Location = spotData.Location;
        ProfilePictureAddress = profilePictureAddress;
        PraiseCount = spotData.PraiseCount;
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