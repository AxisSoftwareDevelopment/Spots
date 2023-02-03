using System;
using System.Collections.Generic;
using System.Text;

namespace Spots.Models
{
    internal interface IDBManager
    {
        bool StartConnectionAsync(string apiKey);
    }
}
