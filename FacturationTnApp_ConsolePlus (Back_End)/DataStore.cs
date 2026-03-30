using System.IO;
using System.Text.Json;

namespace FacturationTnApp
{
    public static class DataStore
    {
        public static void Save(string filePath, AppData data)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json);
        }

        public static AppData Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new AppData();
            }

            string json = File.ReadAllText(filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new AppData();
            }

            AppData? data = JsonSerializer.Deserialize<AppData>(json);
            return data ?? new AppData();
        }
    }
}
