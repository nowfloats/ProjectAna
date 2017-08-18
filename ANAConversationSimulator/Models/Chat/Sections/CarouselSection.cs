using ANAConversationSimulator.Services.ChatInterfaceServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ANAConversationSimulator.Models.Chat.Sections
{
    public class CarouselSection : Section
    {
        public CarouselSection()
        {
            SectionType = SectionTypeEnum.Carousel;
        }
        public List<CarouselItem> Items { get; set; } = new List<CarouselItem>();
    }

    public class CarouselItem : BaseEntity
    {
        public string Title { get; set; }
        public string Caption { get; set; }
        public string ImageUrl { get; set; }
        public List<CarouselButton> Buttons { get; set; } = new List<CarouselButton>();

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
        public string Text { get; set; }
        public string Url { get; set; }
        public CardButtonType Type { get; set; }
        public string VariableValue { get; set; }
        public string NextNodeId { get; set; }

        [JsonIgnore]
        public ICommand Action { get; set; } = new ButtonActionCommand();

        [JsonIgnore]
        public string VariableName { get; set; }
        [JsonIgnore]
        public string NodeId { get; set; }

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