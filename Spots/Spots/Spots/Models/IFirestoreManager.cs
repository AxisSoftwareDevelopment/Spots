using Java.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Models
{
    public interface IFirestoreManager
    {
        Task<bool> SaveNewUserDataAsync(string id, Dictionary<string, string> userData);
    }
}
