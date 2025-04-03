using ArcReactor.Services.Interfaces;
using System;
using System.Threading;

namespace ArcReactor
{
    internal class Application
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IArcReactorService _arcReactorService;

        public Application(IServiceProvider serviceProvider, IArcReactorService arcReactorService)
        {
            _serviceProvider = serviceProvider;
            _arcReactorService = arcReactorService;
        }

        public void Run()
        {
            // Define any pin numbers and other variables for configuring things in the arc reactor service
            int pinNumber = 26;

            _arcReactorService.Initialise(pinNumber);
            Thread.Sleep(250); // Sleep for 1/4 of a second
            _arcReactorService.Start();
        }
    }
}
