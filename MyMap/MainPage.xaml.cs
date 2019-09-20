using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace MyMap
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        Plugin.Geolocator.Abstractions.IGeolocator Geolocator;

        public MainPage()
        {
            InitializeComponent();

            SetupSegment();
            DummyPins();
            SetupMap();
            SetUpEventWidget();
            GetCurrentPosition();
        }

        private void SetUpEventWidget()
        {
            TrackingButton.Text = "Start Location";

            TrackingButton.Clicked += async (sender, events) =>
            {
                if (TrackingButton.Text == "Start Location")
                {
                    // tracking

                    Geolocator = CrossGeolocator.Current;
                    Geolocator.DesiredAccuracy = 100;
                    Geolocator.PositionChanged += (_sender, _event) =>
                    {
                        Console.WriteLine($"Position Status: {_event.Position.Timestamp}");
                        Console.WriteLine($"Position Latitude: {_event.Position.Latitude}");
                        Console.WriteLine($"Position Longitude: {_event.Position.Longitude}");

                        var currentLocation = new Position(_event.Position.Latitude, _event.Position.Longitude);
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(currentLocation, Distance.FromMeters(200)));
                    };
                    await Geolocator.StartListeningAsync(TimeSpan.FromSeconds(5), 50);

                    Device.BeginInvokeOnMainThread(() => {
                        TrackingButton.Text = "Stop Location";
                    });
                }
                else
                {
                    bool result = await DisplayAlert("title", "message", "yes", "no");
                    if(result == true)
                    {
                        await Geolocator.StopListeningAsync();

                        Device.BeginInvokeOnMainThread(() => {
                            TrackingButton.Text = "Start Location";
                        });
                    }
                }
            };
        }

        private void SetupMap()
        {
            Position latLng = new Position(7.0073379, 100.465442);
            //map.MoveCamera(CameraUpdateFactory.NewPositionZoom(latLng, 16));
            map.MoveToRegion(MapSpan.FromCenterAndRadius(latLng, Distance.FromMiles(30)));

            if (CrossGeolocator.Current.IsGeolocationAvailable)
            {
                map.UiSettings.MyLocationButtonEnabled = true;
                map.MyLocationEnabled = true;
            }
        }

        private void GetCurrentPosition()
        {
            Task.Run(async () =>
            {
                try
                {
                    var crossGeolocator = CrossGeolocator.Current;

                    if (!crossGeolocator.IsGeolocationAvailable || !crossGeolocator.IsGeolocationEnabled)
                    {
                        await DisplayAlert("title", "Not available or enabled", "Close");
                    }
                    else
                    {
                        crossGeolocator.DesiredAccuracy = 100;

                        Plugin.Geolocator.Abstractions.Position position = await crossGeolocator.GetLastKnownLocationAsync();

                        if (position != null)
                        {
                            Console.WriteLine($"Current Location: {position.Latitude}, {position.Longitude}");
                            AddPin("current location", "person 01", new Position(position.Latitude, position.Longitude));
                        }
                        else
                        {
                            await DisplayAlert("title", "Unable to get location", "Close");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("title", $"Unable to get location: {ex}", "Close");
                }
            });
        }

        private void DummyPins()
        {
            List<Position> pins = new List<Position>()
            {
                new Position(13.739699, 100.5349273),
                new Position(13.738201, 100.5245553),
                new Position(13.752789, 100.5494063)
            };

            foreach (var item in pins)
            {
                AddPin("dummy", "detail", item);
            }
        }

        private void AddPin(string title, string snippet, Position position)
        {
            BitmapDescriptor bitmap = null;
            if ( ((int)position.Latitude % 2) == 0)
            {
                bitmap = BitmapDescriptorFactory.FromBundle("cmdev_pin_01.png");
            }
            else
            {
                bitmap = BitmapDescriptorFactory.FromBundle("cmdev_pin_02.png");
            }

            var pin = new Pin
            {
                Label = title,
                Address = snippet,
                Position = position,
                //Icon = BitmapDescriptorFactory.DefaultMarker(Color.Aqua),
                Icon = bitmap
            };

            map.Pins.Add(pin);
        }

        private void SetupSegment()
        {
            mSegControl.ValueChanged += (sender, events) =>
            {
                switch (mSegControl.SelectedSegment)
                {
                    case 0:
                        map.MapType = MapType.Street;  
                        break;
                    case 1:
                        map.MapType = MapType.Satellite;
                        break;
                    default:
                        map.MapType = MapType.Hybrid;
                        break;
                }
            };
        }
    }
}
