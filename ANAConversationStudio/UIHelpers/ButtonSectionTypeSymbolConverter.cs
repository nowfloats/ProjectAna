using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Models.Chat.Sections;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ANAConversationStudio.UIHelpers
{
    public class ButtonSectionTypeSymbolConverter : IValueConverter
    {
        //Search the below as 'Segoe MDL2 Assets' in the internet to find how they look.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SectionTypeEnum secType)
            {
                switch (secType)
                {
                    case SectionTypeEnum.Image:
                        return "\uEB9F";
                    case SectionTypeEnum.Text:
                        return "\uEC42"; //Text inside a chat bubble
                    case SectionTypeEnum.Gif:
                        return "\uE913";
                    case SectionTypeEnum.Audio:
                        return "\uE8D6";
                    case SectionTypeEnum.Video:
                        return "\uE20A";
                    case SectionTypeEnum.EmbeddedHtml:
                        return "\uE12B";
                    case SectionTypeEnum.Carousel:
                        return "\uE89F";
                    case SectionTypeEnum.PrintOTP:
                        break;
                    default:
                        break;
                }
            }
            if (value is ButtonTypeEnum buttonType)
            {
                switch (buttonType)
                {
                    case ButtonTypeEnum.OpenUrl:
                        break;
                    case ButtonTypeEnum.GetText:
                        return "\uE13E";
                    case ButtonTypeEnum.GetNumber:
                        return "\uE18F";
                    case ButtonTypeEnum.GetAddress:
                        return "\uE779";
                    case ButtonTypeEnum.GetEmail:
                        return "\uE168";
                    case ButtonTypeEnum.GetPhoneNumber:
                        return "\uE13A";
                    case ButtonTypeEnum.GetItemFromSource:
                        return "\uE14C";
                    case ButtonTypeEnum.GetImage:
                        return "\uE156";
                    case ButtonTypeEnum.GetAudio:
                        return "\uE8D6";
                    case ButtonTypeEnum.GetVideo:
                        return "\uE20A";
                    case ButtonTypeEnum.NextNode:
                        return "\uEA54";
                    case ButtonTypeEnum.DeepLink:
                        break;
                    case ButtonTypeEnum.GetAgent:
                        return "\uE13D";
                    case ButtonTypeEnum.ShowConfirmation:
                        break;
                    case ButtonTypeEnum.FetchChatFlow:
                        return "\uE167";

                    case ButtonTypeEnum.GetDateTime:
                        return "\uE163";
                    case ButtonTypeEnum.GetDate:
                        return "\uE163";
                    case ButtonTypeEnum.GetTime:
                        return "\uE121";
                    case ButtonTypeEnum.GetLocation:
                        return "\uE81D";
                    default:
                        break;
                }
                return "\uEBFC"; //Default for buttons, 'Index Finger Clicking on Tab' symbol looks like a click action indicating a button
            }
            if (value is NodeTypeEnum nodeType)
            {
                switch (nodeType)
                {
                    case NodeTypeEnum.ApiCall:
                        return "\uE943";
                    case NodeTypeEnum.Combination:
                        return "\uEC26"; //Group bubble icon
                    case NodeTypeEnum.Card:
                        return "\uE8AE";
                    default:
                        break;
                }
            }
            return "\uE8F2";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
