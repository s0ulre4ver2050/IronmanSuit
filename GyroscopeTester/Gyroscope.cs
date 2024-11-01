using Iot.Device.Imu;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.I2c;
using System.Numerics;
using System.Threading;

namespace GyroscopeTester
{
    public class Gyroscope
    {
        //int dataPin = 14;
        //int clockPin = 15;
        DeviceFunction _gyroscopeI2CDataDeviceFunction = DeviceFunction.I2C1_DATA;
        DeviceFunction _gyroscopeI2CClockDeviceFunction = DeviceFunction.I2C2_CLOCK;
        I2cConnectionSettings _gyroscopeI2CConnectionSettings;
        Mpu6050 _gyroscope;
        AverageValue _averageValue;
        //Thread _gyroscopeThread;

        public void Initialise(int dataPin, int clockPin)
        {
            Configuration.SetPinFunction(dataPin, _gyroscopeI2CDataDeviceFunction);
            Configuration.SetPinFunction(clockPin, _gyroscopeI2CClockDeviceFunction);

             _gyroscopeI2CConnectionSettings = new I2cConnectionSettings(1, Mpu6050.DefaultI2cAddress);
            _gyroscope = new Mpu6050(I2cDevice.Create(_gyroscopeI2CConnectionSettings));

            _averageValue = new AverageValue();
            //_gyroscopeThread = new Thread(new ThreadStart(ReadGyroscopeValues));

            //Thread.Sleep(5000);

            Console.WriteLine("Starting Gyroscope readings");

            while(true)
            {
                ReadGyroscopeValues();
                //ReadAccelorometerValues();
                Thread.Sleep(500);
            }
        }

        private void ReadGyroscopeValues()
        {
            Vector3 gyroscopeReading = _gyroscope.GetGyroscopeReading();
            // Increment the number of readngs to help build up the average value over time
            _averageValue.UpdateValues(gyroscopeReading);
            // Console.WriteLine($"Retrieving gyroscope reading:");
            Console.WriteLine($"Number Of Readings: { _averageValue.NumberOfReadings } | X:{_averageValue.X.ToString("n4") } | Y: {_averageValue.Y.ToString("n4") } | Z: {_averageValue.Z.ToString("n4") }");
        }
    }
}
