using Mark3Helmet.Models;
using nanoFramework.Hardware.Esp32;
using System;
using System.Diagnostics;
using System.Threading;

namespace Mark3Helmet
{
    public class Program
    {
        public static void Main()
        {
            Helmet helmet = new Helmet(25, 26, DeviceFunction.PWM1, DeviceFunction.PWM2, 12, 17, DeviceFunction.PWM3);
            helmet.Initialise();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
