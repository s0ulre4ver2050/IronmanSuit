using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Adc;
using System.Device.Gpio;
using System.Threading;

namespace ServoZeroSetter.HardwareConfigurations
{
    public class TiltSensorReader
    {
        private GpioPin _sensorPin;
        private GpioController _gpioController;
        /*private AdcChannel _adcChannel0;
        private AdcChannel _adcChannel1;
        private AdcChannel _adcChannel2;
        private AdcChannel _adcChannel3;
        private AdcChannel _adcChannel4;
        private AdcChannel _adcChannel5;
        private AdcChannel _adcChannel6;*/
        private AdcChannel _adcChannel7;
        /*private AdcChannel _adcChannel8;
        private AdcChannel _adcChannel9;*/
        private AdcController _adcController;
        public bool LiveRead { get; set; } = true;

        public TiltSensorReader()
        {

        }

        public void Initialise()
        {
            //_gpioController = new GpioController();
            //_sensorPin = _gpioController.OpenPin(12);

            //_sensorPin.ValueChanged += ShowTiltSensorValue;
            //Configuration.SetPinFunction(25, DeviceFunction.ADC1_CH18);
            _adcController = new AdcController();
            int minValue = _adcController.MinValue;
            int maxValue = _adcController.MaxValue;

            int channelCount = _adcController.ChannelCount;

            /*_adcChannel0 = _adcController.OpenChannel(0);
            _adcChannel1 = _adcController.OpenChannel(1);
            _adcChannel2 = _adcController.OpenChannel(2);
            _adcChannel3 = _adcController.OpenChannel(3);
            _adcChannel4 = _adcController.OpenChannel(4);
            _adcChannel5 = _adcController.OpenChannel(5);
            _adcChannel6 = _adcController.OpenChannel(6);*/
            _adcChannel7 = _adcController.OpenChannel(7);
            /*_adcChannel8 = _adcController.OpenChannel(8);
            _adcChannel9 = _adcController.OpenChannel(9);*/

            //Console.WriteLine($"Channel |    0     |    1     |    2     |    3     |    4     |    5     |    6     |    7     |    8     |    9     |");
            Console.WriteLine($"Channel |    7     |    Timestamp   ");
            while (LiveRead)
            {
                Read();
                Thread.Sleep(1000);
            }
        }

        private void ShowTiltSensorValue(object sender, PinValueChangedEventArgs e)
        {
            Console.WriteLine("Reading value from the tilt sensor");
            Read();
        }

        public void Read()
        {
            /*var value0 = _adcChannel0.ReadValue();
            var value1 = _adcChannel1.ReadValue();
            var value2 = _adcChannel2.ReadValue();
            var value3 = _adcChannel3.ReadValue();
            var value4 = _adcChannel4.ReadValue();
            var value5 = _adcChannel5.ReadValue();
            var value6 = _adcChannel6.ReadValue();*/
            var value7 = _adcChannel7.ReadValue();
            /*var value8 = _adcChannel8.ReadValue();
            var value9 = _adcChannel9.ReadValue();*/

            //Console.WriteLine($"Value   |   { value0.ToString("d4") }   |   {value1.ToString("d4")}   |   {value2.ToString("d4")}   |   {value3.ToString("d4")}   |   {value4.ToString("d4")}   |   {value5.ToString("d4")}   |   {value6.ToString("d4")}   |   {value7.ToString("d4")}   |   {value8.ToString("d4")}   |   {value9.ToString("d4")}   |");
            Console.WriteLine($"Value   |   {value7.ToString("d4")}   | { DateTime.UtcNow }");
        }
    }
}
