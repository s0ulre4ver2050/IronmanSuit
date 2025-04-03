using System;

namespace ArcReactor.Services.Interfaces
{
    public interface IArcReactorService : IDisposable
    {
        void Initialise(int pinNumber);
        void Start();
        void Animate();
        void Stop();

    }
}
