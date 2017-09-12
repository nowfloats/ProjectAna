namespace ANAConversationPlatform.Models
{
    public class BaseContent : BaseIdTimestampEntity
    {
        public string NodeName { get; set; }
        public string NodeId { get; set; }
        public string Emotion { get; set; }
    }

    public class SectionContent : BaseContent
    {
        public string SectionId { get; set; }
    }

    public class TextSectionContent : SectionContent
    {
        public string SectionText { get; set; }
    }

    public class TitleCaptionSectionContent : SectionContent
    {
        public string Title { get; set; }
        public string Caption { get; set; }
    }
    
    public class ButtonContent : BaseContent
    {
        public string ButtonId { get; set; }
        public string ButtonText { get; set; }
        public string ButtonName { get; set; }
    }

    public class CarouselButtonContent : BaseContent
    {
        public string CarouselButtonId { get; set; }
        public string ButtonText { get; set; }
    }

    public class CarouselItemContent : BaseContent
    {
        public string CarouselItemId { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
    }
}