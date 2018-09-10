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
            _logger.LogInformation($"{_resources.GetString("ExhaustAirTemp")} {TempControl.ExhaustAirTemp} C");
            _logger.LogInformation($"{_resources.GetString("CoolantTemp")} {TempControl.CoolantTemp} C");
            _logger.LogInformation($"{_resources.GetString("OutsideAirTemp")} {TempControl.OutsideAirTemp} C");
        }
    }
}