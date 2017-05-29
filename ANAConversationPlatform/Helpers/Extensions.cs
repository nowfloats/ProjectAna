using ANAConversationPlatform.Models;
using ANAConversationPlatform.Models.Sections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ANAConversationPlatform.Helpers
{
    public static class Extensions
    {
        public static Content GetFor(this List<Content> contentCollection, ChatNode node)
        {
            if (string.IsNullOrWhiteSpace(node.Id)) return null;
            return contentCollection.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.NodeId) && x.NodeId == node.Id);
        }

        public static Content GetFor(this List<Content> contentCollection, Section section)
        {
            if (string.IsNullOrWhiteSpace(section._id)) return null;
            return contentCollection.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.SectionId) && x.SectionId == section._id);
        }

        public static Content GetFor(this List<Content> contentCollection, Button btn)
        {
            if (string.IsNullOrWhiteSpace(btn._id)) return null;
            return contentCollection.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.ButtonId) && x.ButtonId == btn._id);
        }
        public static Content GetFor(this List<Content> contentCollection, Coordinates coordiates)
        {
            if (string.IsNullOrWhiteSpace(coordiates.CoordinateListId)) return null;
            return contentCollection.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.ButtonId) && x.ButtonId == coordiates.CoordinateListId);
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
