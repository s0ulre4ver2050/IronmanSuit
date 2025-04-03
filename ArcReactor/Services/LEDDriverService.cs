using ArcReactor.Services.Interfaces;
using Iot.Device.Ws28xx.Esp32;
using nanoFramework.Hardware.Esp32;
using System;
using System.Drawing;

namespace ArcReactor.Services
{
    public class LEDDriverService : ILEDDriverService , IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private int _pixelCount;
        private int _pinNumber;

        private Ws2812c _neo;
        private BitmapImage _image;

        public LEDDriverService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Initialise(int pixelCount, int pinNumber)
        {
            try
            {
                _pixelCount = pixelCount;
                _pinNumber = pinNumber;

                Configuration.SetPinFunction(_pinNumber, DeviceFunction.PWM1);

                _neo = new Ws2812c(_pinNumber, _pixelCount);
                _image = _neo.Image;
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message);
            }
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

            /*Color color = Color.FromArgb(0, redValue, greenValue, blueValue);

            for (var j = 0; j < _image.Width; j++)
            {
                _image.SetPixel(j, 0, color);

            }*/
            _neo.Update();
        }

        public void Dispose()
        {
            if (_neo != null)
            {
                _neo = null;
            }
        }
    }
}
