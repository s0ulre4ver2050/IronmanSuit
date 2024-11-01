using Iot.Device.ServoMotor;
using nanoFramework.Hardware.Esp32;
using ServoZeroSetter.HardwareConfigurations;
using System;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;

namespace ServoZeroSetter
{
    public class Program
    {
        public static void Main()
        {
            /*TiltSensorReader sensorReader = new TiltSensorReader();

            sensorReader.Initialise();

            Thread.Sleep(Timeout.Infinite);*/

            int servoType = 1; // 1 - MG90S, 2 - MF90

            IServoManager servoManager = null;

            if (servoType == 1)
            {
                servoManager = new MG90SServoManager();
            }
            else if (servoType == 2)
            {
                servoManager = new MF90ServoManager();
            }

            servoManager.Initialise();

            servoManager.SetToStartPosition();

            bool running = true;

            while (running)
            {
                Console.WriteLine("Toggling mechanism state!");
                servoManager.ToggleMechanismState();
                Thread.Sleep(5000); // Wait 5 seconds before toggling the state of the servo
            }


            Thread.Sleep(Timeout.Infinite);
        }
    }
}
