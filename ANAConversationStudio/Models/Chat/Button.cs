using ANAConversationStudio.Helpers;
using ANAConversationStudio.UIHelpers;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ANAConversationStudio.Models.Chat
{
	public class Button : RepeatableBaseEntity
	{

		#region Important
		private ButtonTypeEnum _ButtonType = ButtonTypeEnum.NextNode;
		[Category("Important")]
		public ButtonTypeEnum ButtonType
		{
			get { return _ButtonType; }
			set
			{
				if (_ButtonType != value)
				{
					_ButtonType = value;

					FillAlias();
					OnPropertyChanged();
				}
			}
		}
		#endregion

		#region For ButtonType Get[X]
		private string _PlaceholderText;
		[Category("For ButtonType Get[X]")]
		public string PlaceholderText
		{
			get { return _PlaceholderText; }
			set
			{
				if (_PlaceholderText != value)
				{
					_PlaceholderText = value;
					OnPropertyChanged();
				}
			}
		}

		private string _VariableValue;
		[Category("For ButtonType Get[X]")]
		public string VariableValue
		{
			get { return _VariableValue; }
			set
			{
				if (_VariableValue != value)
				{
					_VariableValue = value;
					OnPropertyChanged();
				}
			}
		}

		private string _PrefixText;
		[Category("For ButtonType Get[X]")]
		public string PrefixText
		{
			get { return _PrefixText; }
			set
			{
				if (_PrefixText != value)
				{
					_PrefixText = value;
					OnPropertyChanged();
				}
			}
		}

		private string _PostfixText;
		[Category("For ButtonType Get[X]")]
		public string PostfixText
		{
			get { return _PostfixText; }
			set
			{
				if (_PostfixText != value)
				{
					_PostfixText = value;
					OnPropertyChanged();
				}
			}
		}

		private int _MinLength;
		[Category("For ButtonType GetText")]
		public int MinLength
		{
			get { return _MinLength; }
			set
			{
				if (_MinLength != value)
				{
					_MinLength = value;
					OnPropertyChanged();
				}
			}
		}

		private int _MaxLength;
		[Category("For ButtonType GetText")]
		public int MaxLength
		{
			get { return _MaxLength; }
			set
			{
				if (_MaxLength != value)
				{
					_MaxLength = value;
					OnPropertyChanged();
				}
			}
		}

		private bool _IsMultiLine;
		[Category("For ButtonType GetText")]
		public bool IsMultiLine
		{
			get { return _IsMultiLine; }
			set
			{
				if (_IsMultiLine != value)
				{
					_IsMultiLine = value;
					OnPropertyChanged();
				}
			}
		}

		private string _DefaultText;
		[Category("For ButtonType GetText")]
		public string DefaultText
		{
			get { return _DefaultText; }
			set
			{
				if (_DefaultText != value)
				{
					_DefaultText = value;
					OnPropertyChanged();
				}
			}
		}
		#endregion

		#region Misc
		private bool _PostToChat = true;
		[Category("Misc")]
		public bool PostToChat
		{
			get { return _PostToChat; }
			set
			{
				if (_PostToChat != value)
				{
					_PostToChat = value;
					OnPropertyChanged();
				}
			}
		}

		private string _APIResponseMatchKey;
		[Category("Misc")]
		public string APIResponseMatchKey
		{
			get { return _APIResponseMatchKey; }
			set
			{
				if (_APIResponseMatchKey != value)
				{
					_APIResponseMatchKey = value;
					OnPropertyChanged();
				}
			}
		}

		private string _APIResponseMatchValue;
		[Category("Misc")]
		public string APIResponseMatchValue
		{
			get { return _APIResponseMatchValue; }
			set
			{
				if (_APIResponseMatchValue != value)
				{
					_APIResponseMatchValue = value;
					OnPropertyChanged();
				}
			}
		}

		private string _DeepLinkUrl;
		[Category("Misc")]
		public string DeepLinkUrl
		{
			get { return _DeepLinkUrl; }
			set
			{
				if (_DeepLinkUrl != value)
				{
					_DeepLinkUrl = value;
					OnPropertyChanged();
				}
			}
		}

		private string _Url;
		[Category("Misc")]
		public string Url
		{
			get { return _Url; }
			set
			{
				if (_Url != value)
				{
					_Url = value;
					OnPropertyChanged();
				}
			}
		}

		private int? _BounceTimeout;
		[Category("Misc")]
		public int? BounceTimeout
		{
			get { return _BounceTimeout; }
			set
			{
				if (_BounceTimeout != value)
				{
					_BounceTimeout = value;
					OnPropertyChanged();
				}
			}
		}

		private bool _DefaultButton;
		[Category("Misc")]
		public bool DefaultButton
		{
			get { return _DefaultButton; }
			set
			{
				if (_DefaultButton != value)
				{
					_DefaultButton = value;
					OnPropertyChanged();
				}
			}
		}

		private bool _Hidden;
		[Category("Misc")]
		public bool Hidden
		{
			get { return _Hidden; }
			set
			{
				if (_Hidden != value)
				{
					_Hidden = value;
					OnPropertyChanged();
				}
			}
		}

		private string _NextNodeId;
		public string NextNodeId
		{
			get { return _NextNodeId; }
			set
			{
				if (_NextNodeId != value)
				{
					_NextNodeId = value;
					OnPropertyChanged();
				}
			}
		}
		#endregion

		//Content
		private string _ButtonText;
		[JsonIgnore]
		public string ButtonText
		{
			get { return _ButtonText; }
			set
			{
				if (_ButtonText != value)
				{
					_ButtonText = value;

					FillAlias();
					OnPropertyChanged();
				}
			}
		}

		private string _ButtonName;
		[JsonIgnore]
		public string ButtonName
		{
			get { return _ButtonName; }
			set
			{
				if (_ButtonName != value)
				{
					_ButtonName = value;

					FillAlias();
					OnPropertyChanged();
				}
			}
		}

		[JsonIgnore]
		[BsonIgnore]
		public IEnumerable<ButtonTypeEnum> ButtonTypes => Enum.GetValues(typeof(ButtonTypeEnum)).Cast<ButtonTypeEnum>();

		private ChatNode _ParentNode;
		[JsonIgnore]
		[BsonIgnore]
		public ChatNode ParentNode
		{
			get { return _ParentNode; }
			set
			{
				if (_ParentNode != value)
				{
					_ParentNode = value;
					OnPropertyChanged();
				}
			}
		}

		private bool? _AdvancedOptions;
		[JsonIgnore]
		[BsonIgnore]
		public bool? AdvancedOptions
		{
			get
			{
				if (_AdvancedOptions == null)
				{
					if (!string.IsNullOrWhiteSpace(VariableValue) || !string.IsNullOrWhiteSpace(APIResponseMatchKey) || !string.IsNullOrWhiteSpace(APIResponseMatchValue) || !string.IsNullOrWhiteSpace(PrefixText) || !string.IsNullOrWhiteSpace(PostfixText) || MinLength > 0 || MaxLength > 0 || IsMultiLine || !string.IsNullOrWhiteSpace(DefaultText))
						_AdvancedOptions = true;
					else
						_AdvancedOptions = false;
				}
				return _AdvancedOptions;
			}
			set
			{
				if (_AdvancedOptions != value)
				{
					_AdvancedOptions = value;
					OnPropertyChanged();
				}
			}
		}

		[JsonIgnore]
		[BsonIgnore]
		public ICommand Remove => new ActionCommand((p) => ParentNode.Buttons.Remove(this));

		[JsonIgnore]
		[BsonIgnore]
		public ICommand MoveUp => new ActionCommand((p) =>
		{
			var oldIdx = ParentNode.Buttons.IndexOf(this);
			if (oldIdx <= 0) return;

			ParentNode.Buttons.Move(oldIdx, oldIdx - 1);
		});

		[JsonIgnore]
		[BsonIgnore]
		public ICommand MoveDown => new ActionCommand((p) =>
		{
			var oldIdx = ParentNode.Buttons.IndexOf(this);
			if (oldIdx >= ParentNode.Buttons.Count - 1) return;

			ParentNode.Buttons.Move(oldIdx, oldIdx + 1);
		});

		[JsonIgnore]
		[BsonIgnore]
		public string ContentId { get; set; }

		[JsonIgnore]
		[BsonIgnore]
		public string ContentEmotion { get; set; }

		[JsonIgnore]
		[BsonIgnore]
		public List<ProjectOptions> ProjectUrls
		{
			get
			{
				return StudioContext.Current.ChatFlowProjects.Select(x => new ProjectOptions
				{
					Name = x.Name,
					Url = StudioContext.GetProjectUrl(x)
				}).ToList();
			}
		}

		protected override void FillAlias()
		{
			Alias = Utilities.TrimText(string.IsNullOrWhiteSpace(ButtonName) ? (string.IsNullOrWhiteSpace(ButtonText) ? ButtonType + "" : ButtonText) : ButtonName, 12);
		}

		public override string ToString()
		{
			return Alias;
		}
	}

	public class ProjectOptions
	{
		public string Name { get; set; }
		public string Url { get; set; }
	}

	public enum ButtonTypeEnum
	{
		OpenUrl,
		GetText,
		GetNumber,
		GetAddress,
		GetEmail,
		GetPhoneNumber,
		GetItemFromSource,
		GetImage,
		GetAudio,
		GetFile,
		GetVideo,
		NextNode,
		DeepLink,
		GetAgent,
		ShowConfirmation,
		FetchChatFlow,
		/// <summary>
		/// Format: yyyy-MM-dd
		/// </summary>
		GetDate,
		/// <summary>
		/// Format: HH:mm:ss
		/// </summary>
		GetTime,
		/// <summary>
		/// Format: yyyy-MM-ddTHH:mm:ss
		/// </summary>
		GetDateTime,
		/// <summary>
		/// Format: [Latitude],[Longitude]
		/// </summary>
		GetLocation
	}
}