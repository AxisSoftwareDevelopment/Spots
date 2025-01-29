
namespace Spots.ResourceManager;
    public static class ResourceManagement
    {
        public static string[] GetStringResources(ResourceDictionary? resources, string[] ids)
        {
            List<string> strings = [];

            for (int i = 0; i < ids.Length; i++)
            {
                if (resources != null && resources.TryGetValue(ids[i], out object retrievedValue))
                    strings.Add(retrievedValue.ToString() ?? "");
                else
                    strings.Add(ids[i]);
            }

            return [.. strings];
        }
    }
