using BluetoothApp.Helpers;
using ConditionalCompilationDeviceOrientationService = BluetoothApp.Helpers.BluetoothConnectorService;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace BluetoothApp
{
    // Artigo usado como base: https://app-trap.com/connect-net-maui-app-to-arduino-bluetooth/
    // Repo do artigo: https://github.com/nichojo89/MauiBluetoothAndroidArduino/tree/main
    public partial class MainPage : ContentPage
    {
        private readonly ConditionalCompilationDeviceOrientationService _conditional;
        private string selectedItem = "";

        public MainPage()
        {
            InitializeComponent();

            _conditional = new ConditionalCompilationDeviceOrientationService();
        }

        private async void ScanButton_Clicked(object sender, EventArgs e)
        {
            // Link de exemplo para configurar as permissões: https://github.com/dotnet-bluetooth-le/dotnet-bluetooth-le/blob/master/Source/BLE.Client/BLE.Client.Maui/Platforms/Android/DroidPlatformHelpers.cs
            PermissionStatus status;

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
            {
                status = await Permissions.CheckStatusAsync<BluetoothPermissions>();

                if (status != PermissionStatus.Granted)
                    await Application.Current.MainPage.DisplayAlert("Permission required", "Bluetooth scanning.", "OK");


                status = await Permissions.RequestAsync<BluetoothPermissions>();
            }
            else
            {
                status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                    await Application.Current.MainPage.DisplayAlert("Permission required", "Location permission is required for bluetooth scanning. We do not store or use your location at all.", "OK");

                if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
                    await Application.Current.MainPage.DisplayAlert("Permission required", "Location permission is required for bluetooth scanning. We do not store or use your location at all.", "OK");

                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            //Gets a list of all connected Bluetooth devices
            var ConnectedDevices = _conditional.GetConnectedDevices();
            // Como fazer o bind dos valores list<string>: https://stackoverflow.com/questions/2765369/binding-to-an-observablecollectionstring-listview?rq=4
            listView.ItemsSource = ConnectedDevices;

            ScanButton.IsEnabled = true;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var connected = e.SelectedItem as string;
            selectedItem = connected;

            _conditional.Connect(selectedItem);
        }

        private void TurnOnButton_Clicked(object sender, EventArgs e)
        {
            if (_conditional.IsConnected(selectedItem))
                _conditional.TurnOn();
        }

        private void TurnOffButton_Clicked(object sender, EventArgs e)
        {
            if (_conditional.IsConnected(selectedItem))
                _conditional.TurnOff();
        }

        private void DisconnectButton_Clicked(object sender, EventArgs e)
        {
            _conditional.Disconnect();
            listView.ItemsSource = null;
        }
    }

}
