using Iot.Device.ServoMotor;
using System;
using System.Device.Pwm;
using System.Threading;

namespace TorsoServos.DeviceManagers
{
    public class BaseServoManager : IServoManager
    {
        protected readonly int _leftServoPin;
        protected readonly int _rightServoPin;
        protected readonly int _leftMinAngle;
        protected readonly int _rightMinAngle;
        protected readonly double _movementRange;
        protected PwmChannel _leftPwmChannel;
        protected PwmChannel _rightPwmChannel;
        protected ServoMotor _leftServoMotor;
        protected ServoMotor _rightServoMotor;
        protected Globals.ComponentState _componentState;
        private readonly int _servoMovementWaitTime = 10;

        public BaseServoManager(int leftServoPin, int rightServoPin, int leftMinAngle, int rightMinAngle, double movementRange)
        {
            _leftServoPin = leftServoPin;
            _leftMinAngle = leftMinAngle;
            _rightServoPin = rightServoPin;
            _rightMinAngle = rightMinAngle;
            _movementRange = movementRange;
        }

        public void Initialise()
        {
            /*_leftServoMotor.Start();
            _rightServoMotor.Start();

            Thread.Sleep(250);

            _leftServoMotor.Stop();
            _rightServoMotor.Stop();*/
        }

        public void SetToStartPosition()
        {
            _leftServoMotor.Start();
            _rightServoMotor.Start();

            _leftServoMotor.WriteAngle(_leftMinAngle);
            _rightServoMotor.WriteAngle(_rightMinAngle);

            Thread.Sleep(200);

            _leftServoMotor.Stop();
            _rightServoMotor.Stop();

            _componentState = Globals.ComponentState.Closed;
        }

        public void ToggleMechanismState()
        {
            if (_componentState != Globals.ComponentState.Open)
            {
                OpenMechanism();
            }
            else
            {
                CloseMechanism();
            }
        }

        public void OpenMechanism()
        {
            double leftCurrentAngle = _leftMinAngle;
            double rightCurrentAngle = _rightMinAngle;

            _leftServoMotor.Start();
            _rightServoMotor.Start();

            for (int i = 0; i < _movementRange; i++)
            {
                leftCurrentAngle++; // Increment by 1 each iteration
                rightCurrentAngle--;

                _leftServoMotor.WriteAngle(leftCurrentAngle);
                _rightServoMotor.WriteAngle(rightCurrentAngle);

                Thread.Sleep(_servoMovementWaitTime);
            }

            _leftServoMotor.Stop();
            _rightServoMotor.Stop();

            _componentState = Globals.ComponentState.Open;
        }

        public void CloseMechanism()
        {
            double leftCurrentAngle = _leftMinAngle + _movementRange;
            double rightCurrentAngle = _rightMinAngle - _movementRange;

            _leftServoMotor.Start();
            _rightServoMotor.Start();

            for (int i = 0; i < _movementRange; i++)
            {
                leftCurrentAngle--; // Decrement by 1 each iteration
                rightCurrentAngle++;

                _leftServoMotor.WriteAngle(leftCurrentAngle);
                _rightServoMotor.WriteAngle(rightCurrentAngle);

                Thread.Sleep(_servoMovementWaitTime);
            }

            _leftServoMotor.Stop();
            _rightServoMotor.Stop();

            _componentState = Globals.ComponentState.Closed;
        }
    }
}
