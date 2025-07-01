using Microsoft.AspNetCore.Mvc;

namespace Braphia.MedicalManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class HealthController : Controller
    {
        //private readonly ILogger<WeatherForecastController> _logger;

        //public WeatherForecastController(ILogger<WeatherForecastController> logger)
        //{
        //    _logger = logger;
        //}

        [HttpGet(Name = "")]
        public string Get()
        {
            return "Ok";
        }
    }
}

