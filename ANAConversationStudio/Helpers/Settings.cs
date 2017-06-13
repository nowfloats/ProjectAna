using ANAConversationStudio.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ANAConversationStudio.Helpers
{
    public class Settings
    {
        const string FILE_NAME = "Settings.json";

        public List<DatabaseConnection> SavedConnections { get; set; } = new List<DatabaseConnection>();
        public EditableSettings UpdateDetails { get; set; } = new EditableSettings();
        public static bool IsEncrypted()
        {
            try
            {
                if (!File.Exists(FILE_NAME)) return false;

                JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FILE_NAME));
                //If the settings file gets parsed without decryption, it means, the password is not set yet.
                return false;
            }
            catch
            {
                return true;
            }
        }
        public static Settings Load(string password)
        {
            if (File.Exists(FILE_NAME))
                return JsonConvert.DeserializeObject<Settings>(Utilities.Decrypt(File.ReadAllText(FILE_NAME), password));
            return new Settings();
        }

        public void Save(string password)
        {
            File.WriteAllText(FILE_NAME, Utilities.Encrypt(JsonConvert.SerializeObject(this), password));
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

    public class EditableSettings
    {
        public string StudioUpdateUrl { get; set; }
    }

    public class AutoUpdateResponse
    {
        public string DownloadLink { get; set; }
        public Version Version { get; set; }
    }
}
