using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Models.Chat.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using ANAConversationStudio.Helpers;

namespace ANAConversationStudio.UIHelpers
{
	public class ButtonTypeToFieldVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var fieldName = parameter as string;
			var buttonType = (ButtonTypeEnum)value;
			var hidden = false;

			switch (buttonType)
			{
				case ButtonTypeEnum.PostText:
					hidden = true; //Hide all. Probably!
					break;
				case ButtonTypeEnum.OpenUrl:
					hidden = !(new[] { nameof(Button.Url) }.Contains(fieldName));//Show only Url field
					break;
				case ButtonTypeEnum.GetText:
				case ButtonTypeEnum.GetNumber:
				case ButtonTypeEnum.GetAddress:
				case ButtonTypeEnum.GetEmail:
				case ButtonTypeEnum.GetPhoneNumber:
					//if the passed button property is present in the list, that field should not be visible. here placeholder text should not be visible if button type is input(Get[X]) type
					hidden = new[] { nameof(Button.NextNodeId), nameof(Button.DeepLinkUrl), nameof(Button.Url), nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue) }.Contains(fieldName);
					break;
				case ButtonTypeEnum.GetTime:
				case ButtonTypeEnum.GetDate:
				case ButtonTypeEnum.GetDateTime:
				case ButtonTypeEnum.GetLocation:
					hidden = new[] { nameof(Button.NextNodeId), nameof(Button.DeepLinkUrl), nameof(Button.Url), nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue), nameof(Button.PostfixText), nameof(Button.PrefixText) }.Contains(fieldName);
					break;
				case ButtonTypeEnum.GetImage:
				case ButtonTypeEnum.GetAudio:
				case ButtonTypeEnum.GetFile:
				case ButtonTypeEnum.GetVideo:
					//if the passed button property is present in the list, that field should not be visible. here placeholder text should not be visible if button type is input(Get[X]) type
					hidden = new[] { nameof(Button.NextNodeId), nameof(Button.DeepLinkUrl), nameof(Button.PlaceholderText), nameof(Button.Url), nameof(Button.PostfixText), nameof(Button.PrefixText), nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue) }.Contains(fieldName);
					break;
				case ButtonTypeEnum.GetItemFromSource:
					hidden = new[] { nameof(Button.NextNodeId), nameof(Button.DeepLinkUrl), nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue) }.Contains(fieldName);
					break;
				case ButtonTypeEnum.NextNode:
					hidden = new[] { nameof(Button.NextNodeId), nameof(Button.PostfixText), nameof(Button.PrefixText), nameof(Button.DeepLinkUrl), nameof(Button.Url), nameof(Button.PlaceholderText), }.Contains(fieldName);
					break;
				case ButtonTypeEnum.DeepLink:
					hidden = new[] { nameof(Button.NextNodeId), nameof(Button.Url), nameof(Button.PostfixText), nameof(Button.PrefixText), nameof(Button.PlaceholderText), nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue) }.Contains(fieldName);
					break;
				case ButtonTypeEnum.GetAgent:
					hidden = true; //Hide all. Probably!
					break;
				case ButtonTypeEnum.ShowConfirmation:
					hidden = true; //Hide all. Probably!
					break;
				case ButtonTypeEnum.FetchChatFlow:
					hidden = new[] { nameof(Button.DeepLinkUrl), nameof(Button.PlaceholderText), nameof(Button.PostfixText), nameof(Button.PrefixText), nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue) }.Contains(fieldName);
					break;
				default:
					break;
			}

			return hidden ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}
