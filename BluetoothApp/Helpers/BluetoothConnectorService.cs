using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if ANDROID
using Android.Bluetooth;
using Android.Content;
using Java.Util;
#endif

namespace BluetoothApp.Helpers
{
    public class BluetoothConnectorService
    {
        /// <inheritdoc />
        public List<string> GetConnectedDevices()
        {
#if ANDROID
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
#endif
            return new List<string>();
        }

        /// <inheritdoc />
        public void Connect(string deviceName)
        {
#if ANDROID
            var device = _adapter.BondedDevices.FirstOrDefault(d => d.Name == deviceName);
            if (device == null)
                throw new Exception("Device not found");

            // Aquí se crea el UUID correctamente
            var uuid = UUID.FromString(SspUdid); // Usar UUID de Android
            _socket = device.CreateInsecureRfcommSocketToServiceRecord(uuid);
            _socket.Connect();
#endif
        }

        public void TurnOn()
        {
#if ANDROID
            var buffer = "1";
            _socket.OutputStream.WriteAsync(Encoding.ASCII.GetBytes(buffer), 0, buffer.Length);
#endif
        }

        public void TurnOff()
        {
#if ANDROID
            var buffer = "0";
            _socket.OutputStream.WriteAsync(Encoding.ASCII.GetBytes(buffer), 0, buffer.Length);
#endif
        }

        public bool IsConnected(string deviceName)
        {
            bool result = false;
#if ANDROID
            result = _adapter.BondedDevices.Count > 0;
#endif
            return result;
        }

        public void Disconnect()
        {
#if ANDROID
            _socket.OutputStream.Close();
            Thread.Sleep(1000);
            _socket.Close();
            _socket = null;
#endif
            Application.Current.Quit();
        }

#if ANDROID
        private const string SspUdid = "00001101-0000-1000-8000-00805f9b34fb";  // Identificador de servicio
        private BluetoothAdapter _adapter;
        private BluetoothSocket _socket;

        private BluetoothAdapter GetBluetoothAdapter()
        {
            var bluetoothManager = MauiApplication.Current.GetSystemService(Context.BluetoothService) as BluetoothManager;
            return bluetoothManager?.Adapter;
        }
#endif
    }
}
