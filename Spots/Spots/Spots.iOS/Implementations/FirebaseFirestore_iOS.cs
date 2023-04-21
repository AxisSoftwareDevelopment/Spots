using Foundation;
using Firebase.CloudFirestore;
using Spots.iOS.Implementations;
using Spots.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseFirestore_iOS))]
namespace Spots.iOS.Implementations
{
    public class FirebaseFirestore_iOS : IFirestoreManager
    {
        public async Task<bool> SaveNewUserDataAsync(string id, Dictionary<string, string> userData)
        {
            NSDictionary<NSString, NSObject> nsUserData = DictionaryToNSDictionary(userData);
            try
            {
                await Firestore.SharedInstance.GetCollection("UserData").GetDocument(id).SetDataAsync(nsUserData);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public async Task<Dictionary<string, string>> GetUserDataAsync(string userID)
        {
            DocumentSnapshot DS_UserData = await Firestore.SharedInstance.GetCollection("UserData").GetDocument(userID).GetDocumentAsync();

            return DocumentSnapshotToDictionary(DS_UserData);
        }

        #region Utilites
        private NSDictionary<NSString, NSObject> DictionaryToNSDictionary(Dictionary<string, string> dict)
        {
            NSDictionary<NSString, NSObject> nsDict = new NSDictionary<NSString, NSObject>();

            foreach(string key in dict.Keys)
            {
                nsDict.SetValueForKey(NSObject.FromObject(dict[key]), (NSString)NSObject.FromObject(key));
            }
            
            return nsDict;
        }

        private Dictionary<string, string> DocumentSnapshotToDictionary(DocumentSnapshot document)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            NSDictionary<NSString, NSObject> nsDict = document.GetData(ServerTimestampBehavior.Previous);

            foreach (var key in nsDict.Keys)
            {
                dict[key.Description] = nsDict[key].Description;
            }

            return dict;
        }
        #endregion
    }
}