using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.ViewModels;
using Microsoft.Win32;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ANAConversationStudio.Helpers
{
	public class StudioContext : INotifyPropertyChanged
	{
		#region Private
		/// <summary>
		/// Use to detect unsaved changes
		/// </summary>
		private string LastChatFlowSavedHash { get; set; }

		private StudioContext(string projectFilePath)
		{
			this.ProjectFilePath = projectFilePath;
		}
		private StudioContext() { }

		public (bool, string) LoadChatFlowProject()
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(ProjectFilePath))
				{
					var rawProjectJson = File.ReadAllText(ProjectFilePath);
					ChatFlow = BsonSerializer.Deserialize<ChatFlowPack>(rawProjectJson);
				}
				else
				{
					var firstNode = new ChatNode { Name = "New Node" };
					firstNode.Id = ObjectId.GenerateNewId().ToString();

					ChatFlow = new ChatFlowPack
					{
						ChatNodes = new List<ChatNode>(new[] { firstNode }),
						ChatContent = new List<Models.BaseContent>(),
						NodeLocations = new Dictionary<string, Models.LayoutPoint> { { firstNode.Id, new Models.LayoutPoint { X = 500, Y = 500 } } }
					};
				}
				LastChatFlowSavedHash = Utilities.GenerateHash(ChatFlow.ToJson());
				ChatFlowBuilder.Build(ChatFlow);
				return (true, "");
			}
			catch (Exception ex)
			{
				return (false, ex.Message);
			}
		}
		#endregion

		#region Static

		#region Static Property Changed
		public static event EventHandler StaticPropertyChanged;
		private static void OnStaticPropertyChanged(object sender) => StaticPropertyChanged?.Invoke(sender, new EventArgs());
		#endregion
		private static StudioContext _Current;
		public static StudioContext Current
		{
			get { return _Current; }
			private set
			{
				if (_Current != value)
				{
					_Current = value;

					OnStaticPropertyChanged(_Current);
				}
			}
		}

		public static void ClearCurrent()
		{
			Current = null;
		}
		public static bool IsProjectLoaded(bool showMsg)
		{
			if (Current?.ProjectFilePath == null || Current?.ChatFlow == null)
			{
				if (showMsg)
					MessageBox.Show("No chat flow project is loaded. Please load a chat flow and try again.");
				return false;
			}
			return true;
		}
		public static bool Load(string projectFilePath, MainWindowViewModel viewModelRef)
		{
			var studioContext = new StudioContext(projectFilePath);
			(var done, var msg) = studioContext.LoadChatFlowProject();
			if (!done)
			{
				MessageBox.Show(msg, "Unable to open the project");
				return false;
			}
			Current = studioContext;

			if (!Utilities.Settings.RecentChatFlowFiles.Contains(projectFilePath))
			{
				Utilities.Settings.RecentChatFlowFiles.Insert(0, projectFilePath);
				Utilities.Settings.Save(App.Cryptio);
			}

			if (viewModelRef != null)
				viewModelRef.LoadNodesIntoDesigner();
			return true;
		}
		public static bool LoadNew(MainWindowViewModel viewModelRef)
		{
			var studioContext = new StudioContext();
			(var done, var msg) = studioContext.LoadChatFlowProject();
			if (!done)
			{
				MessageBox.Show(msg, "Unable to create a new project");
				return false;
			}
			Current = studioContext;
			if (viewModelRef != null)
				viewModelRef.LoadNodesIntoDesigner();
			return true;
		}
		public static bool Open(MainWindowViewModel viewModelRef)
		{
			var openFileDialog = new OpenFileDialog
			{
				Filter = "ANA Project Files | *.anaproj",
				CheckFileExists = true,
				Multiselect = false,
				InitialDirectory = Settings.ANADocumentsDirectory,
				Title = "Browse to the ANA Project to load",
				ValidateNames = true
			};
			var done = openFileDialog.ShowDialog();
			if (done == true)
			{
				var loaded = Load(openFileDialog.FileName, viewModelRef);
				if (loaded)
					return true;
			}
			return false;
		}
		private static readonly JsonSerializerSettings publishJsonSettings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore,
			Converters = new List<JsonConverter> { new StringEnumConverter() }
		};
		#endregion

		#region Public

		public string ProjectFilePath { get; set; }

		public string LastChatFlowProjectsSavedHash { get; set; }

		private ChatFlowPack _ChatFlow;
		public ChatFlowPack ChatFlow
		{
			get { return _ChatFlow; }
			set
			{
				if (_ChatFlow != value)
				{
					_ChatFlow = value;
					OnPropertyChanged();
				}
			}
		}

		public string ProjectName
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(ProjectFilePath))
					return Path.GetFileNameWithoutExtension(ProjectFilePath);
				return "Unsaved project";
			}
		}

		public bool AreChatFlowChangesMadeAfterLastSave()
		{
			if (!IsProjectLoaded(false))
				return false;
			if (string.IsNullOrWhiteSpace(LastChatFlowSavedHash))
				return true;

			var currentHash = Utilities.GenerateHash(ChatFlow.ToJson());
			if (LastChatFlowSavedHash == currentHash)
				return false; //no changes

			return true;
		}

		public bool SaveChatFlowProject()
		{
			try
			{
				if (ChatFlow != null)
				{
					if (string.IsNullOrWhiteSpace(ProjectFilePath))
					{
						var saveFileDialog = new SaveFileDialog()
						{
							FileName = "ChatProject.anaproj",
							Filter = "ANA Project Files | *.anaproj",
							AddExtension = true,
							CheckPathExists = true,
							InitialDirectory = Settings.ANADocumentsDirectory,
							OverwritePrompt = true,
							Title = "Specify the ANA Project save location",
							ValidateNames = true
						};
						var done = saveFileDialog.ShowDialog();
						if (done != true)
							return false;
						ProjectFilePath = saveFileDialog.FileName;
					}

					var rawProjectJson = ChatFlow.ToJson();
					File.WriteAllText(ProjectFilePath, rawProjectJson);
					LastChatFlowSavedHash = Utilities.GenerateHash(rawProjectJson);

					if (!Utilities.Settings.RecentChatFlowFiles.Contains(ProjectFilePath))
					{
						Utilities.Settings.RecentChatFlowFiles.Insert(0, ProjectFilePath);
						Utilities.Settings.Save(App.Cryptio);
					}

					return true;
				}
				MessageBox.Show("Chat flow project Not loaded properly!", "Unable to save the flow");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Unable to save the flow");
			}
			return false;
		}

		public string GetCompiledProjectJSON()
		{
			var compiled = ChatFlowBuilder.Build(ChatFlow);
			return JsonConvert.SerializeObject(compiled, publishJsonSettings);
		}

		#region INPC
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName]string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		#endregion

		#endregion
	}

	public class APIResponse
	{
		public bool Status { get; set; }
		public string Message { get; set; }
	}

	public class DataListResponse<T> : APIResponse
	{
		public List<T> Data { get; set; }
	}

	public class DataResponse<T> : APIResponse
	{
		public T Data { get; set; }
	}

	public class UploadFileResponse : APIResponse
	{
		public string Url { get; set; }
	}
}