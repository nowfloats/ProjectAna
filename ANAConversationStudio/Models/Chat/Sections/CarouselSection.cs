using System.Collections.Generic;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class CarouselSection : Section
    {
        public CarouselSection()
        {
            SectionType = SectionTypeEnum.Carousel;
        }
        public List<Carousel> CarouselList { get; set; } = new List<Carousel>();

        public void AddNewCarouselEntry(Carousel carouselEntry)
        {
            if (CarouselList == null)
                CarouselList = new List<Carousel>();
            CarouselList.Add(carouselEntry);
        }

    }

    public class Carousel
    {
        public override string ToString()
        {
            return Title;
        }
        public Carousel() { }
        public Carousel(string title, string imageUrl)
        {
            this.Title = title;
            this.ImageUrl = imageUrl;
        }
        public string Title { get; set; }
        public string ImageUrl { get; set; } = null;
        public List<string> Values { get; set; } = new List<string>();
        public void AddValue(string value)
        {
            if (Values == null)
                Values = new List<string>();
            Values.Add(value);
        }
    }
}