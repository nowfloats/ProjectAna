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
						return "\uE8D3"; //Text inside a chat bubble
					case SectionTypeEnum.Gif:
						return "\uEB9F";
					case SectionTypeEnum.Audio:
						return "\uE8D6";
					case SectionTypeEnum.Video:
						return "\uE786";
					case SectionTypeEnum.EmbeddedHtml:
						return "\uE12B";
					case SectionTypeEnum.Carousel:
						return "\uE843 \uE843 \uE843";
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
						return "\uE8A7";
					case ButtonTypeEnum.GetText:
						return "\uE8D3";
					case ButtonTypeEnum.GetNumber:
						return "\uE18F";
					case ButtonTypeEnum.GetAddress:
						return "\uE707";
					case ButtonTypeEnum.GetEmail:
						return "\uE715";
					case ButtonTypeEnum.GetPhoneNumber:
						return "\uE717";
					case ButtonTypeEnum.GetItemFromSource:
						return "\uE8B5";
					case ButtonTypeEnum.GetImage:
						return "\uEB9F";
					case ButtonTypeEnum.GetAudio:
						return "\uE8D6";
					case ButtonTypeEnum.GetVideo:
						return "\uE20A";
					case ButtonTypeEnum.NextNode:
						return "\uE761";
					case ButtonTypeEnum.DeepLink:
						return "\uE71B";
					case ButtonTypeEnum.GetAgent:
						return "\uE77B";
					case ButtonTypeEnum.ShowConfirmation:
						return "\uE930";
					case ButtonTypeEnum.FetchChatFlow:
						return "\uEA54";
					case ButtonTypeEnum.GetDateTime:
						return "\uEC92";
					case ButtonTypeEnum.GetDate:
						return "\uE787";
					case ButtonTypeEnum.GetTime:
						return "\uE823";
					case ButtonTypeEnum.GetLocation:
						return "\uE707";
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
					case NodeTypeEnum.HandoffToAgent:
						return "\uE77B";
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
