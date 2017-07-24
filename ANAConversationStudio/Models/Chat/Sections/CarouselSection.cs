using ANAConversationStudio.Controls;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class CarouselSection : Section
    {
        public CarouselSection()
        {
            SectionType = SectionTypeEnum.Carousel;
        }

        [Editor(typeof(ChatElementCollectionEditor<CarouselItem>), typeof(ChatElementCollectionEditor<CarouselItem>))]
        public ObservableCollection<CarouselItem> Items { get; set; } = new ObservableCollection<CarouselItem>();
    }

    public class CarouselItem : BaseEntity
    {
        public string ImageUrl { get; set; }

        [Editor(typeof(ChatElementCollectionEditor<CarouselButton>), typeof(ChatElementCollectionEditor<CarouselButton>))]
        public ObservableCollection<CarouselButton> Buttons { get; set; } = new ObservableCollection<CarouselButton>();

        #region Repeat
        public bool DoesRepeat { get; set; }
        public string RepeatOn { get; set; }
        public string RepeatAs { get; set; }
        public int StartPosition { get; set; }
        public int MaxRepeats { get; set; }
        #endregion
    }

    public class CarouselButton : BaseEntity
    {
        public string Url { get; set; }
        public CardButtonType Type { get; set; }
        public string VariableValue { get; set; }
        public string NextNodeId { get; set; }

        #region Repeat
        public bool DoesRepeat { get; set; }
        public string RepeatOn { get; set; }
        public string RepeatAs { get; set; }
        public int StartPosition { get; set; }
        public int MaxRepeats { get; set; }
        #endregion
    }

    public enum CardButtonType
    {
        NextNode,
        DeepLink,
        OpenUrl
    }
}