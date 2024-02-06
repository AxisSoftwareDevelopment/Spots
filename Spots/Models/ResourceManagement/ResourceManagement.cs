
namespace Spots;
    public static class ResourceManagement
    {
        public static string[] GetStringResources(ResourceDictionary resources, string[] ids)
        {
            List<string> strings = new();

            for (int i = 0; i < ids.Length; i++)
            {
                if (resources.TryGetValue(ids[i], out object retrievedValue))
                    strings.Add(retrievedValue.ToString() ?? "");
                    //TODO: strings.Add((string)retrievedValue);
                else
                    strings.Add(ids[i]);
            }

            return [.. strings];
        }
    }
