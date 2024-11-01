using Core.HardwareConfigurations;
using Iot.Device.ServoMotor;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Pwm;
using System.Threading;

namespace ServoZeroSetter.HardwareConfigurations
{
    public class MG90SServoManager : IServoManager
    {
        private int _numberOfServos = 2;

        private DeviceFunction _leftServoDeviceFunction = DeviceFunction.PWM1;
        private DeviceFunction _rightServoDeviceFunction = DeviceFunction.PWM2;

        private int _leftServoPin = 25;
        private int _rightServoPin = 26; // Can also use 12;

        private PwmChannel _leftServoPWM;
        private PwmChannel _rightServoPWM;

        private ServoMotor _leftServo;
        private ServoMotor _rightServo;

        private readonly int _leftServoMinAngle = 0;
        private readonly int _rightServoMinAngle = 180;

        private readonly int _movementAngle = 70;

        private bool _isOpen = false;

        public MG90SServoManager() 
        {
            
        }

        public void Initialise()
        {
            Configuration.SetPinFunction(_leftServoPin, _leftServoDeviceFunction);
            Configuration.SetPinFunction(_rightServoPin, _rightServoDeviceFunction);

            _leftServoPWM = PwmChannel.CreateFromPin(pin: _leftServoPin, frequency: 50, dutyCyclePercentage: 0.5);
            _rightServoPWM = PwmChannel.CreateFromPin(pin: _rightServoPin, frequency: 50, dutyCyclePercentage: 0.5);

            _leftServo = new ServoMotor(_leftServoPWM,
                MG90SServoSettings.MaximumAngle,
                MG90SServoSettings.MinimumPulseWidth,
                MG90SServoSettings.MaximumPulseWidth);

            _rightServo = new ServoMotor(_rightServoPWM,
                MG90SServoSettings.MaximumAngle,
                MG90SServoSettings.MinimumPulseWidth,
                MG90SServoSettings.MaximumPulseWidth);
        }

        public void SetToStartPosition()
        {
            _leftServo.WriteAngle(_leftServoMinAngle);
            _rightServo.WriteAngle(_rightServoMinAngle);

            _leftServo.Start();
            _rightServo.Start();

            Thread.Sleep(1500);

            _leftServo.Stop();
            _rightServo.Stop();

            _isOpen = false;
        }

        public void ToggleMechanismState()
        {
            if (!_isOpen)
            {
                Console.WriteLine("Opening mechanism!");

                OpenMechanism(_leftServo, _rightServo, _leftServoMinAngle, _rightServoMinAngle, _movementAngle);

                _isOpen = true;
            }
            else
            {
                Console.WriteLine("Closing mechanism!");
                CloseMechanism(_leftServo, _rightServo, _leftServoMinAngle + _movementAngle, _rightServoMinAngle - _movementAngle, _movementAngle);
                _isOpen = false;
            }
        }

        public void OpenMechanism(ServoMotor leftServo, ServoMotor rightServo, double leftMinAngle, double rightMinAngle, double targetAngle)
        {
            Console.WriteLine("Opening Mechanism Start");
            double currentAngleLeft = leftMinAngle;
            double currentAngleRight = rightMinAngle;

            leftServo.Start();
            rightServo.Start();

            for (int i = 0; i < targetAngle; i++)
            {
                currentAngleLeft++; // Increment by 1 each iteration
                currentAngleRight--; // Decrement by 1 each iteration

                leftServo.WriteAngle(currentAngleLeft);
                rightServo.WriteAngle(currentAngleRight);

                Thread.Sleep(10); // Sleep for 10 MS
            }

            leftServo.Stop();
            rightServo.Stop();

            Console.WriteLine("Mechanism Opened");
        }

        public void CloseMechanism(ServoMotor leftServo, ServoMotor rightServo, double leftMinAngle, double rightMinAngle, double targetAngle)
        {
            Console.WriteLine("Closing Mechanism Start");
            double currentAngleLeft = leftMinAngle;
            double currentAngleRight = rightMinAngle;

            leftServo.Start();
            rightServo.Start();

            for (int i = 0; i < targetAngle; i++)
            {
                currentAngleLeft--; // Decrement by 1 each iteration
                currentAngleRight++; // Increment by 1 each iteration

                leftServo.WriteAngle(currentAngleLeft);
                rightServo.WriteAngle(currentAngleRight);

                Thread.Sleep(10); // Sleep for 10 MS
            }

            leftServo.Stop();
            rightServo.Stop();

            Console.WriteLine("Mechanism Closed");
        }
    }
}
