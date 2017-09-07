using ANAConversationSimulator.Helpers;
using ANAConversationSimulator.UserControls;
using ANAConversationSimulator.Models.Chat;
using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.System.Threading;
using ANAConversationSimulator.Models.Chat.Sections;
using ANAConversationSimulator.ViewModels;
using UWPControls = Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using System.Globalization;

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
				var parentNode = MainPageViewModel.CurrentInstance.GetNodeById(button.NodeId);
				var parsedParentNode = parentNode?.ToObject<ChatNode>();

				//Special Case: Print OTP Section
				if (Utils.IsSectionTypePresentInNode(parentNode, SectionTypeEnum.PrintOTP))
					button.VariableValue = PrintOTPSection.OTP;

				if (string.IsNullOrWhiteSpace(button.ButtonText))
					button.ButtonText = button.ButtonName;

				var userData = new Dictionary<string, string>();
				switch (button.ButtonType)
				{
					case ButtonTypeEnum.PostText:
						if (!button.Hidden && button.PostToChat)
							ButtonActionHelper.HandlePostTextToThread(button.ButtonText);
						break;
					case ButtonTypeEnum.OpenUrl:
						ButtonActionHelper.HandleOpenUrl(button.Url);
						break;
					case ButtonTypeEnum.GetText:
						if (string.IsNullOrWhiteSpace(button.VariableValue)) return;
						ButtonActionHelper.HandleSaveTextInput(button.VariableName, button.VariableValue);
						userData[button.VariableName] = button.VariableValue;
						if (!button.Hidden && button.PostToChat)
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
						userData[button.VariableName] = button.VariableValue;

						if (!button.Hidden && button.PostToChat)
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
						userData[button.VariableName] = d.ToString();
						if (!button.Hidden && button.PostToChat)
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
						userData[button.VariableName] = button.VariableValue;

						if (!button.Hidden && button.PostToChat)
							ButtonActionHelper.HandlePostTextToThread(button.PrefixText + button.VariableValue + button.PostfixText);
						break;
					case ButtonTypeEnum.GetItemFromSource:
						var valueToSave = button.Items?.FirstOrDefault(x => x.Value == button.VariableValue);
						if (valueToSave?.Key == null && valueToSave?.Value == null)
						{
							Utils.ShowDialog("Invalid value");
							return;
						}
						ButtonActionHelper.HandleSaveTextInput(button.VariableName, valueToSave?.Key);
						userData[button.VariableName] = valueToSave?.Key;

						if (!button.Hidden && button.PostToChat)
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

											#region User Data Fill
											userData["CITY"] = ad.City;
											userData["COUNTRY"] = ad.Country;
											userData["PINCODE"] = ad.PinCode;
											userData["LAT"] = ad.Latitude.ToString();
											userData["LNG"] = ad.Longitude.ToString();
											userData["STREET_ADDRESS"] = ad.StreetAddress;
											#endregion

											if (!button.Hidden && button.PostToChat)
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
					case ButtonTypeEnum.GetFile:
					case ButtonTypeEnum.GetAudio:
					case ButtonTypeEnum.GetVideo:
						var mediaUrl = await ButtonActionHelper.HandleSaveMediaInputAsync(button.VariableName, button.ButtonType);
						userData[button.VariableName] = mediaUrl;
						ButtonActionHelper.HandlePostMediaToThread(mediaUrl, button.ButtonType);
						break;
					case ButtonTypeEnum.NextNode:
						if (!button.Hidden && button.PostToChat)
							ButtonActionHelper.HandlePostTextToThread(button.ButtonText);
						if (!string.IsNullOrWhiteSpace(button.VariableName) && button.VariableValue != null) //VariableValue should be != null only
							ButtonActionHelper.HandleSaveTextInput(button.VariableName, button.VariableValue);
						break;
					case ButtonTypeEnum.DeepLink:
						await ButtonActionHelper.HandleDeepLinkAsync(button.DeepLinkUrl);
						if (!button.Hidden && button.PostToChat)
							ButtonActionHelper.HandlePostTextToThread(button.ButtonText);
						break;
					case ButtonTypeEnum.GetAgent:
						if (MainPageViewModel.CurrentInstance != null)
							MainPageViewModel.CurrentInstance.AgentChat();
						if (!button.Hidden && button.PostToChat)
							ButtonActionHelper.HandlePostTextToThread(button.ButtonText);
						break;
					case ButtonTypeEnum.FetchChatFlow:
						if (!button.Hidden && button.PostToChat)
							ButtonActionHelper.HandlePostTextToThread(button.ButtonText);
						if (!string.IsNullOrWhiteSpace(button.VariableName) && button.VariableValue != null) //VariableValue should be != null only
							ButtonActionHelper.HandleSaveTextInput(button.VariableName, button.VariableValue);
						await ButtonActionHelper.HandleFetchChatFlowAsync(button.Url);
						break;
					case ButtonTypeEnum.GetDate:
						{
							var cd = new DateTimePickerDialog() { IsDateVisible = true };
							cd.Title = "Pick a date";
							var res = await cd.ShowAsync();
							if (res == UWPControls.ContentDialogResult.Primary)
							{
								button.VariableValue = cd.ChoosenDate.ToString("yyyy-MM-dd");
								ButtonActionHelper.HandleSaveTextInput(button.VariableName, button.VariableValue);
								ButtonActionHelper.HandleSaveTextInput(button.VariableName + "_DISPLAY", cd.ChoosenDate.ToString("dd MMM, yyyy"));
								userData[button.VariableName] = button.VariableValue;
								if (!button.Hidden && button.PostToChat)
									ButtonActionHelper.HandlePostTextToThread(button.VariableValue);
							}
							else
								return;
						}
						break;
					case ButtonTypeEnum.GetDateTime:
						{
							var cd = new DateTimePickerDialog() { IsDateVisible = true, IsTimeVisible = true };
							cd.Title = "Pick date time";
							var res = await cd.ShowAsync();
							if (res == UWPControls.ContentDialogResult.Primary)
							{
								DateTimeOffset dtLocal = cd.ChoosenDate.Date + cd.ChoosenTime;
								DateTime dtUTC = DateTime.Parse(dtLocal.ToString(), null, DateTimeStyles.AdjustToUniversal);
								button.VariableValue = JsonConvert.SerializeObject(dtUTC).Trim('"');
								var display = dtLocal.ToString("dd MMM, yyyy hh:mm:ss tt");
								ButtonActionHelper.HandleSaveTextInput(button.VariableName, button.VariableValue);
								ButtonActionHelper.HandleSaveTextInput(button.VariableName + "_DISPLAY", display);
								ButtonActionHelper.HandleSaveTextInput(button.VariableName + "_DISPLAY2", dtUTC.ToString("dd MMM, yyyy hh:mm:ss tt"));
								ButtonActionHelper.HandleSaveTextInput(button.VariableName + "_DISPLAY3", JsonConvert.SerializeObject(dtUTC).Trim('"'));
								userData[button.VariableName] = button.VariableValue;
								if (!button.Hidden && button.PostToChat)
									ButtonActionHelper.HandlePostTextToThread(display);
							}
							else
								return;
						}
						break;
					case ButtonTypeEnum.GetTime:
						{
							var cd = new DateTimePickerDialog() { IsTimeVisible = true };
							cd.Title = "Pick time";
							var res = await cd.ShowAsync();
							if (res == UWPControls.ContentDialogResult.Primary)
							{
								button.VariableValue = JsonConvert.SerializeObject(cd.ChoosenTime).Trim('"');
								var display = new DateTime(cd.ChoosenTime.Ticks).ToString("hh:mm tt");
								ButtonActionHelper.HandleSaveTextInput(button.VariableName, button.VariableValue);
								ButtonActionHelper.HandleSaveTextInput(button.VariableName + "_DISPLAY", display);
								ButtonActionHelper.HandleSaveTextInput(button.VariableName + "_DISPLAY2", new DateTime(cd.ChoosenTime.Ticks).ToString("HH:mm"));
								userData[button.VariableName] = button.VariableValue;
								if (!button.Hidden && button.PostToChat)
									ButtonActionHelper.HandlePostTextToThread(display);
							}
							else
								return;
						}
						break;
					default:
						Utils.ShowDialog($"Button type: {button.ButtonType} not supported");
						break;
				}
				trackViewEvent(button, userData);
				ButtonActionHelper.NavigateToNode(button.NextNodeId);
			}
			else if (parameter is CarouselButton cButton)
			{
				var userData = new Dictionary<string, string>();
				switch (cButton.Type)
				{
					case CardButtonType.NextNode:
						if (!string.IsNullOrWhiteSpace(cButton.VariableName) && cButton.VariableValue != null) //VariableValue should be != null only
						{
							ButtonActionHelper.HandleSaveTextInput(cButton.VariableName, cButton.VariableValue);
							userData[cButton.VariableName] = cButton.VariableValue;
						}
						break;
					case CardButtonType.DeepLink:
						await ButtonActionHelper.HandleDeepLinkAsync(cButton.Url);
						break;
					case CardButtonType.OpenUrl:
						ButtonActionHelper.HandleOpenUrl(cButton.Url);
						break;
					default:
						Utils.ShowDialog($"Button type: {cButton.Type} not supported");
						break;
				}
				trackViewEvent(cButton, userData);
				ButtonActionHelper.NavigateToNode(cButton.NextNodeId);
			}
		}

		private async void trackViewEvent(Button button, Dictionary<string, string> userData)
		{
			await Task.Run(async () =>
			{
				await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
				 {
					 try
					 {
						 if (userData.Count == 0)
							 userData = null;
						 await APIHelper.TrackEvent(Utils.GetClickEvent(button.NodeId, Utils.DeviceId, button._id, button.ButtonText, userData));
					 }
					 catch (Exception ex)
					 {
						 await Utils.ShowDialogAsync(ex.ToString());
					 }
				 });
			});
		}
		private async void trackViewEvent(CarouselButton cButton, Dictionary<string, string> userData)
		{
			await Task.Run(async () =>
			{
				await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
				{
					try
					{
						if (userData.Count == 0)
							userData = null;
						await APIHelper.TrackEvent(Utils.GetClickEvent(cButton.NodeId, Utils.DeviceId, cButton._id, cButton.Text, userData));
					}
					catch (Exception ex)
					{
						await Utils.ShowDialogAsync(ex.ToString());
					}
				});

			});
		}
	}
}
