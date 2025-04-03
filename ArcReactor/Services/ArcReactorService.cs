using ArcReactor.Services.Interfaces;
using System;
using System.Threading;

namespace ArcReactor.Services
{
    public class ArcReactorService : IArcReactorService , IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILEDDriverService _ledDriverService;
        private Thread _thread;

        private readonly float _minimumBrightnessValue = 0.2f; // 20% Brightness
        private readonly float _maximumBrightnessValue = 1.0f; // 100% Brightness
        private float _currentBrightnessValue = 1.0f; // Start with 100% brightness
        private float _brightnessIncrementValue = 0.025f;

        private int _baseRedValue = 80;
        private int _baseGreenValue = 190;
        private int _baseBlueValue = 240;

        private int _redValue;
        private int _greenValue;
        private int _blueValue;

        private bool _isGettingBrighter = true;
        private bool _isGettingDimmer = false;
        private bool _isAnimating = false;

        private Random _random;

        public ArcReactorService(IServiceProvider serviceProvider, ILEDDriverService ledDriverService)
        {
            _serviceProvider = serviceProvider;
            _ledDriverService = ledDriverService;
        }

        public void Animate()
        {
            try
            {
                _ledDriverService.Update(_redValue, _greenValue, _blueValue);
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message);
            }

        }

        public void Initialise(int pinNumber)
        {
            _random = new Random();
            int pixelCount = 19;
            _ledDriverService.Initialise(pixelCount, pinNumber);

            _thread = new Thread(() =>
            {
                while (true)
                {
                    AnimateDimmingEffect();
                    Thread.Sleep(50);
                    AnimateBrighteningEffect();
                    int randomValue = _random.Next(60); // 60 here relates to number of seconds
                    Thread.Sleep(randomValue * 1000); // We have to multiply by 1000 because Thread.Sleep expects milliseconds;
                }
            });
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Stop()
        {
            _thread = null;
        }

        public void Dispose()
        {
            _ledDriverService.Dispose();
        }

        private void AnimateDimmingEffect()
        {
            _currentBrightnessValue = _maximumBrightnessValue;
            _isGettingBrighter = false;
            _isGettingDimmer = true;
            _isAnimating = true;

            while (_isGettingDimmer && _isAnimating)
            {
                // As long as the current brightness value is greater than the specified minimum value
                if (_currentBrightnessValue > _minimumBrightnessValue)
                {
                    _currentBrightnessValue -= _brightnessIncrementValue;

                    _redValue = ValueFromPercentage(_currentBrightnessValue, _baseRedValue);
                    _greenValue = ValueFromPercentage(_currentBrightnessValue, _baseGreenValue);
                    _blueValue = ValueFromPercentage(_currentBrightnessValue, _baseBlueValue);

                    Animate();
                }

                if (_currentBrightnessValue <= _minimumBrightnessValue)
                {
                    _isGettingDimmer = false;
                    _isAnimating = false;
                }

            }
        }

        private void AnimateBrighteningEffect()
        {
            _currentBrightnessValue = _minimumBrightnessValue;
            _isGettingBrighter = true;
            _isGettingDimmer = false;
            _isAnimating = true;

            while (_isGettingBrighter && _isAnimating)
            {
                // As long as the current brightness value is greater than the specified minimum value
                if (_currentBrightnessValue < _maximumBrightnessValue)
                {
                    _currentBrightnessValue += _brightnessIncrementValue;

                    _redValue = ValueFromPercentage(_currentBrightnessValue, _baseRedValue);
                    _greenValue = ValueFromPercentage(_currentBrightnessValue, _baseGreenValue);
                    _blueValue = ValueFromPercentage(_currentBrightnessValue, _baseBlueValue);

                    Animate();
                }

                if (_currentBrightnessValue >= _maximumBrightnessValue)
                {
                    _isGettingBrighter = false;
                    _isAnimating = false;
                }
            }
        }

        private int ValueFromPercentage(float modifier, int baseValue)
        {
            return (int)Math.Round((double)modifier * baseValue);
        }
    }
}
