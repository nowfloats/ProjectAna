using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ANAConversationStudio.Helpers
{
	public class Settings
	{
		const string SettingsFile = "Settings";
		const string SettingsFileExtention = ".json";
		private static string ANAStudioAppDataDirectory
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
				var dir = Path.Combine(ANAStudioAppDataDirectory, "SettingsBackups");
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);
				return dir;
			}
		}
		public static string LoggingDirectory
		{
			get
			{
				var dir = Path.Combine(ANAStudioAppDataDirectory, "Logs");
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);
				return dir;
			}
		}
		private static string SettingsJsonPath => Path.Combine(ANAStudioAppDataDirectory, SettingsFile + SettingsFileExtention);

		private static string _ANADocumentsDirectory;
		public static string ANADocumentsDirectory
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_ANADocumentsDirectory))
				{
					_ANADocumentsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ANA Projects");
					if (!Directory.Exists(_ANADocumentsDirectory))
						Directory.CreateDirectory(_ANADocumentsDirectory);
				}
				return _ANADocumentsDirectory;
			}
		}

		public List<string> RecentChatFlowFiles { get; set; } = new List<string>();
		public List<PublishServer> PublishServers { get; set; } = new List<PublishServer>();
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

	public class EditableSettings
	{
		public string StudioUpdateUrl { get; set; }
	}

	public class AutoUpdateResponse
	{
		public string DownloadLink { get; set; }
		public Version Version { get; set; }
	}

	public class PublishServer
	{
		[PropertyOrder(1)]
		public string Name { get; set; }
		[PropertyOrder(2)]
		public string Url { get; set; }
		[PropertyOrder(3)]
		public string Key { get; set; }
		[PropertyOrder(4)]
		public string Secret { get; set; }

		[Browsable(true), PropertyOrder(5), DisplayName("Chat Projects")]
		public List<PublishChatProject> ChatProjects { get; set; } = new List<PublishChatProject>();

		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(Name))
				return Name;
			if (!string.IsNullOrWhiteSpace(Url))
				return Url;
			return "New Publish Server";
		}

		public (bool Valid, string Msg) Validate()
		{
			var basicValid = Utilities.ValidateStrings(Url, Name);
			if (!basicValid)
				return (false, $"In server '{ToString()}', URL/Name must not be empty");
			if (!Uri.TryCreate(Url, UriKind.Absolute, out Uri u))
				return (false, $"In server '{ToString()}', URL is invalid");

			if (ChatProjects != null && ChatProjects.Count > 0)
			{
				var invalidChatProjects = ChatProjects.Where(x => !x.IsValid()).ToList();
				if (invalidChatProjects.Count > 0)
				{
					var msg = $"In server '{ToString()}', the following chat projects are invalid. Please correct them and try to save again.\r\n{string.Join("\r\n", invalidChatProjects.Select(x => "'" + x + "'"))}";
					return (false, msg);
				}
				return (true, "");
			}
			else
				return (true, "");
		}
	}

	public class PublishChatProject
	{
		[DisplayName("Chat Project Id(a.k.a. Business Id)")]
		[Description("Can contain only lower case alphabets, numbers and '-' (hypen)")]
		[PropertyOrder(1)]
		public string Id { get; set; }

		[PropertyOrder(2)]
		public string Name { get; set; }

		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(Name))
				return Name;
			if (!string.IsNullOrWhiteSpace(Id))
				return Id;
			return "New Publish Chat Project";
		}

		public bool IsValid()
		{
			var notEmpty = Utilities.ValidateStrings(Id, Name);
			if (notEmpty)
				return Regex.IsMatch(Id, "^[a-z0-9\\-]*$");
			return false;
		}
	}
}
