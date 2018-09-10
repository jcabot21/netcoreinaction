using Microsoft.Extensions.Logging;

namespace ACController
{
    public class Controller 
    {
        private readonly LoggerFactory _loggerFactory;
        private readonly Telemetry _telemetry;

        public Controller()
        {
            _loggerFactory = new LoggerFactory().AddRobust();
            _telemetry = new Telemetry(_loggerFactory.CreateLogger<Telemetry>());
        }

        public void Test()
        {
            _telemetry.LogStatus();
        }
    }
}