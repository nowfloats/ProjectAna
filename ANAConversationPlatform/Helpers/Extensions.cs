using ANAConversationPlatform.Models;
using ANAConversationPlatform.Models.Sections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ANAConversationPlatform.Helpers
{
    public static class Extensions
    {
        public static SectionContent GetFor(this IEnumerable<BaseContent> contentCollection, Section section)
        {
            if (string.IsNullOrWhiteSpace(section._id)) return null;
            return contentCollection.FirstOrDefault(x => x is SectionContent && !string.IsNullOrWhiteSpace((x as SectionContent).SectionId) && (x as SectionContent).SectionId == section._id) as SectionContent;
        }
        public static CarouselItemContent GetFor(this IEnumerable<BaseContent> contentCollection, CarouselItem carouselItem)
        {
            if (string.IsNullOrWhiteSpace(carouselItem._id)) return null;
            return contentCollection.FirstOrDefault(x => x is CarouselItemContent && !string.IsNullOrWhiteSpace((x as CarouselItemContent).CarouselItemId) && (x as CarouselItemContent).CarouselItemId == carouselItem._id) as CarouselItemContent;
        }
        public static CarouselButtonContent GetFor(this IEnumerable<BaseContent> contentCollection, CarouselButton carouselBtn)
        {
            if (string.IsNullOrWhiteSpace(carouselBtn._id)) return null;
            return contentCollection.FirstOrDefault(x => x is CarouselButtonContent && !string.IsNullOrWhiteSpace((x as CarouselButtonContent).CarouselButtonId) && (x as CarouselButtonContent).CarouselButtonId == carouselBtn._id) as CarouselButtonContent;
        }
        public static ButtonContent GetFor(this IEnumerable<BaseContent> contentCollection, Button btn)
        {
            return contentCollection.FirstOrDefault(x => x is ButtonContent && !string.IsNullOrWhiteSpace((x as ButtonContent).ButtonId) && (x as ButtonContent).ButtonId == btn._id) as ButtonContent;
        }
        public static string GetDescriptionOrDefault(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Where(x => x is DescriptionAttribute).FirstOrDefault() is DescriptionAttribute descAttr)
                return descAttr.Description;
            else
                return value.ToString();
        }
        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
