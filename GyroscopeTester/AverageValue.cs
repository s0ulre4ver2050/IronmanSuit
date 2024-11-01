using System;
using System.Numerics;

namespace GyroscopeTester
{
    public class AverageValue
    {
        private double currentXAverageValue = 0;
        private double currentYAverageValue = 0;
        private double currentZAverageValue = 0;
        private int maxNumberOfReadings = 50;
        private int numberOfReadings = 0;

        public int NumberOfReadings { get { return numberOfReadings; } }
        public double X { get { return currentXAverageValue; } }
        public double Y { get { return currentYAverageValue; } }
        public double Z { get { return currentZAverageValue; } }

        public void UpdateValues(Vector3 reading)
        {
            if (numberOfReadings < maxNumberOfReadings)
            {
                numberOfReadings++;

                currentXAverageValue = (currentXAverageValue + Math.Abs(reading.X)) / NumberOfReadings;
                currentYAverageValue = (currentYAverageValue + Math.Abs(reading.Y)) / NumberOfReadings;
                currentZAverageValue = (currentZAverageValue + Math.Abs(reading.Z)) / NumberOfReadings;
            }
            
            if (numberOfReadings >= maxNumberOfReadings)
            {
                numberOfReadings = 0;
                currentXAverageValue = Math.Abs(reading.X);
                currentYAverageValue = Math.Abs(reading.Y);
                currentZAverageValue = Math.Abs(reading.Z);
            }
        }
    }
}
