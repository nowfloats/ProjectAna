namespace ANAConversationPlatform.Models.Sections
{
    public class AudioSection : Section
	{
		public AudioSection(string id, int delayInMs, string title, string caption, int durationInSeconds, AudioFormatEnum format, string url, string encodingType, bool downloadable, bool buffer, bool animateEmotion) : base(id, SectionTypeEnum.Audio, delayInMs)
		{
			this.Title = title;
			this.Caption = caption;
			this.DurationInSeconds = durationInSeconds;
			this.Format = format;
			this.Url = url;
			this.EncodingType = encodingType;
			this.Downloadable = downloadable;
			this.Buffer = buffer;
			this.AnimateEmotion = animateEmotion;
		}

		public AudioSection(string id, int delayInMs = 0) : base(id, SectionTypeEnum.Audio, delayInMs)
		{
		}

		public string Title { get; set; }
		public string Caption { get; set; } = null;
		public int DurationInSeconds { get; set; }
		public AudioFormatEnum Format { get; set; }
		public string Url { get; set; }
		public string EncodingType { get; set; } = null;
		public bool Downloadable { get; set; } = false;
		public bool Buffer { get; set; } = false;
		public bool AnimateEmotion { get; set; } = false;
	}

	public enum AudioFormatEnum
	{
		Mp3, Wav, Ogg
	}
}