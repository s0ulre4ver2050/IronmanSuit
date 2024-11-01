using Iot.Device.Imu;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.I2c;
using System.Diagnostics;
using System.Numerics;
using System.Threading;

namespace GyroscopeTester
{
    public class Program
    {
        public static void Main()
        {
            int dataPin = 18;
            int clockPin = 19;

            Gyroscope gyroscope = new Gyroscope();
            gyroscope.Initialise(dataPin, clockPin);

            //I2CScanner scanner = new I2CScanner();
            //scanner.Scan(dataPin, clockPin);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
