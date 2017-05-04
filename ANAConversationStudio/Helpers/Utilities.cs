using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using ANAConversationStudio.Models;
using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Models.Chat.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using Serilog;
using System.Runtime.CompilerServices;
using System.IO;

namespace ANAConversationStudio.Helpers
{
    public static class Utilities
    {
        public static Settings Settings { get; set; }
        static Utilities()
        {
            Settings = Settings.Load();
        }

        public static BaseContent GetContentObject(object contentOwner)
        {
            BaseContent contentObj = null;
            if (contentOwner is TextSection)
                contentObj = new TextSectionContent();
            else if (contentOwner is ImageSection)
                contentObj = new ImageSectionContent();
            else if (contentOwner is AudioSection || contentOwner is VideoSection || contentOwner is EmbeddedHtmlSection || contentOwner is GifSection)
                contentObj = new TitleCaptionSectionContent();
            else if (contentOwner is ChatNode)
                contentObj = new NodeContent();
            else if (contentOwner is Button)
                contentObj = new ButtonContent();
            return contentObj;
        }
        public static IEnumerable<BaseContent> GetContentBank(object choosenOwner)
        {
            if (choosenOwner is ChatNode)
            {
                var node = choosenOwner as ChatNode;
                return MongoHelper.Current.Contents.Where(x => x is NodeContent).Cast<NodeContent>().Where(x => x.NodeId == node.Id).ToList();
            }
            if (choosenOwner is Section)
            {
                var section = choosenOwner as Section;
                return MongoHelper.Current.Contents.Where(x => x is SectionContent).Cast<SectionContent>().Where(x => x.SectionId == section._id).ToList();
            }
            if (choosenOwner is Button)
            {
                var btn = choosenOwner as Button;
                return MongoHelper.Current.Contents.Where(x => x is ButtonContent).Cast<ButtonContent>().Where(x => x.ButtonId == btn._id).ToList();
            }
            return new List<BaseContent>();
        }
        public static void UpdateContentBank(IEnumerable<BaseContent> editedContentEntries)
        {
            if (editedContentEntries != null && editedContentEntries.Count() > 0)
            {
                MongoHelper.Current.Contents.RemoveAll(x => editedContentEntries.Select(y => y._id).Contains(x._id));
                MongoHelper.Current.Contents.AddRange(editedContentEntries);
            }
        }
        public static BaseContent GetContentObjectV2(object contentOwner)
        {
            BaseContent contentObj = null;
            if (contentOwner is Section sec)
            {
                switch (sec.SectionType)
                {
                    case SectionTypeEnum.Image:
                        contentObj = new ImageSectionContent();
                        break;
                    case SectionTypeEnum.Text:
                        contentObj = new TextSectionContent();
                        break;
                    default:
                        contentObj = new TitleCaptionSectionContent();
                        break;
                }
            }
            else if (contentOwner is ChatNode)
                contentObj = new NodeContent();
            else if (contentOwner is Button)
                contentObj = new ButtonContent();
            return contentObj;

        }

        static Random rand = new Random();
        public static int Rand()
        {
            return rand.Next(700, 1000);
        }

        public static bool IsDesignMode()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }

        public static bool ValidateStrings(params string[] strings)
        {
            foreach (var item in strings)
                if (string.IsNullOrWhiteSpace(item)) return false;
            return true;
        }
    }

    public static class Exts
    {
        public static T DeepCopy<T>(this T source)
        {
            var json = source.ToJson();
            return BsonSerializer.Deserialize<T>(json);
        }
    }
    public static class Logger
    {
        static Logger()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.RollingFile("Logs\\Log-{Date}.log").CreateLogger();
        }

        public static void Write(Exception ex, [CallerMemberName] string caller = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = null)
        {
            Log.Error(ex, $"Ex in File: {Path.GetFileName(fileName ?? "")}->Line: {lineNumber}->Method: {caller}");
        }
        public static void Write(string message)
        {
            Log.Debug(message);
        }
    }
    public static class Constants
    {
        //TODO: Implement Window Layout save and restore function.
        public const string WindowLayoutFileName = "WindowLayout.xml";
    }
}
