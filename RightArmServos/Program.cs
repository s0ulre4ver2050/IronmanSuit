using nanoFramework.Hardware.Esp32;
using RightArmServos.Models;
using System;
using System.Diagnostics;
using System.Threading;

namespace RightArmServos
{
    public class Program
    {
        public static void Main()
        {
            RightBracer rightBracer = new RightBracer(
                buttonPin: 12,
                outerServoPin: 33,
                innerServoPin: 25, 
                rocketServoPin: 26,
                repulsorPin: 27,
                outerDeviceFunction: DeviceFunction.PWM1,
                innerDeviceFunction: DeviceFunction.PWM2,
                rocketDeviceFunction: DeviceFunction.PWM3,
                repulsorDeviceFunction: DeviceFunction.PWM4);
            rightBracer.Initialise();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
