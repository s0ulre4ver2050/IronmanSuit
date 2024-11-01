using Iot.Device.ServoMotor;
using System;

namespace ServoZeroSetter.HardwareConfigurations
{
    public interface IServoManager
    {
        public void Initialise();

        public void SetToStartPosition();

        public void ToggleMechanismState();

        public void OpenMechanism(ServoMotor leftServo, ServoMotor rightServo, double leftMinAngle, double rightMinAngle, double targetAngle);

        public void CloseMechanism(ServoMotor leftServo, ServoMotor rightServo, double leftMinAngle, double rightMinAngle, double targetAngle);
    }
}
