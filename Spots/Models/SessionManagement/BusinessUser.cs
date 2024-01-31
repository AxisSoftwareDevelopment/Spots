using Plugin.Firebase.Firestore;
using System.ComponentModel;

namespace Spots;
public class BusinessUser : BindableObject, INotifyPropertyChanged
{
    new public event PropertyChangedEventHandler? PropertyChanged;

    #region Private Parameters
    private string? _brandName;
    private string? _businessName;
    private string? _userID;
    private string? _email;
    private string? _profilePictureAddress;
    private ImageSource? _profilePictureSource;
    private string? _phoneNumber;
    private string? _phoneCountryCode;
    private string? _description;
    private FirebaseLocation? _location;
    private List<string>? _praises;
    #endregion

    #region Public Parameters
    public bool userDataRetrieved = false;
    public ImageSource ProfilePictureSource
    {
        get => _profilePictureSource ?? ImageSource.FromFile("placeholder_logo.jpg");
        set
        {
            _profilePictureSource = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfilePictureSource)));
        }
    }
    public string fullPhoneNumber
    {
        get => (PhoneNumber?.Length < 10 || PhoneCountryCode.Length < 2) ? "+ -- --- --- ----" : $"+({_phoneCountryCode}) {PhoneNumber?[..3]} {PhoneNumber?.Substring(3, 3)} {PhoneNumber?.Substring(6, 4)}";
    }
    [FirestoreDocumentId]
    public string UserID
    {
        get => _userID ?? "";
        set
        {
            _userID = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserID)));
        }
    }
    [FirestoreProperty(nameof(BrandName))]
    public string BrandName
    {
        get => _brandName ?? "";
        set
        {
            _brandName = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BrandName)));
        }
    }
    [FirestoreProperty(nameof(BusinessName))]
    public string BusinessName
    {
        get => _businessName ?? "";
        set
        {
            _businessName = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BusinessName)));
        }
    }
    [FirestoreProperty(nameof(Email))]
    public string Email
    {
        get => _email ?? "";
        set
        {
            _email = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
        }
    }
    [FirestoreProperty(nameof(PhoneNumber))]
    public string PhoneNumber
    {
        get => _phoneNumber ?? "";
        set
        {
            _phoneNumber = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneNumber)));
        }
    }
    [FirestoreProperty(nameof(PhoneCountryCode))]
    public string PhoneCountryCode
    {
        get => _phoneCountryCode ?? "";
        set
        {
            _phoneCountryCode = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneCountryCode)));
        }
    }
    [FirestoreProperty(nameof(Description))]
    public string Description
    {
        get => _description ?? "";
        set
        {
            _description = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
        }
    }
    [FirestoreProperty(nameof(Location))]
    public FirebaseLocation Location
    {
        get => _location ?? new FirebaseLocation();
        set
        {
            _location = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Location)));
        }
    }
    public Location Geolocation
    {
        get => new Location(Location.Latitude, Location.Longitude);
        set
        {
            Location = new FirebaseLocation(Location.Address,
                value.Latitude,
                value.Longitude);
        }
    }
    [FirestoreProperty(nameof(ProfilePictureAddress))]
    public string ProfilePictureAddress
    {
        get => _profilePictureAddress ?? "";
        set
        {
            _profilePictureAddress = value.Equals("") ? null : value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfilePictureAddress)));
        }
    }

    public List<string> Praises
    {
        get => _praises ?? [];
        set
        {
            _praises = value ?? [];
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
        ProfilePictureAddress = "";
        PhoneCountryCode = "";
        PhoneNumber = "";
        Description = "";
        Location = new FirebaseLocation();
        Praises = new List<string>();
    }

    public BusinessUser(string userID, string brandName, string businessName, string email, string? profilePictureAddress = null, ImageSource? profilePictureSource = null,
        string? phoneNumber = null, string? phoneCountryCode = null, string? description = null, FirebaseLocation? location = null, List<string>? praises = null)
    {
        UserID = userID;
        BrandName = brandName;
        BusinessName = businessName;
        Email = email;
        ProfilePictureAddress = profilePictureAddress ?? "";
        ProfilePictureSource = profilePictureSource ?? ImageSource.FromFile("placeholder_logo.jpg");
        PhoneNumber = phoneNumber ?? "";
        PhoneCountryCode = phoneCountryCode ?? "";
        Description = description ?? "";
        Location = location ?? new FirebaseLocation();
        Praises = praises ?? [];

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
