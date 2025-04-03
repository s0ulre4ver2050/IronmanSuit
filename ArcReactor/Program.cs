using System;
using System.Threading;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using ArcReactor.Services.Interfaces;
using ArcReactor.Services;

namespace ArcReactor
{
    public class Program
    {
        public static void Main()
        {
            ServiceProvider serviceProvider = ConfigureServices();
            var application = (Application)serviceProvider.GetRequiredService(typeof(Application));

            application.Run();
        }

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(typeof(Application))
                .AddSingleton(typeof(ILEDDriverService),typeof(LEDDriverService))
                .AddSingleton(typeof(ISPILEDDriverService), typeof(SPILEDDriverService))
                .AddSingleton(typeof(IArcReactorService), typeof(ArcReactorService))
                .BuildServiceProvider();
        }
    }
}
