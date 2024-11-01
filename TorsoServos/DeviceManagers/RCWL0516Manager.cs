using System;
using System.Device.Adc;
using System.Threading;

namespace TorsoServos.DeviceManagers
{
    public class RCWL0516Manager
    {
        private AdcController _adcController;
        private AdcChannel _radarSensorChannel;
        private readonly int _radarChannel = 7; // This maps to pin 35
        private bool _active = false;

        private int _previousRadarSensorReadingValue = 0;
        private Thread _sensorReadingThread;

        /// <summary>
        /// Delegate for button pressed.
        /// </summary>
        /// <param name="sender">Caller object.</param>
        /// <param name="e">Arguments for invoked delegate.</param>
        public delegate void MotionDetectedDelegate(object sender, EventArgs e);

        public event MotionDetectedDelegate MotionDetected;

        public int SensorReadingValue;

        public RCWL0516Manager()
        {
            _adcController = new AdcController();

        }

        public void Initialise()
        {
            _radarSensorChannel = _adcController.OpenChannel(_radarChannel);

            _sensorReadingThread = new Thread(() =>
            {
                while (_active)
                {
                    SensorReadingValue = _radarSensorChannel.ReadValue();
                    if (_previousRadarSensorReadingValue != SensorReadingValue)
                    {
                        _previousRadarSensorReadingValue = SensorReadingValue;
                        HandleMotionDetectionChanged();
                    }

                    Thread.Sleep(2000); // The device only updates its reading once every 2 seconds so we should only check then
                }
            });
        }

        public void StartReading()
        {
            _active = true;
            _sensorReadingThread.Start();
        }

        public void StopReading()
        {
            _active = false;
            _sensorReadingThread.Suspend();
        }

        public void HandleMotionDetectionChanged()
        {
            MotionDetected?.Invoke(this, new EventArgs());
        }
    }
}
