using ANAConversationStudio.Helpers;
using ANAConversationStudio.UIHelpers;
using Microsoft.Win32;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ANAConversationStudio.Models.Chat.Sections
{
	public class Section : BaseEntity
	{
		public Section()
		{
			FillAlias();
		}

		private SectionTypeEnum _SectionType;
		public SectionTypeEnum SectionType
		{
			get { return _SectionType; }
			set
			{
				if (_SectionType != value)
				{
					_SectionType = value;

					FillAlias();
					OnPropertyChanged();
				}
			}
		}


		private int _DelayInMs;
		public int DelayInMs
		{
			get { return _DelayInMs; }
			set
			{
				if (_DelayInMs != value)
				{
					_DelayInMs = value;
					OnPropertyChanged();
				}
			}
		}

		private bool _Hidden;
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

		protected override void FillAlias()
		{
			Alias = Utilities.TrimText(SectionType + "");
		}


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

		[JsonIgnore]
		[BsonIgnore]
		public ICommand Remove => new ActionCommand((p) => ParentNode.Sections.Remove(this));

		[JsonIgnore]
		[BsonIgnore]
		public ICommand MoveUp => new ActionCommand((p) =>
		{
			var oldIdx = ParentNode.Sections.IndexOf(this);
			if (oldIdx <= 0) return;

			ParentNode.Sections.Move(oldIdx, oldIdx - 1);
		});

		[JsonIgnore]
		[BsonIgnore]
		public ICommand MoveDown => new ActionCommand((p) =>
		{
			var oldIdx = ParentNode.Sections.IndexOf(this);
			if (oldIdx >= ParentNode.Sections.Count - 1) return;

			ParentNode.Sections.Move(oldIdx, oldIdx + 1);
		});


		[JsonIgnore]
		[BsonIgnore]
		public string ContentId { get; set; }

		[JsonIgnore]
		[BsonIgnore]
		public string ContentEmotion { get; set; }
	}

	public class TitleCaptionSection : Section
	{
		//Content
		private string _Title;
		public string Title
		{
			get { return _Title; }
			set
			{
				if (_Title != value)
				{
					_Title = value;

					FillAlias();
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

					FillAlias();
					OnPropertyChanged();
				}
			}
		}

		protected override void FillAlias()
		{
			var text = "";
			if (!string.IsNullOrWhiteSpace(Title))
				text = Title;
			else if (!string.IsNullOrWhiteSpace(Caption))
				text = Caption;
			else
				text = SectionType + "";

			Alias = Utilities.TrimText(text);
		}
	}
	public class TitleCaptionUrlSection : TitleCaptionSection
	{
		private string _Url;
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
		public ICommand UploadMedia => new ActionCommand((s) =>
		{
			MessageBox.Show("This feature is coming back soon!", "Feature unavailable");
#if false
			OpenFileDialog ofd = new OpenFileDialog()
			{
				Title = "Please choose a media file to upload",
				CheckFileExists = true,
				CheckPathExists = true,
				Multiselect = false,
				ValidateNames = true,
			};

			switch (SectionType)
			{
				case SectionTypeEnum.Image:
					ofd.Filter = "Image Files (*.jpg, *.jpeg, *.bmp, *.png) | *.jpg; *.jpeg; *.bmp; *.png |All Files (*.*) | *.*";
					break;
				case SectionTypeEnum.Gif:
					ofd.Filter = "GIF Files (*.gif) | *.gif |All Files (*.*) | *.*";
					break;
				case SectionTypeEnum.Audio:
					ofd.Filter = "Audio Files (*.mp3, *.wav, *.wma, *.ogg) | *.mp3; *.wav; *.wma; *.ogg |All Files (*.*) | *.*";
					break;
				case SectionTypeEnum.Video:
					ofd.Filter = "Video Files (*.mp4, *.avi, *.wmv) | *.mp4; *.avi; *.wmv |All Files (*.*) | *.*";
					break;
				default:
					{
						System.Windows.MessageBox.Show("Media upload can only be done for Image, GIF, Video, Audio section types");
						return;
					}
			}

			var done = ofd.ShowDialog();
			if (done == true)
			{
				try
				{
					UploadProgress = "Uploading...";
					var upload = await StudioContext.Current.UploadFile(ofd.FileName);
					if (upload != null && !string.IsNullOrWhiteSpace(upload.Url))
					{
						Url = upload.Url;
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
#endif
		});
	}
	public enum SectionTypeEnum
	{
		Image, Text, Gif, Audio, Video, EmbeddedHtml, Carousel, PrintOTP
	};
}