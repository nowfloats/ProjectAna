using ANAConversationSimulator.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace ANAConversationSimulator.UserControls
{
    public sealed partial class AddressDialog : ContentDialog
    {
        public AddressDialog()
        {
            this.InitializeComponent();
            this.Map.MapServiceToken = Utils.Config.MapToken;
            this.DataContext = this;
            this.Loaded += AddressDialog_Loaded;
        }

        private async void AddressDialog_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAsync();
        }

        public double Latitude
        {
            get { return (double)GetValue(LatitudeProperty); }
            set { SetValue(LatitudeProperty, value); }
        }
        public static readonly DependencyProperty LatitudeProperty = DependencyProperty.Register("Latitude", typeof(double), typeof(AddressDialog), new PropertyMetadata(0.0));

        public double Longitude
        {
            get { return (double)GetValue(LongitudeProperty); }
            set { SetValue(LongitudeProperty, value); }
        }
        public static readonly DependencyProperty LongitudeProperty = DependencyProperty.Register("Longitude", typeof(double), typeof(AddressDialog), new PropertyMetadata(0.0));

        public string City
        {
            get { return (string)GetValue(CityProperty); }
            set { SetValue(CityProperty, value); }
        }
        public static readonly DependencyProperty CityProperty = DependencyProperty.Register("City", typeof(string), typeof(AddressDialog), new PropertyMetadata(null));

        public string Country
        {
            get { return (string)GetValue(CountryProperty); }
            set { SetValue(CountryProperty, value); }
        }
        public static readonly DependencyProperty CountryProperty = DependencyProperty.Register("Country", typeof(string), typeof(AddressDialog), new PropertyMetadata(null));

        public string StreetAddress
        {
            get { return (string)GetValue(StreetAddressProperty); }
            set { SetValue(StreetAddressProperty, value); }
        }
        public static readonly DependencyProperty StreetAddressProperty = DependencyProperty.Register("StreetAddress", typeof(string), typeof(AddressDialog), new PropertyMetadata(null));

        public string PinCode
        {
            get { return (string)GetValue(PinCodeProperty); }
            set { SetValue(PinCodeProperty, value); }
        }
        public static readonly DependencyProperty PinCodeProperty = DependencyProperty.Register("PinCode", typeof(string), typeof(AddressDialog), new PropertyMetadata(null));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(AddressDialog), new PropertyMetadata(false));

        public async Task LoadAsync()
        {
            try
            {
                var accessStatus = await Geolocator.RequestAccessAsync();
                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 0 };
                        IsLoading = true;
                        Geoposition pos = await geolocator.GetGeopositionAsync();
                        IsLoading = false;

                        await LoadMapDetailsAsync(pos.Coordinate.Point);
                        break;

                    case GeolocationAccessStatus.Denied:
                        await Utils.ShowDialogAsync("Without geo location permission, map cannot be zoomed to your location! Zoom it yourself!");
                        break;

                    case GeolocationAccessStatus.Unspecified:
                        break;
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                await Utils.ShowDialogAsync("GPS Error: \r\n\r\n" + ex.Message);
            }
        }
        async Task LoadMapDetailsAsync(Geopoint point)
        {
            IsLoading = true;
            try
            {
                if (Map.MapElements.Count > 0)
                {
                    MapIcon icon = Map.MapElements.First() as MapIcon;
                    icon.Location = point;
                }
                else
                {
                    var mapIcon = new MapIcon()
                    {
                        Location = point,
                        NormalizedAnchorPoint = new Point(0.5, 1.0),
                        Title = "Location",
                        ZIndex = 0,
                    };
                    Map.MapElements.Add(mapIcon);
                }
                Map.Center = point;
                Map.ZoomLevel = 14;

                var (city, country, pincode, address) = await APIHelper.LoadCityCountryFromLatLongAsync(point.Position.Latitude, point.Position.Longitude);

                City = city;
                Country = country;
                Latitude = point.Position.Latitude;
                Longitude = point.Position.Longitude;
                PinCode = pincode;
                StreetAddress = address;
            }
            catch (Exception ex)
            {
                Utils.ShowDialog(ex.ToString());
            }
            IsLoading = false;
        }
        private async void Map_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            await LoadMapDetailsAsync(args.Location);
        }
    }
}
