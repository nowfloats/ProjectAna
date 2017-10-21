using MongoDB.Bson;
using ANAConversationStudio.Models.Chat.Sections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Windows.Input;
using ANAConversationStudio.UIHelpers;
using System.Windows;
using ANAConversationStudio.Helpers;

namespace ANAConversationStudio.Models.Chat
{
	public class ChatNode : INotifyPropertyChanged
	{
		public ChatNode()
		{
			FillAlias();
		}

		#region Important
		private string _Name;
		[Category("Important")]
		public string Name
		{
			get { return _Name; }
			set
			{
				if (_Name != value)
				{
					_Name = value;
					FillAlias();
					OnPropertyChanged();
				}
			}
		}

		private NodeTypeEnum _NodeType = NodeTypeEnum.Combination;
		[Category("Important")]
		public NodeTypeEnum NodeType
		{
			get { return _NodeType; }
			set
			{
				if (_NodeType != value)
				{
					_NodeType = value;
					FillAlias();
					OnPropertyChanged();
				}
			}
		}

		private ObservableCollection<Section> _Sections = new ObservableCollection<Section>();
		[Category("Important")]
		public ObservableCollection<Section> Sections
		{
			get { return _Sections; }
			set
			{
				if (_Sections != value)
				{
					_Sections = value;
					OnPropertyChanged();
				}
			}
		}

		private ObservableCollection<Button> _Buttons = new ObservableCollection<Button>();
		[Category("Important")]
		public ObservableCollection<Button> Buttons
		{
			get { return _Buttons; }
			set
			{
				if (_Buttons != value)
				{
					_Buttons = value;
					OnPropertyChanged();
				}
			}
		}
		#endregion

		#region Misc
		private string _Id;
		[Category("Misc")]
		public string Id
		{
			get { return _Id; }
			set
			{
				if (_Id != value)
				{
					_Id = value;
					OnPropertyChanged();
				}
			}
		}

		private bool _IsStartNode;
		[Category("Misc")]
		public bool IsStartNode
		{
			get { return _IsStartNode; }
			set
			{
				if (_IsStartNode != value)
				{
					_IsStartNode = value;
					OnPropertyChanged();
				}
			}
		}

		private int _TimeoutInMs;
		[Category("Misc")]
		public int TimeoutInMs
		{
			get { return _TimeoutInMs; }
			set
			{
				if (_TimeoutInMs != value)
				{
					_TimeoutInMs = value;
					OnPropertyChanged();
				}
			}
		}

		private string _GroupName;
		[Category("Misc")]
		public string GroupName
		{
			get { return _GroupName; }
			set
			{
				if (_GroupName != value)
				{
					_GroupName = value;
					OnPropertyChanged();
				}
			}
		}
		#endregion

		#region For NodeType ApiCall
		private string _VariableName;
		[Category("For NodeType ApiCall")]
		public string VariableName
		{
			get { return _VariableName; }
			set
			{
				if (_VariableName != value)
				{
					_VariableName = value;
					OnPropertyChanged();
				}
			}
		}

		private ObservableCollection<string> _RequiredVariables;
		[Category("For NodeType ApiCall")]
		public ObservableCollection<string> RequiredVariables
		{
			get { return _RequiredVariables; }
			set
			{
				if (_RequiredVariables != value)
				{
					_RequiredVariables = value;
					OnPropertyChanged();
				}
			}
		}

		private ApiMethodEnum? _ApiMethod;
		[Category("For NodeType ApiCall")]
		public ApiMethodEnum? ApiMethod
		{
			get { return _ApiMethod; }
			set
			{
				if (_ApiMethod != value)
				{
					_ApiMethod = value;
					OnPropertyChanged();
				}
			}
		}

		private string _ApiUrl;
		[Category("For NodeType ApiCall")]
		public string ApiUrl
		{
			get { return _ApiUrl; }
			set
			{
				if (_ApiUrl != value)
				{
					_ApiUrl = value;
					OnPropertyChanged();
				}
			}
		}

		private string _ApiResponseDataRoot;
		[Category("For NodeType ApiCall")]
		public string ApiResponseDataRoot
		{
			get { return _ApiResponseDataRoot; }
			set
			{
				if (_ApiResponseDataRoot != value)
				{
					_ApiResponseDataRoot = value;
					OnPropertyChanged();
				}
			}
		}

		private string _NextNodeId;
		[Category("For NodeType ApiCall")]
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

		#region For NodeType Card
		private string _CardHeader;
		[Category("For NodeType Card")]
		[PropertyOrder(11)]
		public string CardHeader
		{
			get { return _CardHeader; }
			set
			{
				if (_CardHeader != value)
				{
					_CardHeader = value;
					OnPropertyChanged();
				}
			}
		}

		private string _CardFooter;
		[Category("For NodeType Card")]
		[PropertyOrder(12)]
		public string CardFooter
		{
			get { return _CardFooter; }
			set
			{
				if (_CardFooter != value)
				{
					_CardFooter = value;
					OnPropertyChanged();
				}
			}
		}

		private Placement? _Placement;
		[Category("For NodeType Card")]
		[PropertyOrder(13)]
		public Placement? Placement
		{
			get { return _Placement; }
			set
			{
				if (_Placement != value)
				{
					_Placement = value;
					OnPropertyChanged();
				}
			}
		}
		#endregion

		private string _Alias;
		[JsonIgnore]
		public string Alias
		{
			get { return _Alias; }
			set
			{
				if (_Alias != value)
				{
					_Alias = value;
					OnPropertyChanged();
				}
			}
		}

		[JsonIgnore]
		[BsonIgnore]
		public ICommand AddSection => new ActionCommand((param) =>
		{
			var type = (SectionTypeEnum)param;
			switch (type)
			{
				case SectionTypeEnum.Image:
					Sections.Add(new ImageSection
					{
						//Alias = "New Image",
						//Caption = "Image Caption",
						//Title = "Image Title",
						//Url = "http://",
						_id = ObjectId.GenerateNewId().ToString(),
						ParentNode = this,
						ContentId = ObjectId.GenerateNewId().ToString(),
					});
					break;
				case SectionTypeEnum.Text:
					Sections.Add(new TextSection
					{
						//Alias = "New Text",
						//Text = "Text Section",
						_id = ObjectId.GenerateNewId().ToString(),
						ParentNode = this,
						ContentId = ObjectId.GenerateNewId().ToString(),
					});
					break;
				case SectionTypeEnum.Gif:
					Sections.Add(new GifSection
					{
						//Alias = "New Gif",
						//Caption = "Gif Caption",
						//Title = "Gif Title",
						//Url = "http://",
						_id = ObjectId.GenerateNewId().ToString(),
						ParentNode = this,
						ContentId = ObjectId.GenerateNewId().ToString(),
					});
					break;
				case SectionTypeEnum.Audio:
					Sections.Add(new AudioSection
					{
						//Alias = "New Audio",
						//Caption = "Audio Caption",
						//Title = "Audio Title",
						//Url = "http://",
						_id = ObjectId.GenerateNewId().ToString(),
						ParentNode = this,
						ContentId = ObjectId.GenerateNewId().ToString(),
					});
					break;
				case SectionTypeEnum.Video:
					Sections.Add(new VideoSection
					{
						//Alias = "New Video",
						//Caption = "Video Caption",
						//Title = "Video Title",
						//Url = "http://",
						_id = ObjectId.GenerateNewId().ToString(),
						ParentNode = this,
						ContentId = ObjectId.GenerateNewId().ToString(),
					});
					break;
				case SectionTypeEnum.EmbeddedHtml:
					Sections.Add(new EmbeddedHtmlSection
					{
						//Alias = "New Html",
						//Caption = "HTML Caption",
						//Title = "HTML Title",
						//Url = "http://",
						_id = ObjectId.GenerateNewId().ToString(),
						ParentNode = this,
						ContentId = ObjectId.GenerateNewId().ToString(),
					});
					break;
				case SectionTypeEnum.Carousel:
					{
						var carSec = new CarouselSection
						{
							_id = ObjectId.GenerateNewId().ToString(),
							//Alias = "New Carousel",
							//Title = "Carousel Title",
							//Caption = "Carousel Caption",
							ParentNode = this,
							ContentId = ObjectId.GenerateNewId().ToString(),
						};
						var carItem = new CarouselItem
						{
							//Title = "Carousel Item Title",
							//Caption = "Carousel Item Caption",
							//ImageUrl = "http://",
							ParentCarouselSection = carSec,
							_id = ObjectId.GenerateNewId().ToString(),
							ContentId = ObjectId.GenerateNewId().ToString(),
						};
						carItem.Buttons = new ObservableCollection<CarouselButton>
						{
							new CarouselButton
							{
								ContentId = ObjectId.GenerateNewId().ToString(),
								ParentCarouselItem = carItem,
                                //Text = "Carousel Button Text",
                                _id = ObjectId.GenerateNewId().ToString(),
							}
						};
						carSec.Items = new ObservableCollection<CarouselItem> { carItem };
						Sections.Add(carSec);
					}
					break;
				default:
					MessageBox.Show($"Section Type: '{type}' not yet supported!");
					break;
			}
		});

		[JsonIgnore]
		[BsonIgnore]
		public ICommand AddButton => new ActionCommand((param) =>
		{
			Buttons.Add(new Button
			{
				ParentNode = this,
				_id = ObjectId.GenerateNewId().ToString()
			});
		});

		private void FillAlias()
		{
			Alias = Utilities.TrimText(string.IsNullOrWhiteSpace(Name) ? NodeType + "" : Name);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName]string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		public override string ToString()
		{
			return Name;
		}
	}

	public enum ApiMethodEnum
	{
		GET, POST, PUT, HEAD, DELETE, OPTIONS, CONNECT
	}

	public enum NodeTypeEnum
	{
		ApiCall, Combination, Card
	};

	public enum EmotionEnum
	{
		Cool, Happy, Excited, Neutral, Sad, Irritated, Angry
	};

	public enum Placement
	{
		Incoming, Outgoing, Center
	}
}