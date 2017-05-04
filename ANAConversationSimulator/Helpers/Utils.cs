using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;

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
