using Iot.Device.Imu;
using System;
using System.Device.I2c;
using System.Numerics;
using System.Threading;

namespace TorsoServos.DeviceManagers
{
    public class GyroscopeManager
    {
        private readonly int _gyroscopeI2CDataPin;
        private readonly int _gyroscopeI2CClockPin;
        private bool _active = false;

        private Thread _gyroscopeThread;
        private Vector3 averagedGyroscopeBaseLineReadings;
        private Vector3 averagedAccelerometerBaseLineReadings;

        public Vector3 CurrentGyroscopeReading;
        public Vector3 CurrentAccelerometerReading;

        public bool GyroscopeMotionDetected = false;
        public bool AccelerometerMotionDetected = false;

        private I2cConnectionSettings _gyroscopeI2CConnectionSettings;
        private Mpu6050 _gyroscope;

        public delegate void MotionDetectedDelegate(object sender, EventArgs e);

        public event MotionDetectedDelegate MotionDetected;

        public GyroscopeManager(int DataPin, int ClockPin)
        {
            _gyroscopeI2CDataPin = DataPin;
            _gyroscopeI2CClockPin = ClockPin;
        }

        public void Initialise()
        {
            _gyroscopeI2CConnectionSettings = new I2cConnectionSettings(1, Mpu6050.DefaultI2cAddress);
            _gyroscope = new Mpu6050(I2cDevice.Create(_gyroscopeI2CConnectionSettings));

            // Get a baseline set of readings during initialisation
            GetBaselineReadings();

            _gyroscopeThread = new Thread(() =>
            {
                while (_active)
                {
                    CurrentGyroscopeReading = _gyroscope.GetGyroscopeReading();
                    CurrentAccelerometerReading = _gyroscope.GetAccelerometer();

                    GyroscopeMotionDetected = IsValueOutsideBaseRange(CurrentGyroscopeReading.X, averagedGyroscopeBaseLineReadings.X, 8) ||
                        IsValueOutsideBaseRange(CurrentGyroscopeReading.Y, averagedGyroscopeBaseLineReadings.Y, 8) ||
                        IsValueOutsideBaseRange(CurrentGyroscopeReading.Z, averagedGyroscopeBaseLineReadings.Z, 8);

                    AccelerometerMotionDetected = IsValueOutsideBaseRange(CurrentAccelerometerReading.X, averagedAccelerometerBaseLineReadings.X, 5) ||
                        IsValueOutsideBaseRange(CurrentAccelerometerReading.Y, averagedAccelerometerBaseLineReadings.Y, 3) ||
                        IsValueOutsideBaseRange(CurrentAccelerometerReading.Z, averagedAccelerometerBaseLineReadings.Z, 3);

                    MotionDetected?.Invoke(this, new EventArgs());

                    Thread.Sleep(500); // take a reading every half second
                }
            });
        }

        public void GetBaselineReadings()
        {
            int readCount = 0;
            int readLimit = 20;
            Vector3[] _initialGyroscopeBaseLineReadings = new Vector3[readLimit]; // take 20 readings to get a baseline
            Vector3[] _initialAccelerometerBaseLineReadings = new Vector3[readLimit]; // take 20 readings to get a baseline

            averagedGyroscopeBaseLineReadings = new Vector3();
            averagedAccelerometerBaseLineReadings = new Vector3();

            for (int i = 0; i <= readLimit; i++)
            {
                if (readCount < readLimit - 1)
                {
                    Vector3 gyroscopeReading = _gyroscope.GetGyroscopeReading();
                    Vector3 accelerometerReading = _gyroscope.GetAccelerometer();

                    _initialGyroscopeBaseLineReadings[readCount] = gyroscopeReading;
                    _initialAccelerometerBaseLineReadings[readCount] = accelerometerReading;
                    readCount++;
                }
                else
                {
                    averagedGyroscopeBaseLineReadings = GetAverageOfAllValues(_initialGyroscopeBaseLineReadings);
                    averagedAccelerometerBaseLineReadings = GetAverageOfAllValues(_initialAccelerometerBaseLineReadings);
                }

                Thread.Sleep(100);
            }
        }

        public void StartReading()
        {
            _active = true;
            _gyroscopeThread.Start();

        }

        public void StopReading()
        {
            _active = false;
            _gyroscopeThread.Suspend();
        }

        private Vector3 GetAverageOfAllValues(Vector3[] readings)
        {
            Vector3 sum = new Vector3();

            foreach (var reading in readings)
            {
                sum.X += Math.Abs(reading.X);
                sum.Y += Math.Abs(reading.Y);
                sum.Z += Math.Abs(reading.Z);
            }

            Vector3 result = new Vector3();

            result.X = sum.X / readings.Length;
            result.Y = sum.Y / readings.Length;
            result.Z = sum.Z / readings.Length;

            return result;
        }

        private bool IsValueOutsideBaseRange(double value, double baseValue, double valueTolerance)
        {
            double difference = Math.Abs(Math.Abs(value) - Math.Abs(baseValue));
            if (Math.Abs(difference) > Math.Abs(valueTolerance))
                return true;
            return false;
        }


    }
}
