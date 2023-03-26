using Android.Gms.Extensions;
using Android.Gms.Tasks;
using Firebase.Firestore;
using Java.Interop;
using Java.Util;
using Spots.Droid.Implementations;
using Spots.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Schema;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseFirestore_Android))]
namespace Spots.Droid.Implementations
{
    public class FirebaseFirestore_Android : IFirestoreManager
    {
        public async Task<bool> SaveNewUserDataAsync(string id, Dictionary<string, string> userData)
        {
            HashMap hmUserData = DictionaryToHashMap(userData);
            try
            {
                await FirebaseFirestore.Instance.Collection("UserData").Document(id).Set(hmUserData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private HashMap DictionaryToHashMap(Dictionary<string, string> dict)
        {
            HashMap hasmap = new HashMap();

            foreach(string key in dict.Keys)
            {
                hasmap.Put(key, dict[key]);
            }

            return hasmap;
        }

        private Dictionary<string, string> SnapshotToDictionary(DocumentSnapshot snapshot)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var dictSnapshot = snapshot.Data;

            foreach (string key in dictSnapshot.Keys)
            {
                Java.Lang.Object value;
                if( dictSnapshot.TryGetValue(key, out value) )
                {
                    dict[key] = value.ToString();
                }
            }

            return dict;
        }
    }
}