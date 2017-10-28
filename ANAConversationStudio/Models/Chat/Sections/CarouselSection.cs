using ANAConversationStudio.Controls;
using ANAConversationStudio.Helpers;
using ANAConversationStudio.UIHelpers;
using Microsoft.Win32;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ANAConversationStudio.Models.Chat.Sections
{
	public class CarouselSection : TitleCaptionSection
	{
		public CarouselSection()
		{
			SectionType = SectionTypeEnum.Carousel;
		}


		private ObservableCollection<CarouselItem> _Items = new ObservableCollection<CarouselItem>();
		public ObservableCollection<CarouselItem> Items
		{
			get { return _Items; }
			set
			{
				if (_Items != value)
				{
					_Items = value;
					OnPropertyChanged();
				}
			}
		}

		[JsonIgnore]
		[BsonIgnore]
		public ICommand AddItem => new ActionCommand((p) => Items.Add(new CarouselItem
		{
			ContentId = ObjectId.GenerateNewId().ToString(),
			ParentCarouselSection = this,
			_id = ObjectId.GenerateNewId().ToString(),
			Title = "New Carousel Item"
		}));
	}

	public class CarouselItem : RepeatableBaseEntity
	{
		private string _ImageUrl;
		public string ImageUrl
		{
			get { return _ImageUrl; }
			set
			{
				if (_ImageUrl != value)
				{
					_ImageUrl = value;
					OnPropertyChanged();
				}
			}
		}

		private ObservableCollection<CarouselButton> _Buttons = new ObservableCollection<CarouselButton>();
		public ObservableCollection<CarouselButton> Buttons
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

		private string _Title;
		public string Title
		{
			get { return _Title; }
			set
			{
				if (_Title != value)
				{
					_Title = value;
					OnPropertyChanged();
				}
			}
		}

		private string _Caption;
		public string Caption
		{
			get { return _Caption; }
			set
			{
				if (_Caption != value)
				{
					_Caption = value;
					OnPropertyChanged();
				}
			}
		}

		[JsonIgnore]
		[BsonIgnore]
		public string ContentId { get; set; }


		private CarouselSection _ParentCarouselSection;
		[JsonIgnore]
		[BsonIgnore]
		public CarouselSection ParentCarouselSection
		{
			get { return _ParentCarouselSection; }
			set
			{
				if (_ParentCarouselSection != value)
				{
					_ParentCarouselSection = value;
					OnPropertyChanged();
				}
			}
		}

		[JsonIgnore]
		[BsonIgnore]
		public ICommand Remove => new ActionCommand((p) => ParentCarouselSection.Items.Remove(this));

		[JsonIgnore]
		[BsonIgnore]
		public ICommand MoveUp => new ActionCommand((p) =>
		{
			var oldIdx = ParentCarouselSection.Items.IndexOf(this);
			if (oldIdx <= 0) return;

			ParentCarouselSection.Items.Move(oldIdx, oldIdx - 1);
		});

		[JsonIgnore]
		[BsonIgnore]
		public ICommand MoveDown => new ActionCommand((p) =>
		{
			var oldIdx = ParentCarouselSection.Items.IndexOf(this);
			if (oldIdx >= ParentCarouselSection.Items.Count - 1) return;

			ParentCarouselSection.Items.Move(oldIdx, oldIdx + 1);
		});


		[JsonIgnore]
		[BsonIgnore]
		public ICommand AddButton => new ActionCommand((p) => Buttons.Add(new CarouselButton
		{
			ContentId = ObjectId.GenerateNewId().ToString(),
			ParentCarouselItem = this,
			_id = ObjectId.GenerateNewId().ToString()
		}));

		private string _UploadProgress;
		[JsonIgnore]
		[BsonIgnore]
		public string UploadProgress
		{
			get { return _UploadProgress; }
			set
			{
				if (_UploadProgress != value)
				{
					_UploadProgress = value;
					OnPropertyChanged();
				}
			}
		}

		[JsonIgnore]
		[BsonIgnore]
		public ICommand UploadMedia => new ActionCommand(async (s) =>
		{
			var ofd = new OpenFileDialog()
			{
				Title = "Please choose a media file to upload",
				CheckFileExists = true,
				CheckPathExists = true,
				Multiselect = false,
				ValidateNames = true,
				Filter = "Image Files (*.jpg, *.jpeg, *.bmp, *.png, *.gif) | *.jpg; *.jpeg; *.bmp; *.png, *.gif | All Files (*.*) | *.*"
			};

			var done = ofd.ShowDialog();
			if (done == true)
			{
				try
				{
					UploadProgress = "Uploading...";
					var uploadedUrl = await Utilities.FileUploadAsync(ofd.FileName);
					if (uploadedUrl != null && !string.IsNullOrWhiteSpace(uploadedUrl))
					{
						ImageUrl = uploadedUrl;
						UploadProgress = null;
					}
					else
						UploadProgress = "Something went wrong!";
				}
				catch (System.Exception ex)
				{
					UploadProgress = "Unable to upload: " + ex.Message;
				}
			}
		});

		protected override void FillAlias()
		{
			Alias = string.IsNullOrWhiteSpace(Title) ? (string.IsNullOrWhiteSpace(Caption) ? "Carousel Item" : Caption) : Title;
		}
	}

	public class CarouselButton : RepeatableBaseEntity
	{
		public string Url { get; set; }
		public CardButtonType Type { get; set; }
		public string VariableValue { get; set; }
		public string NextNodeId { get; set; }

		private string _Text;
		public string Text
		{
			get { return _Text; }
			set
			{
				if (_Text != value)
				{
					_Text = value;
					OnPropertyChanged();
				}
			}
		}

		[JsonIgnore]
		[BsonIgnore]
		public string ContentId { get; set; }

		[JsonIgnore]
		[BsonIgnore]
		public IEnumerable<CardButtonType> Types => Enum.GetValues(typeof(CardButtonType)).Cast<CardButtonType>();


		private CarouselItem _ParentCarouselItem;
		[JsonIgnore]
		[BsonIgnore]
		public CarouselItem ParentCarouselItem
		{
			get { return _ParentCarouselItem; }
			set
			{
				if (_ParentCarouselItem != value)
				{
					_ParentCarouselItem = value;
					OnPropertyChanged();
				}
			}
		}

		[JsonIgnore]
		[BsonIgnore]
		public ICommand Remove => new ActionCommand((p) => ParentCarouselItem.Buttons.Remove(this));

		[JsonIgnore]
		[BsonIgnore]
		public ICommand MoveUp => new ActionCommand((p) =>
		{
			var oldIdx = ParentCarouselItem.Buttons.IndexOf(this);
			if (oldIdx <= 0) return;

			ParentCarouselItem.Buttons.Move(oldIdx, oldIdx - 1);
		});

		[JsonIgnore]
		[BsonIgnore]
		public ICommand MoveDown => new ActionCommand((p) =>
		{
			var oldIdx = ParentCarouselItem.Buttons.IndexOf(this);
			if (oldIdx >= ParentCarouselItem.Buttons.Count - 1) return;

			ParentCarouselItem.Buttons.Move(oldIdx, oldIdx + 1);
		});

		protected override void FillAlias()
		{
			Alias = (string.IsNullOrWhiteSpace(Text) ? "Carousel Button" : Text);
		}
	}

	public enum CardButtonType
	{
		NextNode,
		DeepLink,
		OpenUrl
	}
}