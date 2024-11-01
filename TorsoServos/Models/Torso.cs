using Iot.Device.Imu;
using Iot.Device.ServoMotor;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Adc;
using System.Device.I2c;
using System.Device.Pwm;
using System.Drawing;
using System.Numerics;
using System.Threading;
using TorsoServos.DeviceManagers;

namespace TorsoServos.Models
{
    public class Torso
    {
        #region servo managers

        private IServoManager _clavicleServoManager; // MG90S
        private IServoManager _shoulderCoverServoManager; // MG90S
        private IServoManager _turretServoManager; // MG90S
        private IServoManager _upperBackFlapManager; // MG90S

        #endregion

        #region private variables
        private readonly DeviceFunction _gyroscopeI2CDataDeviceFunction = DeviceFunction.I2C1_DATA;
        private readonly DeviceFunction _gyroscopeI2CClockDeviceFunction = DeviceFunction.I2C2_CLOCK;


        private readonly int _leftClaviclePin; // MG90S
        private readonly int _rightClaviclePin; // MG90S
        private readonly int _leftShoulderCoverPin; // MG90S
        private readonly int _rightShoulderCoverPin; // MG90S
        private readonly int _leftShoulderTurretPin; // MG90S
        private readonly int _rightShoulderTurretPin; // MG90S
        private readonly int _leftUpperBackFlapPin; // MG90S
        private readonly int _rightUpperBackFlapPin; // MG90S

        private Thread _gyroscopeThread;
        private readonly int _gyroscopeI2CDataPin;
        private readonly int _gyroscopeI2CClockPin;

        private Thread _radarThread;

        private GyroscopeManager _gyroscopeManager;
        private RCWL0516Manager _radarSensor;

        private readonly int _leftClavicleServoMinAngle = 0;
        private readonly int _rightClavicleServoMinAngle = 180;

        private readonly int _leftShoulderCoverServoMinAngle = 0;
        private readonly int _rightShoulderCoverServoMinAngle = 180;

        private readonly int _leftShoulderTurretServoMinAngle = 0;
        private readonly int _rightShoulderTurretServoMinAngle = 180;

        private readonly int _leftUpperBackFlapServoMinAngle = 0;
        private readonly int _rightUpperBackFlapServoMinAngle = 180;

        private readonly double _clavicleMovementAngle = 30;
        private readonly double _shoulderCoverMovementAngle = 150;
        private readonly double _shoulderTurretMovementAngle = 85;
        private readonly double _upperBackFlapMovementAngle = 90;

        private ComponentState _backPanelComponentState;
        private ComponentState _shoulderTurretComponentState;

        private bool _isExternalMotionDetected = false;
        private bool _isExternalMotionDetectedPreviousValue = false;
        private bool _isSuitMovementDetected = false;
        private bool _isSuitMovementDetectedPreviousValue = false;
        private bool _canAnimateAgain = true;

        private Thread _animationLockoutThread;

        #endregion

        #region constructor

        public Torso(
        int leftClaviclePin,
        int rightClaviclePin,
        int leftShoulderCoverPin,
        int rightShoulderCoverPin,
        int leftShoulderTurretPin,
        int rightShoulderTurretPin,
        int leftUpperBackFlapPin,
        int rightUpperBackFlapPin,
        int gyroscopeI2CDataPin,
        int gyroscopeI2CClockPin
        )
        {

            _leftClaviclePin = leftClaviclePin;
            _rightClaviclePin = rightClaviclePin;
            _leftShoulderCoverPin = leftShoulderCoverPin;
            _rightShoulderCoverPin = rightShoulderCoverPin;
            _leftShoulderTurretPin = leftShoulderTurretPin;
            _rightShoulderTurretPin = rightShoulderTurretPin;
            _leftUpperBackFlapPin = leftUpperBackFlapPin;
            _rightUpperBackFlapPin = rightUpperBackFlapPin;
            
            _gyroscopeI2CDataPin = gyroscopeI2CDataPin;
            _gyroscopeI2CClockPin = gyroscopeI2CClockPin;

            _backPanelComponentState = ComponentState.Initialising;
            _shoulderTurretComponentState = ComponentState.Initialising;
        }

        #endregion

        public enum ComponentState
        {
            Closed = 0,
            Open = 1,
            Testing = 2,
            Initialising = 3
        }

        public void Initialise()
        {
            #region setup the pin configurations

            Configuration.SetPinFunction(_leftClaviclePin, DeviceFunction.PWM1);
            Configuration.SetPinFunction(_rightClaviclePin, DeviceFunction.PWM2);
            Configuration.SetPinFunction(_leftShoulderCoverPin, DeviceFunction.PWM3);
            Configuration.SetPinFunction(_rightShoulderCoverPin, DeviceFunction.PWM4);
            Configuration.SetPinFunction(_leftShoulderTurretPin, DeviceFunction.PWM5);
            Configuration.SetPinFunction(_rightShoulderTurretPin, DeviceFunction.PWM6);
            Configuration.SetPinFunction(_leftUpperBackFlapPin, DeviceFunction.PWM7);
            Configuration.SetPinFunction(_rightUpperBackFlapPin, DeviceFunction.PWM8);

            Configuration.SetPinFunction(_gyroscopeI2CDataPin, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(_gyroscopeI2CClockPin, DeviceFunction.I2C1_CLOCK);

            #endregion

            #region setup the servo managers

            _clavicleServoManager = new MG90SServoManager(_leftClaviclePin, _rightClaviclePin, _leftClavicleServoMinAngle, _rightClavicleServoMinAngle, _clavicleMovementAngle); // MG90S
            _clavicleServoManager.Initialise();
            _clavicleServoManager.SetToStartPosition();
            Thread.Sleep(500);

            _shoulderCoverServoManager = new MG90SServoManager(_leftShoulderCoverPin, _rightShoulderCoverPin, _leftShoulderCoverServoMinAngle, _rightShoulderCoverServoMinAngle, _shoulderCoverMovementAngle); // MG90S
            _shoulderCoverServoManager.Initialise();
            _shoulderCoverServoManager.SetToStartPosition();
            Thread.Sleep(500);

            _turretServoManager = new MG90SServoManager(_leftShoulderTurretPin, _rightShoulderTurretPin, _leftShoulderTurretServoMinAngle, _rightShoulderTurretServoMinAngle, _shoulderTurretMovementAngle); // MG90S
            _turretServoManager.Initialise();
            _turretServoManager.SetToStartPosition();
            Thread.Sleep(500);

            _upperBackFlapManager = new MG90SServoManager(_leftUpperBackFlapPin, _rightUpperBackFlapPin, _leftUpperBackFlapServoMinAngle, _rightUpperBackFlapServoMinAngle, _upperBackFlapMovementAngle); // MG90S
            _upperBackFlapManager.Initialise();
            _upperBackFlapManager.SetToStartPosition();
            Thread.Sleep(500);

            #endregion

            #region setup the gyroscope

            _gyroscopeManager = new GyroscopeManager(_gyroscopeI2CDataPin, _gyroscopeI2CClockPin);
            _gyroscopeManager.Initialise();
            _gyroscopeManager.MotionDetected += ReadGyroscopeValues;

            #endregion

            #region setup the radar sensor

            _radarSensor = new RCWL0516Manager();
            _radarSensor.Initialise();
            _radarSensor.MotionDetected += ReadRadarValues;

            #endregion

            // Perform a power on self test and then once that has completed start reading from the gyroscope
            PowerOnSelfTest();
        }

        protected void PowerOnSelfTest()
        {
            TestServos();

            _gyroscopeManager.StartReading();
            _radarSensor.StartReading();

            Animate();
        }

        protected void TestServos()
        {
            bool testFlaps = true;
            bool testTurrets = true;

            if (testFlaps)
            {
                SuitMoving();
                Thread.Sleep(1000);
                SuitStopped();
            }

            if (testTurrets)
            {
                _shoulderTurretComponentState = ComponentState.Testing;
                ArmTurrets(true);
                Thread.Sleep(1000);
                DisarmTurrets(true);
            }
        }


        protected void Animate()
        {
            while (true)
            {
                // If the suit is moving then it doesnt matter if the radar detects motion
                if (_isSuitMovementDetected)
                {
                    SuitMoving();
                }
                else
                {
                    SuitStopped();
                }

                
                if (!_isExternalMotionDetected)
                {                                
                    DisarmTurrets();
                }
                if (_isExternalMotionDetected)
                {
                    ArmTurrets();
                }

                Thread.Sleep(5000); // Do this check every X seconds
            }
        }

        protected void AnimationLockoutCheck()
        {
            _canAnimateAgain = false;

            Thread.Sleep(30000); // set to 30 seconds before another animation effect can trigger

            _canAnimateAgain = true;
        }

        protected void ArmTurrets(bool istest = false)
        {
            if (_shoulderTurretComponentState != ComponentState.Open && _backPanelComponentState != ComponentState.Open && _canAnimateAgain)
            {
                Console.WriteLine("Arming Turrets");

                _clavicleServoManager.OpenMechanism();
                _shoulderCoverServoManager.OpenMechanism();
                _clavicleServoManager.CloseMechanism();
                _turretServoManager.OpenMechanism();
                _shoulderTurretComponentState = ComponentState.Open;

                if (!istest)
                {
                    _animationLockoutThread = new Thread(new ThreadStart(AnimationLockoutCheck));
                    _animationLockoutThread.Start();
                }
            }
        }

        protected void DisarmTurrets(bool istest = false)
        {
            if ((_shoulderTurretComponentState != ComponentState.Closed && _canAnimateAgain) || (_shoulderTurretComponentState == ComponentState.Open && _backPanelComponentState == ComponentState.Open))
            {
                Console.WriteLine("Disarming Turrets");

                _turretServoManager.CloseMechanism();
                _clavicleServoManager.OpenMechanism();
                _shoulderCoverServoManager.CloseMechanism();
                _clavicleServoManager.CloseMechanism();
                _shoulderTurretComponentState = ComponentState.Closed;

                if (!istest)
                {
                    _animationLockoutThread = new Thread(new ThreadStart(AnimationLockoutCheck));
                    _animationLockoutThread.Start();
                }
            }

        }

        protected void SuitMoving()
        {
            if (_backPanelComponentState != ComponentState.Open)
            {
                Console.WriteLine("Suit in motion");

                _upperBackFlapManager.OpenMechanism();

                _backPanelComponentState = ComponentState.Open;

                if (_shoulderTurretComponentState == ComponentState.Open)
                {
                    DisarmTurrets(true);
                }
            }
        }

        protected void SuitStopped()
        {
            if (_backPanelComponentState != ComponentState.Closed)
            {
                Console.WriteLine("Suit stationary");
                _upperBackFlapManager.CloseMechanism();

                _backPanelComponentState = ComponentState.Closed;
            }
        }

        #region mechanism control

        protected void OpenShoulderPanels()
        {
            _shoulderCoverServoManager.OpenMechanism();
        }

        protected void CloseShoulderPanels()
        {
            _shoulderCoverServoManager.CloseMechanism();
        }

        protected void OpenShoulderTurrets()
        {
            _turretServoManager.OpenMechanism();
        }

        protected void CloseShoulderTurrets()
        {
            _turretServoManager.CloseMechanism();
        }

        protected void OpenClavicle()
        {
            _clavicleServoManager.OpenMechanism();
        }

        protected void CloseClavicle()
        {
            _clavicleServoManager.CloseMechanism();
        }

        protected void OpenUpperBackFlaps()
        {
            _upperBackFlapManager.OpenMechanism();
        }

        protected void CloseUpperBackFlaps()
        {
            _upperBackFlapManager.CloseMechanism();
        }

        #endregion mechanism control

        private void ReadRadarValues(object sender, EventArgs e)
        {

            _isExternalMotionDetected = _radarSensor.SensorReadingValue != 0;
            if (_isExternalMotionDetectedPreviousValue != _isExternalMotionDetected)
            {
                _isExternalMotionDetectedPreviousValue = _isExternalMotionDetected;
            }  
        }

        private void ReadGyroscopeValues(object sender, EventArgs e) 
        {
            _isSuitMovementDetected = (_gyroscopeManager.GyroscopeMotionDetected || _gyroscopeManager.AccelerometerMotionDetected);

            if (_isSuitMovementDetectedPreviousValue != _isSuitMovementDetected)
            {
                _isSuitMovementDetectedPreviousValue = _isSuitMovementDetected;
            }

        }
    }
}
