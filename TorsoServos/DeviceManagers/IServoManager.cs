using Iot.Device.ServoMotor;

namespace TorsoServos.DeviceManagers
{
    public interface IServoManager
    {
        public void Initialise();

        public void SetToStartPosition();

        public void ToggleMechanismState();

        public void OpenMechanism();

        public void CloseMechanism();
    }
}
