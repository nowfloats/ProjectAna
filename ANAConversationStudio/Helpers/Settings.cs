using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ANAConversationStudio.Helpers
{
    public class Settings
    {
        const string FILE_NAME = "Settings.json";

        public List<DatabaseConnection> SavedConnections { get; set; } = new List<DatabaseConnection>();

        public static Settings Load()
        {
            if (File.Exists(FILE_NAME))
                return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FILE_NAME));
            return new Settings();
        }
        public void Save()
        {
            File.WriteAllText(FILE_NAME, JsonConvert.SerializeObject(this));
        }
    }

    public class DatabaseConnection
    {
        public string TemplateCollectionName { get; set; }
        public string ContentCollectionName { get; set; }
        public string LayoutCollectionName { get; set; }

        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return "Unnamed Connection";
            return Name;
        }

        public bool IsValid()
        {
            return Utilities.ValidateStrings(TemplateCollectionName, ContentCollectionName, LayoutCollectionName, DatabaseName, ConnectionString, Name);
        }
    }
}
