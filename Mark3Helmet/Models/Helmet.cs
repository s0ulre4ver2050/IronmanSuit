using Core.HardwareConfigurations;
using Iot.Device.Button;
using Iot.Device.ServoMotor;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;

namespace Mark3Helmet.Models
{
    public class Helmet
    {
        private readonly int _buttonPin;
        private readonly GpioButton _gpioButton;

        private readonly DeviceFunction _eyeLEDDeviceFunction;
        private readonly int _eyeLEDPin;
        private PwmChannel _eyeLEDPWM;

        private readonly DeviceFunction _leftServoDeviceFunction;
        private PwmChannel _leftPWMChannel;
        private ServoMotor _leftServoMotor;
        private readonly int _leftServoPinNumber;
        private readonly double _leftServoMinAngle = 160;
        private readonly double _leftServoMaxAngle = 20;

        private readonly DeviceFunction _rightServoDeviceFunction;
        private PwmChannel _rightPWMChannel;
        private ServoMotor _rightServoMotor;
        private readonly int _rightServoPinNumber;
        private readonly double _rightServoMinAngle = 0;
        private readonly double _rightServoMaxAngle = 140;

        private int _buttonPressCount = 0;
        private Timer? _timer = null;

        private readonly EyeAnimationStep[] _eyeAnimationSteps = new EyeAnimationStep[10]
            {
                new EyeAnimationStep() { DutyCycle = 0.8, IntervalMS = 15 },
                new EyeAnimationStep() { DutyCycle = 0.0, IntervalMS = 25 },
                new EyeAnimationStep() { DutyCycle = 0.8, IntervalMS = 25 },
                new EyeAnimationStep() { DutyCycle = 0.0, IntervalMS = 50 },
                new EyeAnimationStep() { DutyCycle = 0.8, IntervalMS = 50 },
                new EyeAnimationStep() { DutyCycle = 0.0, IntervalMS = 60 },
                new EyeAnimationStep() { DutyCycle = 0.8, IntervalMS = 75 },
                new EyeAnimationStep() { DutyCycle = 0.0, IntervalMS = 70 },
                new EyeAnimationStep() { DutyCycle = 0.8, IntervalMS = 100 },
                new EyeAnimationStep() { DutyCycle = 0.0, IntervalMS = 80 }
            };

        private ComponentState _currentState;
        public enum ComponentState
        {
            Closed = 0,
            Open = 1
        }

        private EyeStates _eyeState;
        public enum EyeStates
        {
            Off = 0,
            On = 1
        }

        public ComponentState CurrentState { get { return _currentState; } }
        public EyeStates EyeState { get { return _eyeState; } }

        public Helmet(int leftServoPin, int rightServoPin, DeviceFunction leftServoDeviceFunction, DeviceFunction rightServoDeviceFunction, int buttonPin, int eyeLEDPin, DeviceFunction eyeLEDDeviceFunction)
        {
            _leftServoPinNumber = leftServoPin;
            _rightServoPinNumber = rightServoPin;
            _leftServoDeviceFunction = leftServoDeviceFunction;
            _rightServoDeviceFunction = rightServoDeviceFunction;

            _buttonPin = buttonPin;
            _gpioButton = new GpioButton(_buttonPin);

            _eyeLEDPin = eyeLEDPin;
            _eyeLEDDeviceFunction = eyeLEDDeviceFunction;
        }

        public void Initialise()
        {
            // Setup the pin state for the ESP32
            Configuration.SetPinFunction(_leftServoPinNumber, _leftServoDeviceFunction);
            Configuration.SetPinFunction(_rightServoPinNumber, _rightServoDeviceFunction);
            Configuration.SetPinFunction(_eyeLEDPin, _eyeLEDDeviceFunction);

            // Configure the PWMChannel
            _leftPWMChannel = PwmChannel.CreateFromPin(_leftServoPinNumber, 50);
            _rightPWMChannel = PwmChannel.CreateFromPin(_rightServoPinNumber, 50);

            _eyeLEDPWM = PwmChannel.CreateFromPin(_eyeLEDPin, 40000);

            // Configure the servo motor instance
            _leftServoMotor = new ServoMotor(
                _leftPWMChannel,
                MG90SServoSettings.MaximumAngle,
                MG90SServoSettings.MinimumPulseWidth,
                MG90SServoSettings.MaximumPulseWidth);

            _rightServoMotor = new ServoMotor(
                _rightPWMChannel,
                MG90SServoSettings.MaximumAngle,
                MG90SServoSettings.MinimumPulseWidth,
                MG90SServoSettings.MaximumPulseWidth);

            _eyeLEDPWM.Start();
            _eyeState = EyeStates.Off;

            _gpioButton.Press += (sender, e) =>
            {
                _buttonPressCount++;

                if (_timer == null)
                {
                    _timer = new Timer(TimerElapsed, null, 500, Timeout.Infinite);
                }
            };

            SetToStartPosition();
        }

        public void TimerElapsed(object state)
        {
            if (_buttonPressCount == 1)
            {
                Animate();
            }

            if (_buttonPressCount == 2)
            {
                ToggleEyeState();
            }

            _buttonPressCount = 0;

            _timer.Dispose();
            _timer = null;
        }

        private void SetToStartPosition()
        {
            _leftServoMotor.Start();
            _rightServoMotor.Start();
            _leftServoMotor.WriteAngle(_leftServoMinAngle);
            _rightServoMotor.WriteAngle(_rightServoMinAngle);
            Thread.Sleep(500); // Pause the thread for 1/2 a second to allow the motors to do their job
            _leftServoMotor.Stop();
            _rightServoMotor.Stop();
            StartUpEyeAnimation();
            _currentState = ComponentState.Closed;
        }

        private void StartUpEyeAnimation()
        {
            _eyeLEDPWM.DutyCycle = 0.0;
            Thread.Sleep(250);

            foreach (var eyeAnimationStep in _eyeAnimationSteps)
            {
                _eyeLEDPWM.DutyCycle = eyeAnimationStep.DutyCycle;
                Thread.Sleep(eyeAnimationStep.IntervalMS);
            }
            _eyeLEDPWM.DutyCycle = 0.8;
        }

        private void OpenFaceplateAnimation()
        {
            for (int i = 80; i >= 0; i = i - 10)
            {
                Debug.WriteLine($"Current Count: {i}");
                double brightness = ((double)i / 100);
                Debug.WriteLine($"Current Brightness: {brightness.ToString("F2")}");
                _eyeLEDPWM.DutyCycle = brightness;
                Thread.Sleep(25);
            }

            _eyeState = EyeStates.Off;

            Thread.Sleep(500);
        }

        public bool Close()
        {
            if (_currentState == ComponentState.Open)
            {
                _leftServoMotor.Start();
                _rightServoMotor.Start();
                Debug.WriteLine("Faceplate raised... closing faceplate!");
                var rightCurrentAngle = _rightServoMaxAngle;
                var leftCurrentAngle = _leftServoMaxAngle;
                while (_currentState != ComponentState.Closed)
                {
                    rightCurrentAngle -= 2;
                    leftCurrentAngle += 2;
                    _leftServoMotor.WriteAngle(leftCurrentAngle);
                    _rightServoMotor.WriteAngle(rightCurrentAngle);

                    Thread.Sleep(7);

                    if (rightCurrentAngle <= _rightServoMinAngle)
                    {
                        _currentState = ComponentState.Closed;
                    }
                }

                _leftServoMotor.Stop();
                _rightServoMotor.Stop();

                StartUpEyeAnimation();

                _currentState = ComponentState.Closed;
            }
            return true;
        }

        public bool Open()
        {
            if (_currentState != ComponentState.Open)
            {
                Debug.WriteLine("Faceplate closed... raising faceplate!");

                OpenFaceplateAnimation();

                _leftServoMotor.Start();
                _rightServoMotor.Start();

                var rightCurrentAngle = _rightServoMinAngle;
                var leftCurrentAngle = _leftServoMinAngle;
                while (_currentState != ComponentState.Open)
                {
                    rightCurrentAngle += 2;
                    leftCurrentAngle -= 2;
                    _leftServoMotor.WriteAngle(leftCurrentAngle);
                    _rightServoMotor.WriteAngle(rightCurrentAngle);

                    Thread.Sleep(7);

                    if (rightCurrentAngle >= _rightServoMaxAngle)
                    {
                        _currentState = ComponentState.Open;
                    }
                }

                _currentState = ComponentState.Open;
            }
            return true;
        }

        public void Animate()
        {
            if (_currentState != ComponentState.Closed)
            {
                Open();
            }
            else if (_currentState == ComponentState.Open)
            {
                Close();
            }
        }

        public void ToggleEyeState()
        {
            if (_eyeState == EyeStates.Off)
            {
                _eyeState = EyeStates.On;
                StartUpEyeAnimation();
            }
            else
            {
                _eyeState = EyeStates.Off;
                OpenFaceplateAnimation();
            }
        }
    }

}
