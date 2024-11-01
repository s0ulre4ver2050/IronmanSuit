using LeftArmServos.Models;
using nanoFramework.Hardware.Esp32;
using System.Threading;

namespace LeftArmServos
{
    public class Program
    {
        public static void Main()
        {
            LeftBracer leftBracer = new LeftBracer(
                buttonPin: 12,
                outerServoPin: 33,
                innerServoPin: 25,
                rocketServoPin: 26,
                repulsorPin: 27,
                outerDeviceFunction: DeviceFunction.PWM1,
                innerDeviceFunction: DeviceFunction.PWM2,
                rocketDeviceFunction: DeviceFunction.PWM3,
                repulsorDeviceFunction: DeviceFunction.PWM4);
            leftBracer.Initialise();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
