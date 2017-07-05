using System.Collections.Generic;

namespace ANAConversationPlatform.Models.Sections
{
    public class CarouselSection : Section
    {
        public CarouselSection()
        {
            SectionType = SectionTypeEnum.Carousel;
        }
        public string Title { get; set; }
        public string Caption { get; set; }
        public List<CarouselItem> Items { get; set; } = new List<CarouselItem>();
    }

    public class CarouselItem : BaseEntity
    {
        public string Title { get; set; }
        public string Caption { get; set; }
        public string ImageUrl { get; set; }
        public List<CarouselButton> Buttons { get; set; } = new List<CarouselButton>();
    }

    public class CarouselButton : BaseEntity
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public CardButtonType Type { get; set; }
        public string VariableValue { get; set; }
        public string NextNodeId { get; set; }
    }

    public enum CardButtonType
    {
        NextNode,
        DeepLink,
        OpenUrl
    }
}
