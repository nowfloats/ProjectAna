using ANAConversationSimulator.Models;
using ANAConversationSimulator.ViewModels;
using ANAConversationSimulator.Models.Chat;
using ANAConversationSimulator.Models.Chat.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Newtonsoft.Json.Linq;

namespace ANAConversationSimulator.Helpers
{
	public static class ButtonActionHelper
	{
		public static void HandleSaveTextInput(string varName, string varValue)
		{
			if (string.IsNullOrWhiteSpace(varName)) return;
			Utils.LocalStore[varName] = varValue?.Trim();
		}
		public static void ClearSavedValue(string varName)
		{
			if (string.IsNullOrWhiteSpace(varName)) return;
			if (Utils.LocalStore.ContainsKey(varName))
				Utils.LocalStore.Remove(varName);
		}
		public static object GetSavedValue(string varName)
		{
			if (string.IsNullOrWhiteSpace(varName))
				return null;

			if (varName.Contains('.'))
			{
				var all = varName.Split('.');
				var baseName = all[0];
				if (Utils.LocalStore.ContainsKey(baseName))
				{
					var stripped = string.Join(".", varName.Split('.').Skip(1).ToArray());
					var obj = JToken.Parse(Utils.LocalStore[baseName] + "");
					return obj.SelectToken(stripped) + "";
				}
			}

			if (Utils.LocalStore.ContainsKey(varName))
				return Utils.LocalStore[varName];
			return null;
		}
		public static JArray GetSavedArray(string varName)
		{
			if (Utils.LocalStore.ContainsKey(varName) && !string.IsNullOrWhiteSpace(Utils.LocalStore[varName] + ""))
				return JArray.Parse(Utils.LocalStore[varName] + "");
			return null;
		}
		public static void HandleSaveMultiple(Dictionary<string, object> dict)
		{
			foreach (var item in dict)
				HandleSaveTextInput(item.Key, item.Value + "");
		}
		public static async Task<string> HandleSaveMediaInputAsync(string varName, ButtonTypeEnum buttonType)
		{
			FileOpenPicker fop = new FileOpenPicker() { CommitButtonText = "Done" };

			if (buttonType == ButtonTypeEnum.GetImage)
			{
				fop.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
				fop.ViewMode = PickerViewMode.Thumbnail;
				fop.FileTypeFilter.Add(".jpg");
				fop.FileTypeFilter.Add(".jpeg");
				fop.FileTypeFilter.Add(".png");
				fop.FileTypeFilter.Add(".bmp");
				fop.FileTypeFilter.Add(".gif");
			}
			else if (buttonType == ButtonTypeEnum.GetVideo)
			{
				fop.SuggestedStartLocation = PickerLocationId.VideosLibrary;
				fop.ViewMode = PickerViewMode.Thumbnail;
				fop.FileTypeFilter.Add(".mp4");
				fop.FileTypeFilter.Add(".avi");
				fop.FileTypeFilter.Add(".wmv");
			}
			else if (buttonType == ButtonTypeEnum.GetAudio)
			{
				fop.SuggestedStartLocation = PickerLocationId.MusicLibrary;
				fop.ViewMode = PickerViewMode.List;
				fop.FileTypeFilter.Add(".mp3");
				fop.FileTypeFilter.Add(".wav");
				fop.FileTypeFilter.Add(".wma");
			}
			else if (buttonType == ButtonTypeEnum.GetFile)
			{
				fop.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
				fop.ViewMode = PickerViewMode.List;
				fop.FileTypeFilter.Add("*");
			}
			var sf = await fop.PickSingleFileAsync();
			var fileUploadResp = await APIHelper.UploadFile<UploadFileResponse>(sf.Name, sf);
			if (fileUploadResp != null && fileUploadResp.Url != null)
			{
				Utils.LocalStore[varName] = fileUploadResp.Url;
				return fileUploadResp.Url;
			}
			return null;
		}

		public static void HandlePostTextToThread(string text)
		{
			if (MainPageViewModel.CurrentInstance == null) return;
			MainPageViewModel.CurrentInstance.AddOutgoingSection(new TextSection
			{
				SectionType = SectionTypeEnum.Text,
				Text = text,
			});
		}
		public static void HandlePostMediaToThread(string mediaUrl, ButtonTypeEnum mediaType)
		{
			if (MainPageViewModel.CurrentInstance == null) return;

			Section section = null;
			switch (mediaType)
			{
				case ButtonTypeEnum.GetImage:
					section = new ImageSection
					{
						Url = mediaUrl
					};
					break;
				case ButtonTypeEnum.GetAudio:
					section = new AudioSection
					{
						Url = mediaUrl
					};
					break;
				case ButtonTypeEnum.GetVideo:
					section = new VideoSection
					{
						Url = mediaUrl
					};
					break;
				default:
					return;
			}
			MainPageViewModel.CurrentInstance.AddOutgoingSection(section);
		}
		public static async void HandleOpenUrl(string openUrl)
		{
			await Launcher.LaunchUriAsync(new Uri(openUrl));
		}

		internal static async Task HandleDeepLinkAsync(string deeplinkSlug)
		{
			switch (deeplinkSlug)
			{
				case "asklocationpermission":
					await Geolocator.RequestAccessAsync();
					return;
				default:
					break;
			}
			Utils.ShowDialog($"Deeplink: {deeplinkSlug}.\r\nNote: Deeplinks not supported here");
		}

		internal static async Task HandleFetchChatFlowAsync(string flowUrl)
		{
			await MainPageViewModel.CurrentInstance.JoinNodesFromFlowAsync(flowUrl);
		}

		public static void NavigateToNode(string nodeId)
		{
			MainPageViewModel.CurrentInstance.NavigateToNode(nodeId);
		}
	}
}
