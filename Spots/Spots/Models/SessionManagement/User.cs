using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Models.SessionManagement
{
    public class User : BindableObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Private Parameters
        private string _fullName;
        private string _userID;
        private string _firstName;
        private string _lastName;
        private string _birthDate;
        private string _email;
        private string _profilePictureAddress;
        #endregion

        #region Public Parameters
        public string fullName
        {
            get => _fullName; 
            private set
            {
                _fullName = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(fullName)));
            }
        }
        public string userID
        {
            get => _userID;
            private set
            {
                _userID = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(userID)));
            }
        }
        public string firstName
        {
            get => _firstName;
            private set
            {
                _firstName = value ?? "";
                fullName = $"{value} {lastName}";
            }
        }
        public string lastName
        {
            get => _lastName; 
            private set
            {
                _lastName = value ?? "";
                fullName = $"{firstName} {value}";
            }
        }
        public string birthDate
        {
            get => _birthDate;
            private set
            {
                _birthDate = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(birthDate)));
            }
        }
        public string email
        {
            get => _email;
            private set
            {
                _email = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(email)));
            }
        }

        public string profilePictureAddress
        {
            get => _profilePictureAddress.Equals("null") ? "dotnet_bot.png" : _profilePictureAddress; 
            set
            {
                _profilePictureAddress = value ?? "null";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(profilePictureAddress)));
            }
        }
        #endregion

        // Default Constructor
        public User()
            : this(null, new Dictionary<string, string>()
            {
                { "firstName", "" },
                { "lastName", ""},
                { "birthDate", "" },
                { "email", "" },
                { "profilePictureAddress", "null" }
            })
        { }

        // Regular Constructor
        public User(string userID, Dictionary<string, string> userData)
        {
            this.userID = userID;
            firstName = userData["firstName"].Length > 0 ? userData["firstName"] : "";
            lastName = userData["lastName"].Length > 0 ? userData["lastName"] : "";
            birthDate = userData["birthDate"].Length > 0 ? userData["birthDate"] : "";
            email = userData["email"].Length > 0 ? userData["email"] : "";
            profilePictureAddress = userData["profilePictureAddress"].Length > 0 ? userData["profilePictureAddress"] : "null";
        }

        public Dictionary<string, string> ToDictionary()
        {
            userID = null;
            return new Dictionary<string, string> {
                { "firstName", firstName },
                { "lastName", lastName},
                { "birthDate", birthDate },
                { "email", email },
                { "profilePictureAddress", profilePictureAddress }
            };
        }

        public void UpdateUserData(User userData)
        {
            userID = userData.userID;
            firstName = userData.firstName;
            lastName = userData.lastName;
            birthDate = userData.birthDate;
            email = userData.email;
            profilePictureAddress = userData.profilePictureAddress;
        }
    }
}
