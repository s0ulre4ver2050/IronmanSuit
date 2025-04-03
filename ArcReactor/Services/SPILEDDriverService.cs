using ArcReactor.Services.Interfaces;
using Iot.Device.Ws28xx;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Spi;
using System.Threading;

namespace ArcReactor.Services
{
    public class SPILEDDriverService : ISPILEDDriverService
    {
        private SpiDevice _spiDevice;
        private SpiConnectionSettings _connectionSettings;
        private Ws28xx _neo;
        private BitmapImage _image;
        public void Initialise(int pixelCount, int pinNumber)
        {
            Configuration.SetPinFunction(23, DeviceFunction.SPI2_MOSI);
            Configuration.SetPinFunction(19, DeviceFunction.SPI2_MISO);
            Configuration.SetPinFunction(18, DeviceFunction.SPI2_CLOCK);
            // Pin 22 must be set to ADC to use as the chip selector
            //Configuration.SetPinFunction(pinNumber, DeviceFunction.ADC1_CH4);

            _connectionSettings = new SpiConnectionSettings(1, pinNumber);

            _connectionSettings.ClockFrequency = 2_400_000;
            _connectionSettings.DataBitLength = 8;
            _connectionSettings.Mode = SpiMode.Mode0;

            _spiDevice = SpiDevice.Create(_connectionSettings);

            _neo = new Ws2812b(_spiDevice, pixelCount);
            _image = _neo.Image;
        }

        public void Update(int redValue, int greenValue, int blueValue)
        {
            // Process the byte array in batches of 3
            for (int i = 0; i < _image.Data.Length; i += 3)
            {
                // order of values is
                // green - 0
                // red - 1
                // blue - 2

                _image.Data[i] = ((byte)greenValue);
                _image.Data[i + 1] = ((byte)redValue);
                _image.Data[i + 2] = ((byte)blueValue);
            }
            _neo.Update();
            Thread.Sleep(10);
        }
    }
}
