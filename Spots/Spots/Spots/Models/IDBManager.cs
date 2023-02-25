using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Spots.Models
{
    public interface IDBManager
    {
        bool SetDocument(string collection, Dictionary<string, string> document);
        Dictionary<string, string> GetDocument(string collection, string id);
        bool DeleteDocument(string collection, string id);
    }
}
