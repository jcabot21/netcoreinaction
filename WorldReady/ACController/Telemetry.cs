using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Logging;

namespace ACController
{
    public class Telemetry
    {
        private readonly ILogger _logger;
        private readonly ResourceManager _resources;

        public Telemetry(ILogger logger)
        {
            _logger = logger;
            _resources = new ResourceManager("ACController.strings", typeof(Program).GetTypeInfo().Assembly);
        }

        public void LogStatus()
        {
            _logger.LogInformation(string.Format(_resources.GetString("ExhaustAirTemp"), TempControl.ExhaustAirTemp));
            _logger.LogInformation(string.Format(_resources.GetString("CoolantTemp"), TempControl.CoolantTemp));
            _logger.LogInformation(string.Format(_resources.GetString("OutsideAirTemp"), TempControl.OutsideAirTemp));
        }
    }
}