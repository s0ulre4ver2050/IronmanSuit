using Iot.Device.ServoMotor;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Pwm;
using System.Threading;
using Core.HardwareConfigurations;

namespace TorsoServos.DeviceManagers
{
    public class MG90SServoManager : BaseServoManager
    {

        public MG90SServoManager(int leftServoPin, int rightServoPin, int leftMinAngle, int rightMinAngle, double movementRange) : base(leftServoPin, rightServoPin, leftMinAngle, rightMinAngle, movementRange)
        {
        }

        public void Initialise()
        {
            
            _leftPwmChannel = PwmChannel.CreateFromPin(pin: _leftServoPin, frequency: 50, dutyCyclePercentage: 1);
            _rightPwmChannel = PwmChannel.CreateFromPin(pin: _rightServoPin, frequency: 50, dutyCyclePercentage: 1);

            _leftServoMotor = new ServoMotor(_leftPwmChannel,
                MG90SServoSettings.MaximumAngle,
                MG90SServoSettings.MinimumPulseWidth,
                MG90SServoSettings.MaximumPulseWidth);
            _rightServoMotor = new ServoMotor(_rightPwmChannel,
                MG90SServoSettings.MaximumAngle,
                MG90SServoSettings.MinimumPulseWidth,
                MG90SServoSettings.MaximumPulseWidth);

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
