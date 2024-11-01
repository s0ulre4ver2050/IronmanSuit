using System;

namespace Core.HardwareConfigurations
{
    public static class MF90ServoSettings
    {
        public static double MaximumAngle = 360;
        public static double MinimumPulseWidth = 500;
        public static double MaximumPulseWidth = 2500;
        public static double OperatingVoltage = 6.0;
        public static double MSPerDegree = 0.0013333333333333;

        public static int CalculateThreadSleepFromAngleDifferenceToMove(double startAngle, double endAngle)
        {
            int angleDifference = (int)Math.Ceiling(startAngle - endAngle);
            return (int)(angleDifference * MSPerDegree) * 1000;
        }
    }
}
