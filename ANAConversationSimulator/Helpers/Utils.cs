using ANAConversationSimulator.Models;
using ANAConversationSimulator.Models.Chat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace ANAConversationSimulator.Helpers
{
	public static class Utils
	{
		private static ApplicationDataContainer _APISettings;
		public static ApplicationDataContainer APISettings
		{
			get
			{
				if (_APISettings == null)
					_APISettings = ApplicationData.Current.LocalSettings.CreateContainer("API", ApplicationDataCreateDisposition.Always);
				return _APISettings;
			}
		}

		private static IPropertySet _localStore;
		public static IPropertySet LocalStore
		{
			get
			{
				if (_localStore == null)
					_localStore = ApplicationData.Current.LocalSettings.CreateContainer("LOCAL_STORE", ApplicationDataCreateDisposition.Always).Values;
				return _localStore;
			}
		}

		public static void InitMemoryStack()
		{
			InitDeviceIdInMemoryStack();
		}

		public static void InitDeviceIdInMemoryStack()
		{
			if (!Utils.LocalStore.ContainsKey("DEVICE_ID"))
				Utils.LocalStore["DEVICE_ID"] = Utils.GetFinalDeviceId();
		}

		public static void ShowDialog(string txt)
		{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			new MessageDialog(txt).ShowAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		}

		public static async Task ShowDialogAsync(string txt)
		{
			await new MessageDialog(txt).ShowAsync();
		}
		public static string VersionDisplay()
		{
			var v = Package.Current.Id.Version;
			return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
		}

		public static DeviceFormFactorType GetDeviceFormFactorType()
		{
			System.Diagnostics.Debug.WriteLine(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily);
			switch (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
			{
				case "Windows.Mobile":
					return DeviceFormFactorType.Phone;
				case "Windows.Desktop":
					return Windows.UI.ViewManagement.UIViewSettings.GetForCurrentView().UserInteractionMode == Windows.UI.ViewManagement.UserInteractionMode.Mouse
						? DeviceFormFactorType.Desktop
						: DeviceFormFactorType.Tablet;
				case "Windows.Universal":
					return DeviceFormFactorType.IoT;
				case "Windows.Team":
					return DeviceFormFactorType.SurfaceHub;
				default:
					return DeviceFormFactorType.Other;
			}
		}
		public static bool IsValidEmail(string email)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(email)) return false;
				return Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$");
			}
			catch
			{
				return false;
			}
		}

		public static bool IsValidTextButton(Button button)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(button.VariableValue))
					return false;

				if (button.MinLength > 0 && button.VariableValue.Length < button.MinLength)
					return false;

				if (button.MaxLength > 0 && button.VariableValue.Length > button.MaxLength)
					return false;

				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool ValidateStrings(params string[] input)
		{
			if (input == null) return false;
			foreach (var item in input)
				if (string.IsNullOrWhiteSpace(item)) return false;
			return true;
		}

		public static bool IsValidPhoneNumber(string phoneNumber)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(phoneNumber)) return false;
				return Regex.IsMatch(phoneNumber, @"\+?\d{6,15}");
			}
			catch
			{
				return false;
			}
		}

		const string FILE_NAME = "Config.json";
		public static Config Config { get; private set; }
		public static async Task LoadConfig()
		{
			try
			{
				var configJsonFile = await Package.Current.InstalledLocation.GetFileAsync(FILE_NAME);
				if (configJsonFile != null)
					Config = JsonConvert.DeserializeObject<Config>(await FileIO.ReadTextAsync(configJsonFile));
				else
					ShowDialog("Config.json file not found. It should be placed in the root folder of the project and should contain 'MapToken', 'FileUploadAPI' values.");
			}
			catch (Exception ex)
			{
				ShowDialog("Unable to load Config.\r\n\r\nMessage: " + ex.Message);
			}
		}

		public static ChatActivityEvent GetViewEvent(string nodeId, string userId)
		{
			return new ChatActivityEvent
			{
				EventCategory = "ANA_CHAT",
				EventChannel = "ANA_SIM_WIN",
				NodeId = nodeId,
				EventName = "VIEW",
				UserId = userId,
				EventDateTime = DateTime.UtcNow,
			};
		}
		public static ChatActivityEvent GetClickEvent(string nodeId, string userId, string buttonId, string buttonLabel, Dictionary<string, string> userData)
		{
			return new ChatActivityEvent
			{
				EventCategory = "ANA_CHAT",
				EventChannel = "ANA_SIM_WIN",
				NodeId = nodeId,
				EventName = "CLICK",
				UserId = userId,
				EventDateTime = DateTime.UtcNow,
				EventData = new Dictionary<string, string>
				{
					{ "ButtonID", buttonId },
					{ "ButtonLabel", buttonLabel },
				},
				UserData = userData
			};
		}

		public static string DeviceId
		{
			get
			{
				return (string)Utils.LocalStore["DEVICE_ID"];
			}
		}

		public static string GetFinalDeviceId() => CalculateMD5Hash(GetDeviceId());
		private static string GetDeviceId()
		{
			if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.System.Profile.HardwareIdentification"))
			{
				var token = HardwareIdentification.GetPackageSpecificToken(null);
				var hardwareId = token.Id;
				var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);
				byte[] bytes = new byte[hardwareId.Length];
				dataReader.ReadBytes(bytes);
				return BitConverter.ToString(bytes).Replace("-", "");
			}
			return "DEVICE-ID-NOT-FOUND";
		}

		public static string CalculateMD5Hash(string input)
		{
			var md5 = MD5.Create();
			var inputBytes = Encoding.ASCII.GetBytes(input);
			var hash = md5.ComputeHash(inputBytes);
			var sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
				sb.Append(hash[i].ToString("X2"));
			return sb.ToString();
		}

		public static readonly ButtonTypeEnum[] IGNORED_DEFAULT_BUTTONS = new[] { ButtonTypeEnum.GetItemFromSource };

		public static bool IsSectionTypePresentInNode(JToken node, SectionTypeEnum secType)
		{
			return node?["Sections"]?.Any(x => x.ToObject<Section>()?.SectionType == secType) == true;
		}

		public static T DeepCopy<T>(this T source)
		{
			var json = JsonConvert.SerializeObject(source);
			return JsonConvert.DeserializeObject<T>(json);
		}

		public static string NormalizeFileName(string text)
		{
			if (text == null)
				text = "New File";
			foreach (var c in Path.GetInvalidFileNameChars())
				text.Replace(c, '_');
			return text;
		}

		public static async Task SaveVisualElementToFile(FrameworkElement element, StorageFile file)
		{
			var renderTargetBitmap = new RenderTargetBitmap();
			await renderTargetBitmap.RenderAsync(element, (int)element.Width, (int)element.Height);
			var pixels = await renderTargetBitmap.GetPixelsAsync();
			using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
			{
				var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
				byte[] bytes = pixels.ToArray();
				encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)renderTargetBitmap.PixelWidth, (uint)renderTargetBitmap.PixelHeight, 96, 96, bytes);
				await encoder.FlushAsync();
			}
		}

		public static async Task<Geoposition> GetCurrentGeoLocation()
		{
			var accessStatus = await Geolocator.RequestAccessAsync();
			switch (accessStatus)
			{
				case GeolocationAccessStatus.Allowed:
					Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 0 };
					return await geolocator.GetGeopositionAsync();
				case GeolocationAccessStatus.Denied:
					await Utils.ShowDialogAsync("Geo location permission is required!");
					break;
				case GeolocationAccessStatus.Unspecified:
					break;
			}
			return null;
		}
	}

	public enum DeviceFormFactorType
	{
		Phone,
		Desktop,
		Tablet,
		IoT,
		SurfaceHub,
		Other
	}
}
