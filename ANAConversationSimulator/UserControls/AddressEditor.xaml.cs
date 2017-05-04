using BoostWin10.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BoostWin10.UserControls
{
    public sealed partial class AddressEditor : UserControl
    {
        public AddressEditor()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public double Latitude
        {
            get { return (double)GetValue(LatitudeProperty); }
            set { SetValue(LatitudeProperty, value); }
        }
        public static readonly DependencyProperty LatitudeProperty = DependencyProperty.Register("Latitude", typeof(double), typeof(AddressEditor), new PropertyMetadata(0));

        public double Longitude
        {
            get { return (double)GetValue(LongitudeProperty); }
            set { SetValue(LongitudeProperty, value); }
        }
        public static readonly DependencyProperty LongitudeProperty = DependencyProperty.Register("Longitude", typeof(double), typeof(AddressEditor), new PropertyMetadata(0));

        public string City
        {
            get { return (string)GetValue(CityProperty); }
            set { SetValue(CityProperty, value); }
        }
        public static readonly DependencyProperty CityProperty = DependencyProperty.Register("City", typeof(string), typeof(AddressEditor), new PropertyMetadata(null));

        public string Country
        {
            get { return (string)GetValue(CountryProperty); }
            set { SetValue(CountryProperty, value); }
        }
        public static readonly DependencyProperty CountryProperty = DependencyProperty.Register("Country", typeof(string), typeof(AddressEditor), new PropertyMetadata(null));

        public string StreetAddress
        {
            get { return (string)GetValue(StreetAddressProperty); }
            set { SetValue(StreetAddressProperty, value); }
        }
        public static readonly DependencyProperty StreetAddressProperty = DependencyProperty.Register("StreetAddress", typeof(string), typeof(AddressEditor), new PropertyMetadata(null));

        public async Task LoadAsync()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 0 };
                    Geoposition pos = await geolocator.GetGeopositionAsync();

                    var mapIcon = new MapIcon()
                    {
                        Location = pos.Coordinate.Point,
                        NormalizedAnchorPoint = new Point(0.5, 1.0),
                        Title = "Location",
                        ZIndex = 0
                    };
                    Map.MapElements.Add(mapIcon);
                    Map.Center = mapIcon.Location;
                    Map.ZoomLevel = 14;

                    var (city, country, pincode) = await APIHelper.LoadCityCountryFromLatLongAsync(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);
                    {
                        City = city;
                        Country = country;
                        Latitude = pos.Coordinate.Point.Position.Latitude;
                        Longitude = pos.Coordinate.Point.Position.Longitude;
                    }
                    break;

                case GeolocationAccessStatus.Denied:
                    await Utils.ShowDialogAsync("Geolocation permission is needed! Give it now!");
                    await LoadAsync();
                    break;

                case GeolocationAccessStatus.Unspecified:
                    break;
            }

        }
    }
}
