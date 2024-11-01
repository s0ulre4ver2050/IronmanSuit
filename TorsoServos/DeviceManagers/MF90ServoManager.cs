using Iot.Device.ServoMotor;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Pwm;
using System.Threading;
using Core.HardwareConfigurations;

namespace TorsoServos.DeviceManagers
{
    public class MF90ServoManager : BaseServoManager
    {
        public MF90ServoManager(int leftServoPin, int rightServoPin, int leftMinAngle, int rightMinAngle, double movementRange) : base(leftServoPin, rightServoPin, leftMinAngle, rightMinAngle, movementRange)
        {
        }

        public void Initialise()
        {
            _leftPwmChannel = PwmChannel.CreateFromPin(pin: _leftServoPin, frequency: 50, dutyCyclePercentage: 0.5);
            _rightPwmChannel = PwmChannel.CreateFromPin(pin: _rightServoPin, frequency: 50, dutyCyclePercentage: 0.5);

            _leftServoMotor = new ServoMotor(_leftPwmChannel,
                MF90ServoSettings.MaximumAngle,
                MF90ServoSettings.MinimumPulseWidth,
                MF90ServoSettings.MaximumPulseWidth);
            _rightServoMotor = new ServoMotor(_rightPwmChannel,
                MF90ServoSettings.MaximumAngle,
                MF90ServoSettings.MinimumPulseWidth,
                MF90ServoSettings.MaximumPulseWidth);

            base.Initialise();

            _componentState = Globals.ComponentState.Initialising;
        }

        public void SetToStartPosition()
        {
            base.SetToStartPosition();
        }

        public void ToggleMechanismState()
        {
            base.ToggleMechanismState();
        }

        public void OpenMechanism()
        {
            base.OpenMechanism();
        }

        public void CloseMechanism()
        {
            base.CloseMechanism();
        }
    }
}
