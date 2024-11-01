using System;

namespace Core.HardwareConfigurations
{
    public class ServoStep
    {
        public int Order { get; set; }
        public int Angle { get; set; }
        public int ThreadSleepDuration { get; set; } = 7; // default to 100 ms 
    }
}
