namespace ANAConversationSimulator.Models.Chat.Sections
{
    public class GifSection : Section
    {
        public string Url { get; set; }
        public bool PreFetch { get; set; }
        public GifSection(string id, int delayInMs, string url, bool preFetch, string caption) : base(id, SectionTypeEnum.Gif, delayInMs)
        {
            this.Url = url;
            this.PreFetch = preFetch;
            this.Caption = caption;
        }

        public GifSection(string id, int delayInMs = 0) : base(id, SectionTypeEnum.Gif, delayInMs)
        {
        }
        public GifSection() { }
    }
}