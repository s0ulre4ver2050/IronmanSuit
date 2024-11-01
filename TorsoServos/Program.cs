using nanoFramework.Hardware.Esp32;
using System;
using System.Diagnostics;
using System.Threading;
using TorsoServos.Models;

namespace TorsoServos
{
    public class Program
    {
        public static void Main()
        {
            Torso torso = new Torso(
                leftClaviclePin:12,
                leftShoulderCoverPin: 27,                
                leftShoulderTurretPin: 26,
                leftUpperBackFlapPin: 25,

                rightClaviclePin: 16,
                rightShoulderCoverPin:17,                
                rightShoulderTurretPin:21,                
                rightUpperBackFlapPin:22,                

                gyroscopeI2CDataPin: 18,
                gyroscopeI2CClockPin: 19
                );

            torso.Initialise();
        }
    }
}
