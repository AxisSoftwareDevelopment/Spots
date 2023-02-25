using Firebase.Firestore;
using Java.Util;
using Spots.Droid.Implementations;
using Spots.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseFirestore_DBManager))]
namespace Spots.Droid.Implementations
{
    public class FirebaseFirestore_DBManager : IDBManager
    {
        Dictionary<string, string> IDBManager.GetDocument(string collection, string id)
        {
            DocumentReference docRef = FirebaseFirestore.Instance.Collection(collection).Document(id);
            //HashMap document = docRef.Get();
            return new Dictionary<string, string>();
        }

        bool IDBManager.SetDocument(string collection, Dictionary<string, string> document)
        {
            try
            {
                HashMap parsedDocument = DictionaryToHashMap(document);

                FirebaseFirestore.Instance.Collection(collection).Document().Set(parsedDocument);
                return true;
            }
            catch
            {
                return false;
            }
        }

        bool IDBManager.DeleteDocument(string collection, string id)
        {
            throw new NotImplementedException();
        }

        private HashMap DictionaryToHashMap(Dictionary<string, string> document)
        {
            HashMap map = new HashMap();

            foreach (string key in document.Keys)
            {
                map.Put(key, document[key]);
            }

            return map;
        }

        private Dictionary<string, string> HashMapToDictionary(HashMap map)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (string key in map.KeySet())
            {
                dict[key] = map.Get(key).ToString();
            }

            return dict;
        }
    }
}