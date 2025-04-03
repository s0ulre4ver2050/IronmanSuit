using System;

namespace ArcReactor.Services.Interfaces
{
    public interface ILEDDriverService : IDisposable
    {
        void Initialise(int pixelCount, int pinNumber);

        void Update(int redValue, int greenValue, int blueValue);
    }
}
