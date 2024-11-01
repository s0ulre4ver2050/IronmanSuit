using Iot.Device.Button;
using Iot.Device.ServoMotor;
using nanoFramework.Hardware.Esp32;
using Iot.Device.Ws28xx.Esp32;
using Core.HardwareConfigurations;
using System;
using System.Device.Pwm;
using System.Threading;
using System.Drawing;

namespace RightArmServos.Models
{
    public class RightBracer
    {
        private Thread _lightingThread;

        private Timer _timer;

        private readonly RepulsorAnimationStep[] _repulsorAnimationStep;

        private readonly DeviceFunction _rocketServoDeviceFunction;
        private readonly DeviceFunction _outerPanelServoDeviceFunction;
        private readonly DeviceFunction _innerPanelServoDeviceFunction;
        private readonly DeviceFunction _repulsorDeviceFunction;

        private PwmChannel _rocketServoPWMChannel;
        private PwmChannel _outerPanelPWMChannel;
        private PwmChannel _innerPanelPWMChannel;

        private ServoMotor _rocketServoMotor;
        private ServoMotor _outerPanelServoMotor;
        private ServoMotor _innerPanelServoMotor;

        private readonly Ws2812c _neo;
        private readonly Random _random;
        private readonly BitmapImage _image;

        private readonly int _rocketServoPinNumber;
        private readonly int _outerPanelPinNumber;
        private readonly int _innerPanelPinNumber;
        private readonly int _repulsorPinNumber;

        private readonly double _rocketServoMinAngle = 0;
        private readonly double _outerPanelMinAngle = 0;
        private readonly double _innerPanelMinAngle = 0;

        private readonly double _rocketServoMaxAngle = 85;
        private readonly double _outerPanelMaxAngle = 45;
        private readonly double _innerPanelMaxAngle = 45;

        private readonly int _buttonPin;
        private readonly GpioButton _gpioButton;

        private readonly int _servoMotorSleepDuration = 10;

        private ComponentState _componentState;

        private ServoStep[] _rocketStepsOpen = new ServoStep[29]
        {
            new ServoStep() { Angle = 0},
            new ServoStep() { Angle = 3},
            new ServoStep() { Angle = 6},
            new ServoStep() { Angle = 9 },
            new ServoStep() { Angle = 12 },
            new ServoStep() { Angle = 15 },
            new ServoStep() { Angle = 18 },
            new ServoStep() { Angle = 21 },
            new ServoStep() { Angle = 24 },
            new ServoStep() { Angle = 27 },
            new ServoStep() { Angle = 30 },
            new ServoStep() { Angle = 33 },
            new ServoStep() { Angle = 36 },
            new ServoStep() { Angle = 39 },
            new ServoStep() { Angle = 42 },
            new ServoStep() { Angle = 45 },
            new ServoStep() { Angle = 48 },
            new ServoStep() { Angle = 51 },
            new ServoStep() { Angle = 54 },
            new ServoStep() { Angle = 57 },
            new ServoStep() { Angle = 60 },
            new ServoStep() { Angle = 63 },
            new ServoStep() { Angle = 66 },
            new ServoStep() { Angle = 69 },
            new ServoStep() { Angle = 72 },
            new ServoStep() { Angle = 75 },
            new ServoStep() { Angle = 78 },
            new ServoStep() { Angle = 81 },
            new ServoStep() { Angle = 84 }
        };

        private ServoStep[] _rocketStepsClose = new ServoStep[29]
        {
            new ServoStep() { Angle = 84 },
            new ServoStep() { Angle = 81 },
            new ServoStep() { Angle = 78 },
            new ServoStep() { Angle = 75 },
            new ServoStep() { Angle = 72 },
            new ServoStep() { Angle = 69 },
            new ServoStep() { Angle = 66 },
            new ServoStep() { Angle = 63 },
            new ServoStep() { Angle = 60 },
            new ServoStep() { Angle = 57 },
            new ServoStep() { Angle = 54 },
            new ServoStep() { Angle = 51 },
            new ServoStep() { Angle = 48 },
            new ServoStep() { Angle = 45 },
            new ServoStep() { Angle = 42 },
            new ServoStep() { Angle = 39 },
            new ServoStep() { Angle = 36 },
            new ServoStep() { Angle = 33 },
            new ServoStep() { Angle = 30 },
            new ServoStep() { Angle = 27 },
            new ServoStep() { Angle = 24 },
            new ServoStep() { Angle = 21 },
            new ServoStep() { Angle = 18 },
            new ServoStep() { Angle = 15 },
            new ServoStep() { Angle = 12 },
            new ServoStep() { Angle = 9 },
            new ServoStep() { Angle = 6 },
            new ServoStep() { Angle = 3 },
            new ServoStep() { Angle = 0 }
        };

        private ServoStep[] _outerPanelSteps = new ServoStep[46]
        {
            new ServoStep() { Angle = 0 },
            new ServoStep() { Angle = 1 },
            new ServoStep() { Angle = 2 },
            new ServoStep() { Angle = 3 },
            new ServoStep() { Angle = 4 },
            new ServoStep() { Angle = 5 },
            new ServoStep() { Angle = 6 },
            new ServoStep() { Angle = 7 },
            new ServoStep() { Angle = 8 },
            new ServoStep() { Angle = 9 },
            new ServoStep() { Angle = 10 },
            new ServoStep() { Angle = 11 },
            new ServoStep() { Angle = 12 },
            new ServoStep() { Angle = 13 },
            new ServoStep() { Angle = 14 },
            new ServoStep() { Angle = 15 },
            new ServoStep() { Angle = 16 },
            new ServoStep() { Angle = 17 },
            new ServoStep() { Angle = 18 },
            new ServoStep() { Angle = 19 },
            new ServoStep() { Angle = 20 },
            new ServoStep() { Angle = 21 },
            new ServoStep() { Angle = 22 },
            new ServoStep() { Angle = 23 },
            new ServoStep() { Angle = 24 },
            new ServoStep() { Angle = 25 },
            new ServoStep() { Angle = 26 },
            new ServoStep() { Angle = 27 },
            new ServoStep() { Angle = 28 },
            new ServoStep() { Angle = 29 },
            new ServoStep() { Angle = 30 },
            new ServoStep() { Angle = 31 },
            new ServoStep() { Angle = 32 },
            new ServoStep() { Angle = 33 },
            new ServoStep() { Angle = 34 },
            new ServoStep() { Angle = 35 },
            new ServoStep() { Angle = 36 },
            new ServoStep() { Angle = 37 },
            new ServoStep() { Angle = 38 },
            new ServoStep() { Angle = 39 },
            new ServoStep() { Angle = 40 },
            new ServoStep() { Angle = 41 },
            new ServoStep() { Angle = 42 },
            new ServoStep() { Angle = 43 },
            new ServoStep() { Angle = 44 },
            new ServoStep() { Angle = 45 }
        };

        private ServoStep[] _innerPanelSteps = new ServoStep[46]
        {
            new ServoStep() { Angle = 45 },
            new ServoStep() { Angle = 44 },
            new ServoStep() { Angle = 43 },
            new ServoStep() { Angle = 42 },
            new ServoStep() { Angle = 41 },
            new ServoStep() { Angle = 40 },
            new ServoStep() { Angle = 39 },
            new ServoStep() { Angle = 38 },
            new ServoStep() { Angle = 37 },
            new ServoStep() { Angle = 36 },
            new ServoStep() { Angle = 35 },
            new ServoStep() { Angle = 34 },
            new ServoStep() { Angle = 33 },
            new ServoStep() { Angle = 32 },
            new ServoStep() { Angle = 31 },
            new ServoStep() { Angle = 30 },
            new ServoStep() { Angle = 29 },
            new ServoStep() { Angle = 28 },
            new ServoStep() { Angle = 27 },
            new ServoStep() { Angle = 26 },
            new ServoStep() { Angle = 25 },
            new ServoStep() { Angle = 24 },
            new ServoStep() { Angle = 23 },
            new ServoStep() { Angle = 22 },
            new ServoStep() { Angle = 21 },
            new ServoStep() { Angle = 20 },
            new ServoStep() { Angle = 19 },
            new ServoStep() { Angle = 18 },
            new ServoStep() { Angle = 17 },
            new ServoStep() { Angle = 16 },
            new ServoStep() { Angle = 15 },
            new ServoStep() { Angle = 14 },
            new ServoStep() { Angle = 13 },
            new ServoStep() { Angle = 12 },
            new ServoStep() { Angle = 11 },
            new ServoStep() { Angle = 10 },
            new ServoStep() { Angle = 9 },
            new ServoStep() { Angle = 8 },
            new ServoStep() { Angle = 7 },
            new ServoStep() { Angle = 6 },
            new ServoStep() { Angle = 5 },
            new ServoStep() { Angle = 4 },
            new ServoStep() { Angle = 3 },
            new ServoStep() { Angle = 2 },
            new ServoStep() { Angle = 1 },
            new ServoStep() { Angle = 0 },
        };

        private readonly int _pixelCount = 7;
        private float _currentBrightness;
        private int _redValue;
        private int _greenValue;
        private int _blueValue;
        private bool _isGettingBrighter = true;
        private readonly float _minBrightness = 0.2f;// value out of 10 to represent 10 percent intervals
        private readonly float _maxBrightness = 1f; // value our of 10 to represent 10 percent intervals (nearer 1 is closer to 100%)

        private bool _canAnimateRocket = true;
        private Thread _rocketAnimationLockoutThread;

        public enum ComponentState
        {
            Closed = 0,
            Open = 1,
            Testing = 2,
            Initialising = 3
        }

        public ComponentState CurrentState { get { return _componentState; } }

        public RightBracer(
            int buttonPin,
            int outerServoPin,
            int innerServoPin,
            int rocketServoPin,
            int repulsorPin,
            DeviceFunction outerDeviceFunction,
            DeviceFunction innerDeviceFunction,
            DeviceFunction rocketDeviceFunction,
            DeviceFunction repulsorDeviceFunction)
        {
            _buttonPin = buttonPin;
            _outerPanelPinNumber = outerServoPin;
            _innerPanelPinNumber = innerServoPin;
            _rocketServoPinNumber = rocketServoPin;
            _repulsorPinNumber = repulsorPin;

            _outerPanelServoDeviceFunction = outerDeviceFunction;
            _innerPanelServoDeviceFunction = innerDeviceFunction;
            _rocketServoDeviceFunction = rocketDeviceFunction;
            _repulsorDeviceFunction = repulsorDeviceFunction;

            _gpioButton = new GpioButton(_buttonPin);

            _neo = new Ws2812c(_repulsorPinNumber, _pixelCount);
            _currentBrightness = _minBrightness;

            _random = new Random();

            _repulsorAnimationStep = new RepulsorAnimationStep[21]
            {
                new RepulsorAnimationStep() { RedValue = 74, GreenValue = 178, BlueValue = 232 }, // 100 %
                new RepulsorAnimationStep() { RedValue = 71, GreenValue = 171, BlueValue = 223 }, // 96 %
                new RepulsorAnimationStep() { RedValue = 68, GreenValue = 164, BlueValue = 213 }, // 92 %
                new RepulsorAnimationStep() { RedValue = 65, GreenValue = 157, BlueValue = 204 }, // 88 %
                new RepulsorAnimationStep() { RedValue = 62, GreenValue = 150, BlueValue = 195 }, // 84 %
                new RepulsorAnimationStep() { RedValue = 59, GreenValue = 142, BlueValue = 186 }, // 80 %
                new RepulsorAnimationStep() { RedValue = 56, GreenValue = 135, BlueValue = 176 }, // 76 %
                new RepulsorAnimationStep() { RedValue = 53, GreenValue = 128, BlueValue = 167 }, // 72 %
                new RepulsorAnimationStep() { RedValue = 50, GreenValue = 121, BlueValue = 158 }, // 68 %
                new RepulsorAnimationStep() { RedValue = 47, GreenValue = 114, BlueValue = 148 }, // 64 %
                new RepulsorAnimationStep() { RedValue = 44, GreenValue = 107, BlueValue = 139 }, // 60 %
                new RepulsorAnimationStep() { RedValue = 41, GreenValue = 100, BlueValue = 130 }, // 56 %
                new RepulsorAnimationStep() { RedValue = 38, GreenValue = 93,  BlueValue = 121 }, // 52 %
                new RepulsorAnimationStep() { RedValue = 36, GreenValue = 85,  BlueValue = 111 }, // 48 %
                new RepulsorAnimationStep() { RedValue = 33, GreenValue = 78,  BlueValue = 102 }, // 44 %
                new RepulsorAnimationStep() { RedValue = 30, GreenValue = 71,  BlueValue = 93 }, // 40 %
                new RepulsorAnimationStep() { RedValue = 27, GreenValue = 64,  BlueValue = 84 }, // 36 %
                new RepulsorAnimationStep() { RedValue = 24, GreenValue = 57,  BlueValue = 74 }, // 32 %
                new RepulsorAnimationStep() { RedValue = 21, GreenValue = 50,  BlueValue = 65 }, // 28 %
                new RepulsorAnimationStep() { RedValue = 18, GreenValue = 43,  BlueValue = 56 }, // 24 %
                new RepulsorAnimationStep() { RedValue = 15, GreenValue = 36,  BlueValue = 46 }, // 20 %
            };
        }

        public void Initialise()
        {
            _componentState = ComponentState.Initialising;

            Configuration.SetPinFunction(_outerPanelPinNumber, _outerPanelServoDeviceFunction);
            Configuration.SetPinFunction(_innerPanelPinNumber, _innerPanelServoDeviceFunction);
            Configuration.SetPinFunction(_rocketServoPinNumber, _rocketServoDeviceFunction);
            Configuration.SetPinFunction(_repulsorPinNumber, _repulsorDeviceFunction);

            _outerPanelPWMChannel = PwmChannel.CreateFromPin(pin: _outerPanelPinNumber, frequency: 50, dutyCyclePercentage: 0.5);
            _innerPanelPWMChannel = PwmChannel.CreateFromPin(pin: _innerPanelPinNumber, frequency: 50, dutyCyclePercentage: 0.5);
            _rocketServoPWMChannel = PwmChannel.CreateFromPin(pin: _rocketServoPinNumber, frequency: 50, dutyCyclePercentage: 0.5);

            _outerPanelServoMotor = new ServoMotor(_outerPanelPWMChannel,
                MG90SServoSettings.MaximumAngle,
                MG90SServoSettings.MinimumPulseWidth,
                MG90SServoSettings.MaximumPulseWidth);

            _innerPanelServoMotor = new ServoMotor(_innerPanelPWMChannel,
                MG90SServoSettings.MaximumAngle,
                MG90SServoSettings.MinimumPulseWidth,
                MG90SServoSettings.MaximumPulseWidth);

            _rocketServoMotor = new ServoMotor(_rocketServoPWMChannel,
                MG90SServoSettings.MaximumAngle,
                MG90SServoSettings.MinimumPulseWidth,
                MG90SServoSettings.MaximumPulseWidth);

            _lightingThread = new Thread(new ThreadStart(Animate));

            _gpioButton.IsHoldingEnabled = true;

            PerformSystemTest();

            Console.WriteLine("Starting lighting animations");
            _lightingThread.Start();

            _gpioButton.Holding += (sender, e) =>
            {
                if (_gpioButton.IsPressed)
                {
                    Open();
                }
                else
                {
                    Close();
                }
            };
        }

        protected void Open()
        {
            if (!_componentState.Equals(ComponentState.Open) && _canAnimateRocket)
            {
                try
                {
                    _innerPanelServoMotor.Start();
                    _outerPanelServoMotor.Start();
                    _rocketServoMotor.Start();

                    OpenPanels();
                    Thread.Sleep(400);
                    OpenRocket();

                    _innerPanelServoMotor.Stop();
                    _outerPanelServoMotor.Stop();
                    _rocketServoMotor.Stop();

                    _componentState = ComponentState.Open;

                    Console.WriteLine("Bracer Open");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }

        protected void Close()
        {
            if (!_componentState.Equals(ComponentState.Closed) && _canAnimateRocket)
            {
                try
                {
                    _innerPanelServoMotor.Start();
                    _outerPanelServoMotor.Start();
                    _rocketServoMotor.Start();

                    CloseRocket();
                    Thread.Sleep(400);
                    ClosePanels();

                    _innerPanelServoMotor.Stop();
                    _outerPanelServoMotor.Stop();
                    _rocketServoMotor.Stop();

                    _componentState = ComponentState.Closed;

                    Console.WriteLine("Bracer Closed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }

        protected void PerformSystemTest()
        {
            int interval = 1000;
            // Start with Testing the side Panels
            Console.WriteLine($"Starting system test");

            Open();
            Thread.Sleep(interval);
            Close();
            Thread.Sleep(interval);

            Console.WriteLine($"System test complete");

            Console.WriteLine("Setting Start Position");
            Close();
        }

        private void OpenRocket()
        {
            Console.WriteLine($"Opening Rocket");

            foreach (ServoStep step in _rocketStepsOpen)
            {
                _rocketServoMotor.WriteAngle(step.Angle);
                Thread.Sleep(step.ThreadSleepDuration);
            }
        }

        private void CloseRocket()
        {
            Console.WriteLine($"Closing Rocket");

            foreach (ServoStep step in _rocketStepsClose)
            {
                _rocketServoMotor.WriteAngle(step.Angle);
                Thread.Sleep(step.ThreadSleepDuration);
            }
        }

        private void OpenPanels()
        {
            Console.WriteLine($"Opening Panels");

            for (int i = 0; i != _innerPanelSteps.Length - 1; i++)
            {
                ServoStep innerStep = _innerPanelSteps[i];
                ServoStep outerStep = _outerPanelSteps[i];
                _innerPanelServoMotor.WriteAngle(innerStep.Angle);
                _outerPanelServoMotor.WriteAngle(outerStep.Angle);
                Thread.Sleep(innerStep.ThreadSleepDuration);
            }
        }

        private void ClosePanels()
        {
            Console.WriteLine($"Closing Panels");

            for (int i = _innerPanelSteps.Length - 1; i != 0; i--)
            {
                ServoStep innerStep = _innerPanelSteps[i];
                ServoStep outerStep = _outerPanelSteps[i];
                _innerPanelServoMotor.WriteAngle(innerStep.Angle);
                _outerPanelServoMotor.WriteAngle(outerStep.Angle);
                Thread.Sleep(innerStep.ThreadSleepDuration);
            }
        }

        public void Animate()
        {
            while (_lightingThread.IsAlive)
            {
                _canAnimateRocket = false;
                
                BrightenRepulsorLights();
                Thread.Sleep(100);
                DimRepulsorLights();

                int interval = _random.Next(300); // Choose a random interval once every x seconds

                _canAnimateRocket = true;

                Thread.Sleep(interval * 1000);
            }
        }

        private void DimRepulsorLights()
        {
            // increase the index to get a dimmer light
            for (int i = 0; i != _repulsorAnimationStep.Length-1; i++)
            {
                RepulsorAnimationStep currentStep = _repulsorAnimationStep[i];
                SetNeoPixels(currentStep);
            }
        }

        private void BrightenRepulsorLights()
        {
            // decrease the index to get a brighter light
            for (int i = _repulsorAnimationStep.Length-1; i != 0; i--)
            {
                RepulsorAnimationStep currentStep = _repulsorAnimationStep[i];
                SetNeoPixels(currentStep);
            }
        }

        private void SetNeoPixels(RepulsorAnimationStep repulsorAnimationStep)
        {
            for (var j = 0; j < _neo.Image.Width; j++)
            {
                Color color = Color.FromArgb(0, repulsorAnimationStep.RedValue, repulsorAnimationStep.GreenValue, repulsorAnimationStep.BlueValue);
                _neo.Image.SetPixel(j, 0, color);

            }
            _neo.Update();
        }

        private void RocketAnimationLockoutTimer()
        {
            _canAnimateRocket = false;
            Thread.Sleep(15000);
            _canAnimateRocket = true;
        }
    }

}
