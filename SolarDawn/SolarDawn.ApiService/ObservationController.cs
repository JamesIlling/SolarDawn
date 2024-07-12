using Microsoft.AspNetCore.Mvc;
using SolarDawn.Shared;

namespace SolarDawn.ApiService
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObservationController : ControllerBase
    {
        private readonly ILogger<ObservationController> _logger;

        public ObservationController(ILogger<ObservationController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok();
        }

        [HttpPost]
        public ActionResult AddObservation([FromBody] WeatherObservation weatherObservation)
        {
            _logger.LogInformation("Received {observation}", weatherObservation);
            return Ok();
        }
    }
}
