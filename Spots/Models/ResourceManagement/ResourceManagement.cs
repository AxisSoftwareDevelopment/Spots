using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Models.ResourceManagement
{
    public static class ResourceManagement
    {
        public static string[] GetStringResources(ResourceDictionary resources, string[] ids)
        {
            List<string> strings = new();

            for (int i = 0; i < ids.Length; i++)
            {
                if (resources.TryGetValue(ids[i], out object retrievedValue))
                    strings.Add(retrievedValue.ToString());
                else
                    strings.Add(ids[i]);
            }

            return strings.ToArray();
        }
    }
}
