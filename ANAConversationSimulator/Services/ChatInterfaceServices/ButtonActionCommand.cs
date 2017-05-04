using ANAConversationSimulator.Helpers;
using ANAConversationSimulator.UserControls;
using ANAConversationSimulator.Models.Chat;
using System;
using System.Linq;
using System.Windows.Input;

namespace ANAConversationSimulator.Services.ChatInterfaceServices
{
    public class ButtonActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            if (parameter is Button button)
            {
                switch (button.ButtonType)
                {
                    case ButtonTypeEnum.PostText:
                        if (!button.Hidden)
                            ButtonActionHelper.HandlePostTextToThread(button.ButtonText);
                        break;
                    case ButtonTypeEnum.OpenUrl:
                        ButtonActionHelper.HandleOpenUrl(button.Url);
                        break;
                    case ButtonTypeEnum.GetText:
                        if (string.IsNullOrWhiteSpace(button.VariableValue)) return;
                        ButtonActionHelper.HandleSaveTextInput(button.VariableName, button.VariableValue);
                        if (!button.Hidden)
                            ButtonActionHelper.HandlePostTextToThread(button.PrefixText + button.VariableValue + button.PostfixText);
                        break;
                    case ButtonTypeEnum.GetEmail:
                        if (string.IsNullOrWhiteSpace(button.VariableValue)) return;
                        if (!Utils.IsValidEmail(button.VariableValue))
                        {
                            Utils.ShowDialog("Invalid format");
                            return;
                        }
                        ButtonActionHelper.HandleSaveTextInput(button.VariableName, button.VariableValue);
                        if (!button.Hidden)
                            ButtonActionHelper.HandlePostTextToThread(button.PrefixText + button.VariableValue + button.PostfixText);
                        break;
                    case ButtonTypeEnum.GetNumber:
                        if (string.IsNullOrWhiteSpace(button.VariableValue)) return;
                        double d;
                        if (!double.TryParse(button.VariableValue, out d))
                        {
                            Utils.ShowDialog("Invalid format");
                            return;
                        }
                        ButtonActionHelper.HandleSaveTextInput(button.VariableName, d.ToString());
                        if (!button.Hidden)
                            ButtonActionHelper.HandlePostTextToThread(button.PrefixText + d.ToString() + button.PostfixText);
                        break;
                    case ButtonTypeEnum.GetPhoneNumber:
                        if (string.IsNullOrWhiteSpace(button.VariableValue)) return;
                        if (!Utils.IsValidPhoneNumber(button.VariableValue))
                        {
                            Utils.ShowDialog("Invalid format");
                            return;
                        }
                        ButtonActionHelper.HandleSaveTextInput(button.VariableName, button.VariableValue);
                        if (!button.Hidden)
                            ButtonActionHelper.HandlePostTextToThread(button.PrefixText + button.VariableValue + button.PostfixText);
                        break;
                    case ButtonTypeEnum.GetItemFromSource:
                        var valueToSave = button.Items.FirstOrDefault(x => x.Value == button.VariableValue);
                        if (valueToSave.Key == null && valueToSave.Value == null)
                        {
                            Utils.ShowDialog("Invalid value");
                            return;
                        }
                        ButtonActionHelper.HandleSaveTextInput(button.VariableName, valueToSave.Key);
                        if (!button.Hidden)
                            ButtonActionHelper.HandlePostTextToThread(button.PrefixText + button.VariableValue + button.PostfixText);
                        break;
                    case ButtonTypeEnum.GetAddress:
                        {
                            bool done = false;
                            while (!done)
                            {
                                var ad = new AddressDialog();
                                var res = await ad.ShowAsync();
                                switch (res)
                                {
                                    case Windows.UI.Xaml.Controls.ContentDialogResult.Primary:
                                        if (Utils.ValidateStrings(ad.City, ad.PinCode, ad.Country, ad.Latitude == 0 ? "" : ad.Latitude.ToString(), ad.Longitude == 0 ? "" : ad.Longitude.ToString(), ad.StreetAddress))
                                        {
                                            ButtonActionHelper.HandleSaveTextInput("CITY", ad.City);
                                            ButtonActionHelper.HandleSaveTextInput("COUNTRY", ad.Country);
                                            ButtonActionHelper.HandleSaveTextInput("PINCODE", ad.PinCode);
                                            ButtonActionHelper.HandleSaveTextInput("LAT", ad.Latitude.ToString());
                                            ButtonActionHelper.HandleSaveTextInput("LNG", ad.Longitude.ToString());
                                            ButtonActionHelper.HandleSaveTextInput("STREET_ADDRESS", ad.StreetAddress);

                                            if (!button.Hidden)
                                                ButtonActionHelper.HandlePostTextToThread($"{ad.StreetAddress}\r\n\r\nCity: {ad.City}\r\nCountry: {ad.Country}\r\nPin: {ad.PinCode}");
                                            done = true;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                if (!done)
                                    await Utils.ShowDialogAsync("All fields in address are mandatory!");
                            }
                        }
                        break;
                    case ButtonTypeEnum.GetImage:
                    case ButtonTypeEnum.GetAudio:
                    case ButtonTypeEnum.GetVideo:
                        var image = await ButtonActionHelper.HandleSaveMediaInputAsync(button.VariableName, button.ButtonType);
                        ButtonActionHelper.HandlePostMediaToThread(image, button.ButtonType);
                        break;
                    case ButtonTypeEnum.NextNode:
                        if (!button.Hidden)
                            ButtonActionHelper.HandlePostTextToThread(button.ButtonText);
                        if (!string.IsNullOrWhiteSpace(button.VariableName) && button.VariableValue != null) //VariableValue should be != null only
                            ButtonActionHelper.HandleSaveTextInput(button.VariableName, button.VariableValue);
                        break;
                    case ButtonTypeEnum.DeepLink:
                        await ButtonActionHelper.HandleDeepLinkAsync(button.DeepLinkUrl);
                        if (!button.Hidden)
                            ButtonActionHelper.HandlePostTextToThread(button.ButtonText);
                        break;
                    default:
                        Utils.ShowDialog($"Button type: {button.ButtonType} not supported");
                        break;
                }
                ButtonActionHelper.NavigateToNode(button.NextNodeId);
            }
        }
    }
}
