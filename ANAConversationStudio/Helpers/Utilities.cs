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
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ANAConversationStudio.Helpers
{
    public static class Utilities
    {
        public static Settings Settings { get; set; }
        
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
            if (choosenOwner is ChatNode node)
                return MongoHelper.Current.Contents.Where(x => x is NodeContent).Cast<NodeContent>().Where(x => x.NodeId == node.Id).ToList();

            if (choosenOwner is Section section)
                return MongoHelper.Current.Contents.Where(x => x is SectionContent).Cast<SectionContent>().Where(x => x.SectionId == section._id).ToList();

            if (choosenOwner is Button btn)
                return MongoHelper.Current.Contents.Where(x => x is ButtonContent).Cast<ButtonContent>().Where(x => x.ButtonId == btn._id).ToList();

            if (choosenOwner is CarouselItem cItem)
                return MongoHelper.Current.Contents.Where(x => x is CarouselItemContent).Cast<CarouselItemContent>().Where(x => x.CarouselItemId == cItem._id).ToList();

            if (choosenOwner is CarouselButton cButton)
                return MongoHelper.Current.Contents.Where(x => x is CarouselButtonContent).Cast<CarouselButtonContent>().Where(x => x.CarouselButtonId == cButton._id).ToList();

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
            else if (contentOwner is CarouselItem)
                contentObj = new CarouselItemContent();
            else if (contentOwner is CarouselButton)
                contentObj = new CarouselButtonContent();
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

        public static async Task<AutoUpdateResponse> GetLatestVersionInfo()
        {
            if (Settings.UpdateDetails != null && !string.IsNullOrWhiteSpace(Settings.UpdateDetails.StudioUpdateUrl))
            {
                using (var wc = new WebClient())
                {
                    var resp = await wc.DownloadStringTaskAsync(Settings.UpdateDetails.StudioUpdateUrl);
                    return JsonConvert.DeserializeObject<AutoUpdateResponse>(resp);
                }
            }
            return null;
        }

        public static string Encrypt(string encryptString, string encryptionKey)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        public static string Decrypt(string cipherText, string encryptionKey)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
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
