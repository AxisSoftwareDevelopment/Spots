using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using static Android.Renderscripts.Sampler;

namespace Spots.Models
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
        private Image _profilePicture;
        private string _profilePictureAddress;
        #endregion

        #region Public Parameters
        public string fullName
        {
            get { return _fullName; }
            private set 
            {
                _fullName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(fullName)));
            }
        }
        public string userID 
        { 
            get { return _userID; } 
            private set 
            {
                _userID = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(userID)));
            }
        }
        public string firstName
        {
            get { return _firstName; }
            private set
            {
                _firstName = value;
                fullName = $"{value} {lastName}";
            }
        }
        public string lastName
        {
            get { return _lastName; }
            private set
            {
                _lastName = value;
                fullName = $"{firstName} {value}";
            }
        }
        public string birthDate
        {
            get { return _birthDate; }
            private set
            {
                _birthDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(birthDate)));
            }
        }
        public string email
        {
            get { return _email; }
            private set
            {
                _email = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(email)));
            }
        }
        public Image profilePicture
        {
            get { return _profilePicture; }
            private set
            {
                _profilePicture = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(profilePicture)));
            }
        }

        public string profilePictureAddress
        {
            get { return profilePicture.Source.ToString().Replace("File: ", ""); }
            set
            {
                _profilePictureAddress = value;

                if (_profilePictureAddress.Equals("null") || _profilePictureAddress.Length == 0)
                {
                    // Set default profile picture
                    profilePicture = new Image() { Source = RsrcManager.GetImagePath("img_Logo") };
                }
                else
                {
                    // Get image from DB
                    profilePicture = new Image();
                }
            }
        }
        #endregion

        // Default Constructor
        public User() 
            : this(null, new Dictionary<string, string>()
            {
                { "firstName", "First Name" },
                { "lastName", "Last Name"},
                { "birthDate", "05/05/1555" },
                { "email", "example@mail.com" },
                { "profilePictureAddress", "null" }
            }) 
        { }

        // Regular Constructor
        public User(string userID, Dictionary<string, string> userData)
        {
            this.userID = userID;
            firstName = userData["firstName"].Length > 0 ? userData["firstName"] : "First Name";
            lastName = userData["lastName"].Length > 0 ? userData["lastName"] : "Last Name";
            birthDate = userData["birthDate"].Length > 0 ? userData["birthDate"] : "05/05/1555";
            email = userData["email"].Length > 0 ? userData["email"] : "example@mail.com";
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
    }
}
