using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Bluetooth;
using BluetoothApp.Platforms.Android.Bluetooth;
using Java.Util;
using BluetoothApp.Helpers;


[assembly: Dependency(typeof(BluetoothConnector))]
namespace BluetoothApp.Platforms.Android.Bluetooth
{
    public partial class BluetoothConnector : IBluetoothConnector
    {
        /// <inheritdoc />
        public List<string> GetConnectedDevices()
        {
            _adapter = GetBluetoothAdapter();

            if (_adapter == null)
                throw new Exception("No Bluetooth adapter found.");

            if (_adapter.IsEnabled)
            {
                if (_adapter.BondedDevices.Count > 0)
                    return _adapter.BondedDevices.Select(d => d.Name).ToList();
            }
            else
                Console.Write("Bluetooth is not enabled on device");

            return new List<string>();
        }

        /// <inheritdoc />
        public void Connect(string deviceName)
        {
            var device = _adapter.BondedDevices.FirstOrDefault(d => d.Name == deviceName);
            var _socket = device.CreateInsecureRfcommSocketToServiceRecord(UUID.FromString(SspUdid));

            _socket.Connect();
        }

        public void TurnOn()
        {
            var buffer = "1";

            // Write data to the device to trigger LED
            _socket.OutputStream.WriteAsync(Encoding.ASCII.GetBytes(buffer), 0, buffer.Length);
        }

        public void TurnOff()
        {
            var buffer = "2";

            // Write data to the device to trigger LED
            _socket.OutputStream.WriteAsync(Encoding.ASCII.GetBytes(buffer), 0, buffer.Length);
        }

        /// <summary>
        /// The standard UDID for SSP
        /// </summary>
        private const string SspUdid = "00001101-0000-1000-8000-00805f9b34fb";
        private BluetoothAdapter _adapter;
        private BluetoothSocket _socket;

        private BluetoothAdapter GetBluetoothAdapter()
        {
            var bluetoothManager = MauiApplication.Current.GetSystemService("bluetooth") as BluetoothManager;
            return bluetoothManager.Adapter;
        }

        public bool IsConnected(string deviceName)
        {
            bool result = false;

            result = _adapter.BondedDevices.Count() > 0;

            return result;
        }

        public void Disconnect()
        {
            _socket.OutputStream.Close();
            Thread.Sleep(1000);
            _socket.Close();
            _socket = null;
            Application.Current.Quit();
        }
    }
}
