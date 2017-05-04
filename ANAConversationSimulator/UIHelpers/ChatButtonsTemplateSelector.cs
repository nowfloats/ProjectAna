using ANAConversationSimulator.Models.Chat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ANAConversationSimulator.UIHelpers
{
    public class ChatButtonsTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is ANAConversationSimulator.Models.Chat.Button button)
            {
                switch (button.ButtonType)
                {
                    case ButtonTypeEnum.PostText:
                    case ButtonTypeEnum.GetText:
                    case ButtonTypeEnum.GetEmail:
                    case ButtonTypeEnum.GetNumber:
                    case ButtonTypeEnum.GetPhoneNumber:
                        {
                            if (!string.IsNullOrWhiteSpace(button.PrefixText) || !string.IsNullOrWhiteSpace(button.PostfixText))
                                return Application.Current.Resources["TextInputWithPreAndPostChatButtonTemplate"] as DataTemplate;
                            else
                                return Application.Current.Resources["TextInputChatButtonTemplate"] as DataTemplate;
                        }
                    case ButtonTypeEnum.GetItemFromSource:
                        {
                            if (!string.IsNullOrWhiteSpace(button.PrefixText) || !string.IsNullOrWhiteSpace(button.PostfixText))
                                return Application.Current.Resources["TextAutoCompleteInputPrePostfixChatButtonTemplate"] as DataTemplate;
                            else
                                return Application.Current.Resources["TextAutoCompleteInputChatButtonTemplate"] as DataTemplate;
                        }
                    #region Default
                    //case ButtonTypeEnum.None:
                    //case ButtonTypeEnum.OpenUrl:
                    //case ButtonTypeEnum.GetImage:
                    //case ButtonTypeEnum.GetAudio:
                    //case ButtonTypeEnum.GetVideo:
                    //case ButtonTypeEnum.NextNode:
                    //case ButtonTypeEnum.DeepLink:
                    //case ButtonTypeEnum.GetAgent:
                    //case ButtonTypeEnum.ApiCall:
                    //    return Application.Current.Resources["ClickChatButtonTemplate"] as DataTemplate; 
                    #endregion
                    default:
                        return Application.Current.Resources["ClickChatButtonTemplate"] as DataTemplate;
                }
            }
            return null;
        }
    }
}
