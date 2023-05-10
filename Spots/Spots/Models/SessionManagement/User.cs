using Plugin.Firebase.Firestore;
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
        private DateTimeOffset _birthDate;
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
        [FirestoreDocumentId]
        public string userID
        {
            get => _userID;
            private set
            {
                _userID = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(userID)));
            }
        }
        [FirestoreProperty(nameof(firstName))]
        public string firstName
        {
            get => _firstName;
            private set
            {
                _firstName = value ?? "";
                fullName = $"{value} {lastName}";
            }
        }
        [FirestoreProperty(nameof(lastName))]
        public string lastName
        {
            get => _lastName; 
            private set
            {
                _lastName = value ?? "";
                fullName = $"{firstName} {value}";
            }
        }
        [FirestoreProperty(nameof(birthDate))]
        public DateTimeOffset birthDate
        {
            get => _birthDate;
            private set
            {
                _birthDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(birthDate)));
            }
        }
        [FirestoreProperty(nameof(email))]
        public string email
        {
            get => _email;
            private set
            {
                _email = value ?? "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(email)));
            }
        }
        [FirestoreProperty(nameof(profilePictureAddress))]
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
        public User()
        {
            userID = "";
            firstName = "";
            lastName = "";
            birthDate = DateTimeOffset.Now;
            email = "";
            profilePictureAddress = "null";
        }

        public User(string UserID, string FirstName, string LastName, DateTimeOffset BirthDate, string Email, string ProfilePicture)
        {
            userID = UserID;
            firstName = FirstName;
            lastName = LastName;
            birthDate = BirthDate;
            email = Email;
            profilePictureAddress = ProfilePicture;
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
