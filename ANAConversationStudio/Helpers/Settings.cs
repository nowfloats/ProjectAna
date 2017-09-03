using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ANAConversationStudio.Helpers
{
    public class Settings
    {
        const string SettingsFile = "Settings";
        const string SettingsFileExtention = ".json";
        private static string SettingsJsonDirectory
        {
            get
            {
#if STANDALONE
                return ""; //Use current directory to save settings when in standalone mode
#else
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ANAConversationStudio");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
#endif
            }
        }
        public static string SettingsJsonBackupDirectory
        {
            get
            {
                var dir = Path.Combine(SettingsJsonDirectory, "SettingsBackups");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
        }
        private static string SettingsJsonPath => Path.Combine(SettingsJsonDirectory, SettingsFile + SettingsFileExtention);

        public List<ChatServerConnection> SavedChatServerConnections { get; set; } = new List<ChatServerConnection>();
        public EditableSettings UpdateDetails { get; set; } = new EditableSettings();
        public static bool IsEncrypted()
        {
            try
            {
                if (!File.Exists(SettingsJsonPath)) return false;

                JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsJsonPath));
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
            if (File.Exists(SettingsJsonPath))
                return JsonConvert.DeserializeObject<Settings>(Utilities.Decrypt(File.ReadAllText(SettingsJsonPath), password));
            return new Settings();
        }

        public void Save(string password)
        {
            File.WriteAllText(SettingsJsonPath, Utilities.Encrypt(JsonConvert.SerializeObject(this), password));
        }

        public static void Delete()
        {
            if (File.Exists(SettingsJsonPath))
                File.Move(SettingsJsonPath, Path.Combine(SettingsJsonBackupDirectory, SettingsFile + "-" + Guid.NewGuid().ToString() + SettingsFileExtention));
        }
    }

    public class ChatServerConnection
    {
        [PropertyOrder(1)]
        [DisplayName("Connection Name")]
        public string Name { get; set; }
        [PropertyOrder(2)]
        [DisplayName("Server URL")]
        public string ServerUrl { get; set; }

        [PropertyOrder(3)]
        [DisplayName("API Key")]
        public string APIKey { get; set; }
        [PropertyOrder(4)]
        [DisplayName("API Secret")]
        public string APISecret { get; set; }

        [PropertyOrder(5)]
        [DisplayName("Set as default")]
        public bool IsDefault { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Name))
                return Name + (IsDefault ? " (Default)" : "");
            if (!string.IsNullOrWhiteSpace(ServerUrl))
                return ServerUrl + (IsDefault ? " (Default)" : "");
            return "New Chat Server Connection";
        }

        public bool IsValid()
        {
            return Utilities.ValidateStrings(ServerUrl, Name);
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
